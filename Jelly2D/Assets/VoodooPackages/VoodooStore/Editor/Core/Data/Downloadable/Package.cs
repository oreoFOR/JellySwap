using System;
using System.Collections.Generic;

namespace VoodooPackages.Tool.VST
{
    [Serializable]
    public class Package : IDownloadable
    {
        public string   name;
        public string   author;
        public string   displayName;
        public string   description;
        public string   version;
        public string   unityVersion;
        public string   category;
        public string   subCategory;
        public int      size;
        [NonSerialized]
        public string   localSha;
        public string   serverSha;
        public string   pluginSha;

        [NonSerialized]
        public bool existRemotely;
        
        public ListContainer dependencies;

        public ListContainer tags;

        public int index;
        public int showIndex;

        public string documentationLink;

        public string Name => name;

        public int VersionStatus => string.IsNullOrEmpty(localSha) ? VersionState.NotPresent : 
                    localSha == serverSha ? VersionState.UpToDate : 
                    localSha == "manually" ? VersionState.Manually : VersionState.OutDated;
        public bool isInstalled  => VersionStatus == VersionState.UpToDate || VersionStatus == VersionState.OutDated;

        public List<Package> Content => new List<Package> { this };

        public void FillDependencies(ref PackageComposition composition)
        {
            if (dependencies == null || composition.Contains(this))
            {
                return;
            }

            composition.Add(this);

            for (int i = 0; i < dependencies.dataList.Count; i++)
            {
                Package package = VoodooStore.GetPackageByName(dependencies.dataList[i]);

                if (package == null)
                {
                    continue;
                }
    
                package.FillDependencies(ref composition);
            }
        }

        public void FillUses(ref PackageComposition composition)
        {
            if (dependencies == null || composition.Contains(this))
            {
                return;
            }

            composition.Add(this);

            for (int i = 0; i < VoodooStore.packages.Count; i++)
            {
                if (VoodooStore.packages[i].dependencies.dataList.Contains(name) == false)
                {
                    continue;
                }

                VoodooStore.packages[i].FillUses(ref composition);
            }
        }

        public override string ToString()
        {
            return string.Format("name : {0}, author : {1}, displayName : {2}, description : {3}," +
                " version : {4}, unityVersion : {5}, category : {6}, subCategory : {7}," +
                " size : {8}, localSha : {9}, serverSha : {10}, dependencies : {11}",
                name, author, displayName, description, version, unityVersion, category, subCategory, size, localSha, serverSha, dependencies);
        }
    }
}