using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VoodooPackages.Tool.VST
{
	public class ExporterEditor
	{
		private Texture2D              dragDropZoneTexture;
		private int                    packageToExportIndex;
		private bool                   displayPackageInfo;
		private bool                   displayPackageValidation;
		private static readonly string ExportDirectoryRelativePath = Path.Combine("Assets", "VSTExport");
		private ExporterHelpBoxArea    exporterHelpBoxArea;
		private Vector2                validationScrollView;
		private int                    toolbarInt;
		private string[]               toolbarStrings = {"Informations", "Dependencies"};
		private string                 multiPackageText;
        
		public void OnGUI()
		{
            GUI.enabled = GitHubBridge.isPushing == false;
            Show();
            GUI.enabled = true;
        }

        private void Show()
        {
	        GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
	        EditorGUILayout.LabelField("PACKAGE EXPORTER");
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            DropAreaGUI();
            
	        CalculateMultiPackageNameChoice();
            
            if (!string.IsNullOrEmpty(multiPackageText))
            {
	            EditorGUILayout.LabelField(multiPackageText);
            }
            else
            {
	            GUILayout.Space(3);
            }

            ShowPackageName();
            
	        EditorGUILayout.Space();
		    toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);
		    EditorGUILayout.Space();
		    
		    EditorGUI.BeginChangeCheck();
		    EditorGUILayout.BeginVertical("box");
		    if (toolbarInt == 0)
		    {
			    ExporterInfoEditor.ShowInfo();
		    }

		    if (toolbarInt == 1)
		    {
			    ExporterDependencyEditor.ShowDependency();
		    }
		    EditorGUILayout.EndVertical();
		    if (EditorGUI.EndChangeCheck())
		    {
			    ExporterValidationEditor.Validate();
		    }

		    ExporterValidationEditor.ShowValidation();
		    ShowExportButton();
        }

        private void DropAreaGUI()
        {
	        Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 100.0f, GUILayout.ExpandWidth(true));
            var TextStyle = new GUIStyle();
            TextStyle.normal.textColor = Color.white;

            if (Exporter.data.elementsToExport == null || Exporter.data.elementsToExport.Count == 0)
            {
	            dragDropZoneTexture = ContentHelper.UIdragDrop;
            }
            else if (dragDropZoneTexture == null)
            {
	            dragDropZoneTexture = ContentHelper.UIfolder;
            }

            GUI.Box(drop_area, dragDropZoneTexture);

            //Stop here if not (drag updated or drag perform) or mouse isn't in drop area
            if (!(evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)|| !drop_area.Contains(evt.mousePosition))
            {
	            return;
            }

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type != EventType.DragPerform) 
	            return;
            
            DragAndDrop.AcceptDrag();

            OnDragAccepted();
        }

        private void OnDragAccepted()
        {
	        dragDropZoneTexture = ContentHelper.UIfolder;
	        
	        SetElementsToExport(DragAndDrop.objectReferences);
            
            SetExportPackage();
            ExporterValidationEditor.Validate();
        }

        private void SetElementsToExport(Object[] objectReferences)
        {
	        Exporter.data.elementsToExport = new List<string>();

	        for (int i = 0; i < objectReferences.Length; i++)
	        {
		        Exporter.data.elementsToExport.Add(AssetDatabase.GetAssetPath(objectReferences[i]));
	        }

	        Exporter.data.package.name = objectReferences[0].name;
	        Exporter.data.onlinePackage = VoodooStore.packages.Find(x => x.name == Exporter.data.package.name);
        }

        private void SetExportPackage()
        {
	        ExporterData data = Exporter.data;
	        data.package.displayName = Regex.Replace(data.package.name, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");

	        Package onlinePackage = Exporter.data.onlinePackage ?? VoodooStore.packages.Find(x => x.name == Exporter.data.package.name);

	        if (onlinePackage == null)
	        {
		        Event.current.Use();
		        return;
	        }
	        
	        data.isNewPackage = false;
	        data.package.author = onlinePackage.author;
            data.selectedAuthor = Array.IndexOf(data.authors, data.package.author);
            if (data.selectedAuthor == -1)
	            data.selectedAuthor = 0;

            data.package.category = onlinePackage.category;
            data.categorySelected = Array.IndexOf(data.categories, data.package.category);

            Exporter.LoadSubCategories();

            data.package.subCategory = onlinePackage.subCategory;
            data.subCategorySelected = Array.IndexOf(data.subCategories, data.package.subCategory);

            if (data.subCategorySelected == -1)
            {
	            data.subCategorySelected = 0;
            }

            data.package.documentationLink = onlinePackage.documentationLink;
            data.package.dependencies = onlinePackage.dependencies;

            //Select all the dependencies including himself
            data.InitializeDependencies(onlinePackage);

            data.package.tags = onlinePackage.tags;
            data.package.description = onlinePackage.description;
            data.package.version = onlinePackage.version;

            Version _version = Version.Parse(data.package.version);
            _version = new Version(_version.Major, _version.Minor, _version.Build + 1);
            data.package.version = _version.ToString();
            
            packageToExportIndex = 0;
            Event.current.Use();
            
            CalculateMultiPackageNameChoice();
        }

        private void CalculateMultiPackageNameChoice()
        {
	        multiPackageText = string.Empty;

	        if (Exporter.data.elementsToExport == null || Exporter.data.elementsToExport?.Count <= 1)
		        return;
	        
	        multiPackageText = Exporter.data.elementsToExport[0];
		        
	        for (int i = 1; i < Exporter.data.elementsToExport.Count; i++)
	        {
		        multiPackageText += " + " + Exporter.data.elementsToExport[i];
	        }
        }

        private void ShowPackageName()
        {
	        if (string.IsNullOrEmpty(Exporter.data.package.name))
		        return;
	        
	        EditorGUILayout.BeginHorizontal();
	        {
		        EditorGUILayout.LabelField(Exporter.data.package.name, ContentHelper.StyleHeader);
		        if (Exporter.data.elementsToExport != null && Exporter.data.elementsToExport.Count > 1)
		        {
			        if (GUILayout.Button(">", GUILayout.Width(20)))
			        {
				        packageToExportIndex++;
				        if (packageToExportIndex > Exporter.data.elementsToExport.Count - 1)
				        {
					        packageToExportIndex = 0;
				        }
				        
				        Exporter.data.package.name = Path.GetFileName(Exporter.data.elementsToExport[packageToExportIndex]);
				        Exporter.data.onlinePackage = VoodooStore.packages.Find(x => x.name == Exporter.data.package.name);
			        }
		        }
	        }
	        EditorGUILayout.EndHorizontal();
        }
        
        private void ShowExportButton()
        {
	        EditorGUILayout.Space();
	        if (ExporterValidationEditor.isValid == false)
	        {
		        GUI.enabled = false;
	        }
	        
	        EditorGUILayout.LabelField("Patch note", EditorStyles.boldLabel);
	        Exporter.data.commitMessage = EditorGUILayout.TextArea(Exporter.data.commitMessage, GUILayout.MinHeight(54));

            if (GUILayout.Button("Export !", GUILayout.Height(40.0f)))
	        {
		        bool exportValidatedByUser = EditorUtility.DisplayDialog(Exporter.data.package.Name,
			        "You are going to push the package \"" + Exporter.data.package.Name + "\" to the VoodooStore. Are you sure ?", "Yes", "Cancel");

		        if(exportValidatedByUser)
		        {
			        Exporter.ExportPackage();
		        }
	        }
	        EditorGUILayout.Space();
	        
	        GUI.enabled = true;
        }
	}
}