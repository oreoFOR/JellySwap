using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{ 
    public static class VoodooStore
    {
        public static string                              signInToken;
        
        public static string                              masterSha;
        public static List<Package>                       packages  = new List<Package>();
        
        public static PackagePreset                       cart      = PackagePreset.cart;
        public static PackagePreset                       favorites = PackagePreset.favorites;
        public static List<PackagePreset>                 presets   = new List<PackagePreset>();
        
        public static ObservableCollection<IDownloadable> selection = new ObservableCollection<IDownloadable>();
        public static int                                 filters;

        public static Package GetPackageByPath(string path)
        {
            string[] splitPath = path.Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (splitPath.Length < 2)
            {
                return null;
            }

            Package package = GetPackageByName(splitPath[splitPath.Length - 2]);

            if (package == null)
            {
                package = new Package { name = splitPath[splitPath.Length - 2] };
                packages.Add(package);
            }

            return package;
        }

        public static Package GetPackageByName(string name)
        {
            for (int i = 0; i < packages.Count; i++)
            {
                if (packages[i].name == name)
                {
                    return packages[i];
                }
            }

            return null;
        }

        public static void RemoveDeletedPackages()
        {
            for (var i = 0; i < packages.Count; i++)
            {
                Package package = packages[i];
                if (!package.existRemotely)
                {
                    packages.Remove(package);
                }
            }
        }

        public static bool IsFilterActive(Filters filter) 
        {
            return (filters & (int)filter) == (int)filter;
        }
        
        public static bool ContainsPreset(IPackageComposition preset)
        {
            for (int i = 0; i < presets.Count; i++)
            {
                if (presets[i].Name == preset.Name && presets[i].Count == preset.Count && presets[i].All(preset.Contains))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanSaveAsPreset(PackagePreset preset) 
        {
            if (preset == null || preset.Count <= 0)
            {
                return false;
            }

            return ContainsPreset(preset) == false;
        }

        public static void SaveAsPreset(PackagePreset preset)
        {
            if (CanSaveAsPreset(preset) == false)
            {
                return;
            }

            preset.colorText = ColorUtility.ToHtmlStringRGB(preset.Color);
            presets.Add(preset);
        }

        public static void Dispose()
        {
            signInToken      = "";
            masterSha        = "";
            packages         = new List<Package>();
            cart             = PackagePreset.cart;
            favorites        = PackagePreset.favorites;
            selection        = new ObservableCollection<IDownloadable>();
            presets          = new List<PackagePreset>();
            filters          = 0;
        }
    }
}