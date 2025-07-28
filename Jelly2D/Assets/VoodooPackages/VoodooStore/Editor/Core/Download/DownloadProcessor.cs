using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{
    public static class DownloadProcessor
    {
        static Queue<DownloadRequest> pendingRequests = new Queue<DownloadRequest>();
        static DownloadRequest        request;

        public static bool HasRequestRunning => pendingRequests.Count > 0 || request != null;

        ///////////Install functions/////////////
        public static DownloadRequest DownloadPackage(Package package)
        {
            DownloadRequest request = new DownloadRequest(new List<Package> { package }, true);
            AddRequest(request);

            return request;
        }

        public static DownloadRequest DownloadPackages(List<Package> packages)
        {
            if (packages.Count == 1)
            {
                return DownloadPackage(packages[0]);
            }
            
            int downloadOption = GetDownloadOption(packages);
            if (downloadOption == 1 )
            {
                return null;
            }

            DownloadRequest request = new DownloadRequest(packages, downloadOption == 2);
            AddRequest(request);

            return request;
        }

        public static int GetDownloadOption(List<Package> pkgs) 
        {
            StringBuilder message = new StringBuilder("They are multiple packages to download due to dependencies:")
                .Append(Environment.NewLine).Append(Environment.NewLine);

            int count = pkgs.Count;
            for (int i = 0; i < count; i++)
            {
                if (pkgs[i].VersionStatus != VersionState.UpToDate)
                {
                    message.Append(pkgs[i].displayName).Append(Environment.NewLine);
                }
            }

            message.Append(Environment.NewLine).Append("Do you want to download all of them at the same time or one by one ?");

            return EditorUtility.DisplayDialogComplex("Warning", message.ToString(), "All of them", "Cancel", "One by One");
        }

        private static void AddRequest(DownloadRequest newRequest)
        {
            pendingRequests.Enqueue(newRequest);
            if (request != null)
            {
                return;
            }

            NextRequest();
        }

        private static void NextRequest()
        {
            if (pendingRequests.Count <= 0)
            {
                request = null;
                return;
            }

            request = pendingRequests.Dequeue();
            request.Start(NextPackage, NextRequest);
        }

        public static async void NextPackage()
        {
            Package pkg = request.Current;
            
            //TODO here check if local downloaded pkg is up to date if so skip download

            byte[] byteArray = await GitHubBridge.GetBlobContent(pkg.pluginSha);

            string directoryPath = Path.Combine(PathHelper.DirectoryPath, "Packages");
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            string path = Path.Combine(directoryPath, pkg.name + PathHelper.unityPackage);
            File.WriteAllBytes(path, byteArray);

            AssetDatabase.ImportPackage(path, request.interactiveDownload);
        }

        public static void UninstallProcess(List<Package> packages, bool askConfirmation = false)
        {
            for (int i = packages.Count - 1; i >= 0; --i)
            {
                string packageDataPath = Application.dataPath + "/VoodooPackages/" + packages[i].name;

                if (packages[i].VersionStatus == VersionState.NotPresent)
                {
                    packages.RemoveAt(i);
                    continue;
                }

                if (Directory.Exists(packageDataPath) == false)
                {
                    packages[i].localSha = string.Empty;
                    packages.RemoveAt(i);
                    continue;
                }
            }

            string _warningMessage = "The following packages will be uninstall:" + Environment.NewLine + Environment.NewLine;

            for (int i = 0; i < packages.Count; i++)
            {
                _warningMessage += packages[i].displayName + Environment.NewLine;
            }

            _warningMessage += Environment.NewLine;
            _warningMessage += Environment.NewLine + "Are you sure you want to do that ?";

            if (EditorUtility.DisplayDialog("Warning", _warningMessage, "Yes", "Cancel"))
            {
                for (int i = packages.Count - 1; i >= 0; i--)
                {
                    Uninstall(packages[i]);
                }

                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
        }

        public static void Uninstall(Package package)
        {
            string packageDataPath = Application.dataPath + "/VoodooPackages/" + package.name;

            if (Directory.Exists(packageDataPath))
            {
                Directory.Delete(packageDataPath, true);
            }

            package.localSha = string.Empty;
        }
    }

    public sealed class DownloadRequest
    {
        bool disposed;

        public List<Package> packages;
        public int           index;
        public bool          interactiveDownload;
        
        public event Action<ImportResult, string> packageImported;
        public event Action readyForNextPackage;
        public event Action requestComplete;

        public DownloadRequest(List<Package> pkgs, bool auto) 
        {
            packages     = pkgs;
            index        = -1;
            interactiveDownload = auto;
            disposed     = false;
        }

        public Package Current => packages[index];

        public void Start(Action onNext, Action onComplete) 
        {
            AssetDatabase.importPackageCompleted += ImportCompleted;
            AssetDatabase.importPackageCancelled += ImportCancelled;
            AssetDatabase.importPackageFailed    += ImportFailed;
            
            readyForNextPackage += onNext;
            requestComplete += onComplete;

            EditorApplication.LockReloadAssemblies();
#if UNITY_2019_3_OR_NEWER
            AssetDatabase.DisallowAutoRefresh();
#endif

            Next();
        }

        void ImportCompleted(string packageName)
        {
            Current.localSha = Current.serverSha;
            packageImported?.Invoke(ImportResult.Success, string.Empty);
            Next();
        }

        void ImportCancelled(string packageName)
        {
            Debug.LogWarning(packageName + " download has been cancelled");
            packageImported?.Invoke(ImportResult.Cancelled, string.Empty);
            packages.RemoveAt(index);
            index--;
            Next();
        }

        void ImportFailed(string packageName, string errorMessage)
        {
            Debug.LogError(packageName + " download has failed with message : " + errorMessage);
            packageImported?.Invoke(ImportResult.Failure, errorMessage);
            Next();
        }

        void Next() 
        {
            index++;
            while(index < packages.Count && Current.VersionStatus == VersionState.UpToDate)
            {
                index++;
            }

            if (index >= packages.Count)
            {
                End();
                requestComplete?.Invoke();
                Dispose();
            }
            else
            {
                readyForNextPackage?.Invoke();
            }
        }

        void End() 
        {
            EditorApplication.UnlockReloadAssemblies();
#if UNITY_2019_3_OR_NEWER
            AssetDatabase.AllowAutoRefresh();
#endif

            ImportAssetOptions importOption = ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive;
            foreach (var package in packages)
            {
                AssetDatabase.ImportAsset("Assets/VoodooPackages/" + package.Name, importOption);
            }

            AssetDatabase.Refresh();
        }

        public void Dispose() 
        {
            if (disposed)
            {
                return;
            }

            disposed = true;

            AssetDatabase.importPackageCompleted -= ImportCompleted;
            AssetDatabase.importPackageCancelled -= ImportCancelled;
            AssetDatabase.importPackageFailed    -= ImportFailed;

            packages.Clear();
            requestComplete = null;
            GC.SuppressFinalize(this);
        }
    }

    public enum ImportResult 
    {
        Success,
        Cancelled,
        Failure
    }
}