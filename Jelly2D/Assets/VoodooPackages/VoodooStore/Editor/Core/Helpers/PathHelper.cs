using System;
using System.Collections.Generic;
using System.IO;

namespace VoodooPackages.Tool.VST
{
    public static class PathHelper
    {
        //URL
        public const string urlUser                       = "https://api.github.com/user";
        public const string baseUrl                       = "https://api.github.com/repos/VoodooTeam/";

        public const string urlVS                         = baseUrl + "VoodooStore/";
        public const string urlVSTest                     = baseUrl + "VoodooStoreTest/";

        public const string urlContent                    = urlVSTest + "contents/{0}";
        public const string urlBlobs                      = urlVSTest + "git/blobs/{0}";

        public const string unityPackage                  = ".unitypackage";
        public const string readme                        = "readme.txt";

        public const string vstPortal                     = "https://voodoo.atlassian.net/wiki/spaces/VST/overview";
        public const string serviceDeskSupport            = "https://voodoo.atlassian.net/servicedesk/customer/portal/11";
        
        public static readonly string urlMaster           = urlVSTest + "branches/master";
        public static readonly string UserFolderLocation  = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static readonly string DirectoryPath       = Path.Combine(UserFolderLocation, "Voodoo");
     
        public static readonly List<string> IgnoredFiles  = new List<string>()
        {
            "README.md",
            ".DS_Store",
            ".gitignore"
        };


        public static string GetContentURL(string packageName)
        {
            return string.Format(urlContent, packageName);
        }

        public static string GetPackageContentURL(string name)
        {
            return string.Format(urlContent + "/Plugin.unitypackage", name);
        }

        public static string GetPackageBlobURL(string sha)
        {
            return string.Format(urlBlobs, sha);
        }
    }
}