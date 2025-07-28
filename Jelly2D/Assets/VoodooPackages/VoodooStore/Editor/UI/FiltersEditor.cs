using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;

namespace VoodooPackages.Tool.VST
{
    public class FiltersEditor : IEditor
    {
        // SearchBar
        public static List<Package> filteredPackages = new List<Package>();
        private string              searchString     = string.Empty;

        private List<Category>      categories       = new List<Category>();
        private string[]            categoriesNames;
        private string[]            subCategoriesNames;
        private int                 categoryIndex;
        private int                 subCategoryIndex;

        public static event Action<List<Package>> onApplyFilter;

        public void OnEnable()
        {
            GitHubBridge.fetchCompleted += OnFetchCompleted;
            OnFetchCompleted();
            onApplyFilter = null;
        }

        public void OnDisable()
        {
            GitHubBridge.fetchCompleted -= OnFetchCompleted;
            onApplyFilter = null;
        }

        public void Controls()
        {
        }

        private void OnFetchCompleted()
        {
            RefreshTabs();
            ApplyFilters();
        }

        void RefreshTabs()
        {
            categories.Clear();

            foreach (Categories categories in (Categories[])Enum.GetValues(typeof(Categories)))
            {
                Category newCategory = new Category(categories.ToString());
                this.categories.Add(newCategory);
                newCategory.AddSubCategory("All");
            }
            
            // Add SubCategory
            for (int i = 0; i < VoodooStore.packages.Count; i++)
            {
                for (int j = 1; j < categories.Count; j++)
                {
                    if (String.Equals(categories[j].name, VoodooStore.packages[i].category))
                    {
                        categories[j].AddSubCategory(VoodooStore.packages[i].subCategory);
                        categories[0].AddSubCategory(VoodooStore.packages[i].subCategory);
                    }
                }
            }
            
            FillCategoryList();
        }
        
        private void FillCategoryList()
        {
            List<string> cats = new List<string>();
            cats.AddRange(categories.Select(c => c.name));
            categoriesNames = cats.ToArray();

            List<string> subs = new List<string>();
            IEnumerable<string> subCategory = categories[categoryIndex].subCategories.Select(s => s.name);
            subs.AddRange(subCategory);

            subCategoriesNames = subs.ToArray();
        }

        private void ApplyFilters()
        {
            filteredPackages.Clear();

            if (categoryIndex < 0 && subCategoryIndex < 0 && VoodooStore.filters == 0)
            {
                filteredPackages.AddRange(VoodooStore.packages);
                return;
            }

            for (int i = 0; i < VoodooStore.packages.Count; i++)
            {
                if (IsPackageMatchingFilters(VoodooStore.packages[i]) == false)
                {
                    continue;
                }

                filteredPackages.Add(VoodooStore.packages[i]);
            }

            if (VoodooStore.selection?.Count > 0 && VoodooStore.selection[0] is Package)
            {
                if (filteredPackages.Count == 0)
                {
                    VoodooStore.selection.Clear();
                }
                else
                {
                    List<Package> currentFilteredPackages = new List<Package>();

                    foreach (IDownloadable downloadable in VoodooStore.selection)
                    {
                        if (downloadable is Package pkg && filteredPackages.Contains(pkg))
                        {
                            currentFilteredPackages.Add(pkg);
                        }
                    }

                    VoodooStore.selection.Clear();
                    foreach (Package currentFilteredPackage in currentFilteredPackages)
                    {
                        VoodooStore.selection.Add(currentFilteredPackage);
                    }
                }
            }

            onApplyFilter?.Invoke(filteredPackages);
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            {
                ShowFilters();
                ShowSearchBar();
            }
            GUILayout.EndHorizontal();

            ShowTabs();
        }

        private void ShowFilters()
        {
            EditorGUI.BeginChangeCheck();

            if (VoodooStore.IsFilterActive(Filters.NotInstalled))
            {
                if (GUILayout.Button(ContentHelper.UIvalidate, GUI.skin.FindStyle("toolbarButton"), GUILayout.Width(30)))
                {
                    VoodooStore.filters &= ~(int)Filters.NotInstalled;
                    VoodooStore.filters |= (int)Filters.Installed;
                }
            }
            if (VoodooStore.IsFilterActive(Filters.Installed))
            {
                GUI.contentColor = ContentHelper.VSTGreen;
                if (GUILayout.Button(ContentHelper.UIvalidate, GUI.skin.FindStyle("toolbarButton"), GUILayout.Width(30)))
                {
                    VoodooStore.filters &= ~(int)Filters.Installed;
                    VoodooStore.filters |= (int)Filters.Updatable;
                }
            }
            else if (VoodooStore.IsFilterActive(Filters.Updatable))
            {
                GUI.contentColor = ContentHelper.VSTOrange;
                if (GUILayout.Button(ContentHelper.UIrefresh, GUI.skin.FindStyle("toolbarButton"), GUILayout.Width(30)))
                {
                    VoodooStore.filters &= ~(int)Filters.Updatable;
                }
            }
            else
            {
                GUI.contentColor = Color.white;
                if (GUILayout.Button(ContentHelper.UIdownload, GUI.skin.FindStyle("toolbarButton"), GUILayout.Width(30)))
                {
                    VoodooStore.filters |= (int)Filters.Installed;
                }
            }

            GUI.contentColor = Color.white;

            // Change
            if (EditorGUI.EndChangeCheck())
            {
                ApplyFilters();
            }

            GUILayout.FlexibleSpace();
        }

        private void ShowSearchBar()
        {
            EditorGUI.BeginChangeCheck();
            {
                searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"), GUILayout.Width(200));
            }
            if (EditorGUI.EndChangeCheck())
            {
                ApplyFilters();
            }

            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                searchString = "";

                ApplyFilters();

                GUI.FocusControl(null);
            }

            GUILayout.FlexibleSpace();
        }

        private void ShowTabs()
        {
            EditorGUI.BeginChangeCheck();
            {
                categoryIndex = GUILayout.Toolbar(categoryIndex, categoriesNames);
            }
            if (EditorGUI.EndChangeCheck())
            {
                subCategoryIndex = 0;
                FillCategoryList();
                ApplyFilters();
            }
            
            EditorGUI.BeginChangeCheck();
            {
                subCategoryIndex = GUILayout.Toolbar(subCategoryIndex, subCategoriesNames);
            }
            if (EditorGUI.EndChangeCheck())
            {
                FillCategoryList();
                ApplyFilters();
            }
        }

        private bool IsPackageMatchingFilters(Package pkg)
        {
            if (categoryIndex > 0 && pkg.category != categories[categoryIndex].name)
            {
                return false;
            }

            if (subCategoryIndex > 0 && pkg.subCategory != categories[categoryIndex].subCategories[subCategoryIndex].name)
            {
                return false;
            }

            if (string.IsNullOrEmpty(searchString) == false && pkg.name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) < 0)
            {
                return false;
            }

            if (VoodooStore.IsFilterActive(Filters.Installed) && pkg.VersionStatus != VersionState.UpToDate)
            {
                return false;
            }

            if (VoodooStore.IsFilterActive(Filters.Updatable) && pkg.VersionStatus != VersionState.OutDated)
            {
                return false;
            }
            
            if (VoodooStore.IsFilterActive(Filters.NotInstalled) && pkg.VersionStatus != VersionState.NotPresent)
            {
                return false;
            }

            return true;
        }
    }
}