using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{
    public static partial class GitHubBridge
    {
        private static GitHubClient       client;

        private static string             Owner             = "VoodooTeam";
        private static string             StoreRepository   = "VoodooStore";
        private static string             MasterRef         = "master";

        private static NewTree            tempTree;

        private static List<Task>         fetchingTasks = new List<Task>();

        public static string              errorMessage;

        public static bool                isPushing = false;
        
        public static event Action<float> fetchChanged;
        public static event Action        fetchCompleted;

        private static GitHubClient Client 
        {
            get 
            {
                if (client == null)
                {
                    SetupClient();
                }

                return client;
            }
        }

        private static Branch Master => Client.Repository.Branch.Get(Owner, StoreRepository, MasterRef).Result;

        public static void OnEnable() 
        {
            SetupClient();
        }

        public static void SetupClient()
        {
            errorMessage = "";
            ProductHeaderValue productInformation = new ProductHeaderValue("VoodooUser");
            if (string.IsNullOrEmpty(VoodooStore.signInToken))
            {
                return;
            }
            Credentials credentials = new Credentials(VoodooStore.signInToken);
            client = new GitHubClient(productInformation) { Credentials = credentials };
        }

        public static async Task<bool> TryGetMaster() 
        {
            try 
            {
                _ = await Client.Repository.Get(Owner, StoreRepository);
            }
            catch
            {
                errorMessage = "Invalid credentials. Please check your rights toward the store with GTD team.";
                VoodooStoreEditor.state = State.UNSIGNED;
                return false;
            }
            
            errorMessage = "";

            await FetchMaster();
            return true;
        }

        public static async Task FetchMaster()
        {
            VoodooStoreEditor.state = State.FETCHING;

            string latestSha = Master.Commit.Sha;
            if (VoodooStore.masterSha == latestSha)
            {
                FinishFetch();
                return;
            }

            VoodooStore.masterSha = latestSha;

            TreeResponse recursiveRequest = await Client.Git.Tree.GetRecursive(Owner, StoreRepository, latestSha);
            if (recursiveRequest.Truncated)
            {
                TreeResponse request = await Client.Git.Tree.Get(Owner, StoreRepository, latestSha);
                await FetchTree(request, true);
            }
            else
            {
                await FetchTree(recursiveRequest);
            }

            await Task.WhenAll(fetchingTasks);
            FinishFetch();

            VoodooStore.RemoveDeletedPackages();
        }

        public static async Task FetchTree(TreeResponse response, bool isRecursive = false) 
        {
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < response.Tree.Count; i++)
            {
                TreeItem file = response.Tree[i];
                if (isFileIgnored(file))
                {
                    continue;
                }

                if (file.Type == TreeType.Tree)
                {
                    if (isRecursive)
                    {
                        TreeResponse subTree = await Client.Git.Tree.Get(Owner, StoreRepository, file.Sha);
                        tasks.Add(FetchTree(subTree, true));
                    }

                    continue;
                }

                Package package = VoodooStore.GetPackageByPath(file.Path);
                package.existRemotely = true;

                if (file.Path.Contains(PathHelper.unityPackage) && file.Sha != package.pluginSha)
                {
                    package.pluginSha   = file.Sha;
                    package.size        = file.Size;
                }
                
                if (file.Path.Contains(PathHelper.readme) && file.Sha != package.serverSha)
                {
                    //IncFetchingGoal();
                    fetchingTasks.Add(FetchPackageInfo(package, file.Sha));
                }
            }

            Task.WaitAll(tasks.ToArray());
        }

        public static bool isFileIgnored(TreeItem file)
        {
            for (int i = 0; i < PathHelper.IgnoredFiles.Count; i++)
            {
                if (file.Path.Contains(PathHelper.IgnoredFiles[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static async Task FetchPackageInfo(Package pkg, string sha) 
        {
            byte[]  bytes           = await GetBlobContent(sha);
            string  json            = System.Text.Encoding.UTF8.GetString(bytes);
            Package packageFromJson = JsonUtility.FromJson<Package>(json);

            pkg.serverSha = sha;
            if (pkg.VersionStatus == VersionState.UpToDate)
            {
                pkg.version = packageFromJson.version;
            }

            pkg.name                = packageFromJson.name;
            pkg.author              = packageFromJson.author;
            pkg.category            = packageFromJson.category;
            pkg.subCategory         = packageFromJson.subCategory;
            pkg.description         = packageFromJson.description;
            pkg.displayName         = packageFromJson.displayName;
            pkg.unityVersion        = packageFromJson.unityVersion;
            pkg.version             = packageFromJson.version;
            pkg.documentationLink   = packageFromJson.documentationLink;
            pkg.dependencies        = packageFromJson.dependencies;
            pkg.tags                = packageFromJson.tags;
            
            //DecFetchingFiles();
        }

        //public static void IncFetchingGoal()
        //{
        //    System.Threading.Interlocked.Increment(ref fetchingFilesCount);
        //}

        //public static void DecFetchingFiles()
        //{
        //    System.Threading.Interlocked.Increment(ref fetchingFiles);
        //    fetchChanged?.Invoke((float)fetchingFiles / (float)fetchingFilesCount);
            
        //    if (fetchingFiles >= fetchingFilesCount)
        //    {
        //        FinishFetch();
        //    }
        //}

        private static void FinishFetch() 
        {
            VerifyExisting();

            fetchCompleted?.Invoke();
            VoodooStoreEditor.state = State.FETCH_OK;
        }

        private static void VerifyExisting()
        {
            for (int i = 0; i < VoodooStore.packages.Count; i++)
            {
                string packagePath = Path.Combine(UnityEngine.Application.dataPath, "VoodooPackages", VoodooStore.packages[i].name);
                if (VoodooStore.packages[i].VersionStatus == VersionState.NotPresent && 
                    Directory.Exists(packagePath))
                {
                    VoodooStore.packages[i].localSha = "manually";
                }

                if (VoodooStore.packages[i].isInstalled &&
                    Directory.Exists(packagePath) == false)
                {
                    VoodooStore.packages[i].localSha = string.Empty;
                }
            }
        }

        public static async Task<byte[]> GetBlobContent(string sha)
        {
            Blob blob = await Client.Git.Blob.Get(Owner, StoreRepository, sha);
            if (blob == null)
            {
                Debug.LogError("the blob should not be null");
            }

            return Convert.FromBase64String(blob.Content);
        }

        public static async Task CommitAdd(string filePath, string content) 
        {
            // if there is no temporary tree, create a new one.
            // it will contains all file added to the commit 
            // and will have the previous commit as parent
            if (tempTree == null)
            {
                Octokit.Commit latestCommit = await Client.Git.Commit.Get(Owner, StoreRepository, Master.Commit.Sha);
                tempTree = new NewTree { BaseTree = latestCommit.Tree.Sha };
            }

            // Create the blob(s) corresponding to your file(s)
            NewBlob         tempBlob    = new NewBlob { Encoding = EncodingType.Utf8, Content = content };
            BlobReference   blobRef     = await Client.Git.Blob.Create(Owner, StoreRepository, tempBlob);

            // Add blob to commitTree
            tempTree.Tree.Add(new NewTreeItem { Path = filePath, Mode = "100644", Type = TreeType.Blob, Sha = blobRef.Sha });
        }

        public static async Task CommitAdd(string filePath, byte[] content)
        {
            // if there is no temporary tree, create a new one.
            // it will contains all file added to the commit 
            // and will have the previous commit as parent
            if (tempTree == null)
            {
                Octokit.Commit latestCommit = await Client.Git.Commit.Get(Owner, StoreRepository, Master.Commit.Sha);
                tempTree = new NewTree { BaseTree = latestCommit.Tree.Sha };
            }

            // Create the blob(s) corresponding to your file(s)
            NewBlob         tempBlob    = new NewBlob { Encoding = EncodingType.Base64, Content = Convert.ToBase64String(content) };
            BlobReference   blobRef     = await Client.Git.Blob.Create(Owner, StoreRepository, tempBlob);

            // Add blob to commitTree
            tempTree.Tree.Add(new NewTreeItem { Path = filePath, Mode = "100644", Type = TreeType.Blob, Sha = blobRef.Sha});
        }

        public static async void Push(string commitMessage) 
        {
            if (tempTree == null)
            {
                Debug.LogError("You are trying to push a commit with no file");
            }

            isPushing = true;

            TreeResponse tree       = await Client.Git.Tree.Create(Owner, StoreRepository, tempTree);

            // Create the commit with the SHAs of the tree and the reference of master branch
            NewCommit    tempCommit = new NewCommit(commitMessage, tree.Sha, Master.Commit.Sha);
            var          commit     = await Client.Git.Commit.Create(Owner, StoreRepository, tempCommit);

            // Update the reference of master branch with the SHA of the commit
            // huge warning you have to use "refs" and at least 2 slash for your ref to be considered, I found it in github doc
            await Client.Git.Reference.Update(Owner, StoreRepository, "refs/heads/master", new ReferenceUpdate(commit.Sha));

            tempTree    = null;
            isPushing   = false;

            _ = TryGetMaster();
        }

        public static void Dispose() 
        {
            fetchChanged    = null;
            fetchCompleted  = null;

            client          = null;
        }
    }
}