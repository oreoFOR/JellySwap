using System;
using UnityEditor;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{
	public static class ExporterDependencyEditor
	{
		private static string dependencySearchString;
		private static Vector2 dependencyScrollPosition;
		
		public static void ShowDependency()
		{
			ShowDependencySearchBar();
			
	        dependencyScrollPosition = EditorGUILayout.BeginScrollView(dependencyScrollPosition, GUILayout.Height(160));
            for (int i = 0; i < Exporter.data.dependencyPackages.Count; i++)
            {
	            DependencyPackage dependencyPackage = Exporter.data.dependencyPackages[i];
	            
	            if (ShouldBeDisplayed(dependencyPackage) == false)
	            {
		            continue;
	            }

	            if (Exporter.data.unselectableDependencyPackages.Contains(dependencyPackage.packageName))
	            {
		            GUI.enabled = false;
	            }
	            
	            EditorGUILayout.BeginHorizontal();
	            {
		            EditorGUI.BeginChangeCheck();
		            bool value = EditorGUILayout.ToggleLeft(dependencyPackage.packageName, dependencyPackage.isSelected);

		            if (EditorGUI.EndChangeCheck())
		            {
			            OnPackageSelected(dependencyPackage, value);
		            }
	            }
	            EditorGUILayout.EndHorizontal();
	            
		        GUI.enabled = true;
            }

            GUILayout.EndScrollView();
        }

		private static bool ShouldBeDisplayed(DependencyPackage dependencyPackage)
		{
			string dependencyPackageName   = dependencyPackage.packageName;
			bool isVoodooStore             = dependencyPackageName == "VoodooStore";
			bool isItself                  = dependencyPackageName == Exporter.data.package.name;
			bool isOutsideOfSearchString   = string.IsNullOrEmpty(dependencySearchString) == false
			                                 && dependencyPackageName.IndexOf(dependencySearchString, StringComparison.OrdinalIgnoreCase) < 0;
	           
			return !isVoodooStore && !isItself && !isOutsideOfSearchString;
		}

		private static void OnPackageSelected(DependencyPackage dependencyPackage, bool toggleValue)
		{
			string dependencyPackageName = dependencyPackage.packageName;
			Package package = VoodooStore.packages.Find(x => x.name == dependencyPackageName);
			Exporter.data.AddPackageToDependencyList(package, toggleValue);
		}

		private static void ShowDependencySearchBar()
		{
			EditorGUILayout.BeginHorizontal();
	        
			dependencySearchString = GUILayout.TextField(dependencySearchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            
			if (GUILayout.Button(GUIContent.none, GUI.skin.FindStyle("ToolbarSeachCancelButton")))
			{
				dependencySearchString = "";
				GUI.FocusControl(null);
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}