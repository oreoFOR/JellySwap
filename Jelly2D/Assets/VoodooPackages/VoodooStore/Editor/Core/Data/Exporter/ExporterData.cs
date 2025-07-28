using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{
	[Serializable]
	public class ExporterData
	{
		public bool                     isNewPackage;
		public Package                  onlinePackage;
	    public Package                  package;
	    public List<string>             elementsToExport;
	    public List<DependencyPackage>  dependencyPackages;
	    public List<string>             unselectableDependencyPackages;
	    public string                   commitMessage;

        //Categories & SubCategories
        public string[]                 categories;
		public string[]                 subCategories;
		public int                      categorySelected;
		public int                      subCategorySelected;
		public string                   newSubCategory;
		public string                   newSubCategoryText = "New Sub Category";
		
		//Author
		public string[]                 authors;
		public int                      selectedAuthor;
		public string                   newAuthor;

		public ExporterData()
		{
			InitExporterPackage();
		}
		
		private void InitExporterPackage()
		{
			// Suppose that is a new package
			isNewPackage = true;

			package = new Package();
			package.version = "1.0.0";
			package.unityVersion = Application.unityVersion;

			categories = Enum.GetNames(typeof(Categories));
			string[] tempCategories = new string[categories.Length-1];
			for (var i = 1; i < categories.Length; i++)
			{
				tempCategories[i-1] = categories[i];
			}
			categories = tempCategories;

			categorySelected = 0;

			newSubCategory = "";

			LoadSubCategories();

			subCategorySelected = 0;

			newAuthor = "";

			List<string> _authors = new List<string>();

			foreach (Package _package in VoodooStore.packages)
			{
				if (!_authors.Contains(_package.author))
				{
					_authors.Add(_package.author);
				}
			}

			_authors.Add("New Author");
			authors = _authors.ToArray();

			selectedAuthor = 0;

			// Reset Object
			elementsToExport = new List<string>();

			// Dependency
			package.dependencies = new ListContainer {dataList = new List<string>()};
			dependencyPackages = new List<DependencyPackage>();

			for (int i = 0; i < VoodooStore.packages.Count; i++)
			{
				DependencyPackage dependencyPackage = new DependencyPackage
				{
					packageName = VoodooStore.packages[i].name,
					isSelected = false,
				};
				dependencyPackages.Add(dependencyPackage);
			}
			
			unselectableDependencyPackages = new List<string>();
		}
		
		public void LoadSubCategories()
		{
			List<string> _subCategories = new List<string>();
			string curCategorySelected = categories[categorySelected];

			for (var i = 0; i < VoodooStore.packages.Count; i++)
			{
				Package _package = VoodooStore.packages[i];
				if (curCategorySelected == _package.category)
				{
					if (!_subCategories.Contains(_package.subCategory))
					{
						_subCategories.Add(_package.subCategory);
					}
				}
			}

			_subCategories.Add(newSubCategoryText);
			subCategories = _subCategories.ToArray();
		}
		
		public void InitializeDependencies(Package package)
		{
			foreach (DependencyPackage dependencyPackage in Exporter.data.dependencyPackages)
			{
				dependencyPackage.isSelected = false;
			}
			
			unselectableDependencyPackages = new List<string>();
			
			if (package.dependencies?.dataList == null)
				return;

			for (int i = 0; i < package.dependencies.dataList.Count; i++)
			{
				Package childPackage = VoodooStore.packages.Find(x => x.name == package.dependencies.dataList[i]);
				
				if (childPackage == null)
					continue;

				AddPackageToDependencyList(childPackage);
			}
		}

		public void AddPackageToDependencyList(Package package, bool add = true)
		{
			DependencyPackage dependencyPackage = Exporter.data.dependencyPackages.Find(x => x.packageName == package.name);
			dependencyPackage.isSelected = add;

			SetDependencyRecursive(package, add);
			dependencyPackages = dependencyPackages.OrderByDescending(x => x.isSelected).ThenBy(x => x.packageName).ToList();
		}

		private void SetDependencyRecursive(Package package, bool add)
		{
			if (package.dependencies?.dataList == null)
				return;
			
			for (int i = 0; i < package.dependencies.dataList.Count; i++)
			{
				Package childPackage = VoodooStore.packages.Find(x => x.name == package.dependencies.dataList[i]);
				
				if (childPackage == null)
					continue;
				
				if (add)
				{
					unselectableDependencyPackages.Add(childPackage.name);
					DependencyPackage dependencyPackage = Exporter.data.dependencyPackages.Find(x => x.packageName == childPackage.name);
					if (dependencyPackage.isSelected)
					{
						dependencyPackage.isSelected = false;
					}
				}
				else
				{
					unselectableDependencyPackages.Remove(childPackage.name);
				}
				
				SetDependencyRecursive(childPackage, add);
			}
		}
	}
}
