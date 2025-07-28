using System;
using System.Collections.Generic;

namespace VoodooPackages.Tool.VST
{

    [Serializable]
    public class Branche
    {
        public string name;
        public Commit commit;

        public string treeUrl => commit.commit.tree.url;
    }

    [Serializable]
    public class Commit 
    {
        public string sha;
        public CommitContent commit; 
    }

    [Serializable]
    public class CommitContent
    {
        public Tree tree;
    }

    [Serializable]
    public class Tree
    {
        public string sha;
        public string url;
    }

    public class Repository
    {
        public string           sha;
        public string           url;
        public List<GitHubFile> tree = new List<GitHubFile>();
        public bool             truncated;
    }

    [Serializable]
    public class GitHubFile
    {
        public string   name;
        public string   path;
        public string   sha;
        public int      size;
        public string   url;
        public string   html_url;
        public string   git_url;
        public string   download_url;
        public string   type;
        public string   content;
        public string   encoding;

        /// DA FUCK IS THIS USE FOR
        public override string ToString()
        {
            return string.Format("name : {0}, path : {1}, sha : {2}, size : {3}," +
                " url : {4}, html_url : {5}, git_url : {6}, download_url : {7}," +
                " type : {8}, content : {9}, encoding : {10}",
                name, path, sha, size, url, html_url, git_url, download_url, type, content, encoding);
        }
    }
}