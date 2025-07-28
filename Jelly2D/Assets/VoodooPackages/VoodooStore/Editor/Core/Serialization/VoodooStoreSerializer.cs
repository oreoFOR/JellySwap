using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{
    public static class VoodooStoreSerializer 
    {
        public static void Read()
        {
            ReadStoreData();
            ReadProjectData();
        }

        public static void ReadStoreData() 
        {
            //TODO offer the possibility to redefine the path were info are save
            string infoPath = Path.Combine(PathHelper.DirectoryPath, "info.json");

            if (File.Exists(infoPath) == false)
            {
                return;
            }

            string text;
            using (StreamReader reader = File.OpenText(infoPath))
            {
                text = reader.ReadToEnd();
                reader.Close();
            }

            VoodooStoreData data = JsonUtility.FromJson<VoodooStoreData>(text);
            if (data != null)
            {
                StoreDataToStore(data);
            }
        }

        private static void StoreDataToStore(VoodooStoreData data)
        {
            VoodooStore.signInToken = data.signInToken;
            VoodooStore.masterSha   = data.masterSha;
            VoodooStore.packages    = data.packages;
            VoodooStore.presets     = new List<PackagePreset>();
            foreach (PackagePreset packagePreset in data.presets)
            {
                GetRealPackages(packagePreset, out PackagePreset newPreset);
                VoodooStore.presets.Add(newPreset);
            }
            
            GetRealPackages(data.cart, out VoodooStore.cart);
            GetRealPackages(data.favorites, out VoodooStore.favorites);
        }

        private static void GetRealPackages<T>(T from, out T to) where T : PackagePreset
        {
            to = from;
            
            List<Package> tempPackages = new List<Package>(from);
            to.Clear();
            foreach (Package package in tempPackages)
            {
                Package vstPackage = VoodooStore.packages.Find(x => x.Name == package.Name);
                if (vstPackage != null)
                {
                    to.Add(vstPackage);
                }
            }
        }

        public static void ReadProjectData()
        {
            //TODO offer the possibility to redefine the path were info are save
            string infoPath           = Path.Combine(PathHelper.DirectoryPath, PlayerSettings.productName + "Info.json");
            if (File.Exists(infoPath) == false)
            {
                return;
            }

            string text;
            using (StreamReader reader = File.OpenText(infoPath))
            {
                text = reader.ReadToEnd();
                reader.Close();
            }

            ProjectData data = JsonUtility.FromJson<ProjectData>(text);
            if (data != null)
            {
                ProjectDataToStore(data);
            }
        }

        private static void ProjectDataToStore(ProjectData data)
        {
            for (int i = 0; i < data.packagesNames.Count; i++)
            {
                Package pkg =  VoodooStore.GetPackageByName(data.packagesNames[i]);
                if (pkg == null)
                {
                    continue;
                }

                pkg.localSha = data.packagesSha[i];
            }
        }

        public static void Write()
        {
            WriteStoreData();
            WriteProjectData();
        }

        public static void WriteStoreData()
        {
            VoodooStoreData data = StoreToStoreData();

            string text     = JsonUtility.ToJson(data, true);
            string infoPath = Path.Combine(PathHelper.DirectoryPath, "info.json");

            if (Directory.Exists(PathHelper.DirectoryPath) == false)
            {
                Directory.CreateDirectory(PathHelper.DirectoryPath);
            }

            File.WriteAllText(infoPath, text);
        }

        private static VoodooStoreData StoreToStoreData() 
        {
            return new VoodooStoreData
            {
                signInToken = VoodooStore.signInToken,
                masterSha   = VoodooStore.masterSha,
                packages    = VoodooStore.packages,
                cart        = VoodooStore.cart,
                favorites   = VoodooStore.favorites,
                presets     = new List<PackagePreset>(VoodooStore.presets),
            };
        }

        public static void WriteProjectData()
        {
            ProjectData data = StoreToProjectData();

            string text      = JsonUtility.ToJson(data, true);
            string infoPath  = Path.Combine(PathHelper.DirectoryPath, PlayerSettings.productName + "Info.json");

            if (Directory.Exists(PathHelper.DirectoryPath) == false)
            {
                Directory.CreateDirectory(PathHelper.DirectoryPath);
            }

            File.WriteAllText(infoPath, text);
        }

        private static ProjectData StoreToProjectData()
        {
            ProjectData data = new ProjectData();
            foreach (Package pkg in VoodooStore.packages)
            {
                if (pkg == null || string.IsNullOrEmpty(pkg.localSha))
                {
                    continue;
                }
                
                data.packagesNames.Add(pkg.name);
                data.packagesSha.Add(pkg.localSha);
            }

            return data;
        }
    }
}