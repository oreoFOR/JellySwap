using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{ 
	public static class Exporter
	{
		public static ExporterData data;

		public static void LoadSubCategories()
		{
			data.LoadSubCategories();
		}

        public static async void ExportPackage() 
        {
            string directoryPath = Path.Combine(PathHelper.DirectoryPath, "Packages", data.package.name);
            if (Directory.Exists(directoryPath) == false)
            {
                Directory.CreateDirectory(directoryPath);
            }

            string pkgPath = Path.Combine(directoryPath, data.package.name + PathHelper.unityPackage);
            
            // Package
            if (data.elementsToExport?.Count > 0)
            {
                AssetDatabase.ExportPackage(data.elementsToExport.ToArray(), pkgPath, ExportPackageOptions.Recurse);
            }

            // SetUp Size
            data.package.size = (int)new FileInfo(pkgPath).Length;
            
            await GitHubBridge.CommitAdd(data.package.name + "/Plugin.unitypackage", File.ReadAllBytes(pkgPath));

            //SetUp dependencies
            data.package.dependencies.dataList = new List<string>();
            foreach (DependencyPackage dependencyPackage in data.dependencyPackages)
            {
                if (dependencyPackage.isSelected)
                {
                    data.package.dependencies.dataList.Add(dependencyPackage.packageName);
                }
            }

            // Create Readme
            string text = JsonUtility.ToJson(data.package, true);
            await GitHubBridge.CommitAdd(data.package.name + "/readme.txt", text);

            GitHubBridge.Push(data.package.name + " v" + data.package.version + " - " + data.commitMessage);
        }
	}
}