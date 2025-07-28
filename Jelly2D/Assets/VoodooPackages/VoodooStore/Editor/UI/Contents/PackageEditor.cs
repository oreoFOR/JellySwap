using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{
    public class PackageEditor : AbstractGenericEditor<List<Package>>
    {
        private static readonly int ContentId = "Content".GetHashCode();

        public override void OnGUI(List<Package> packages)
        {
            if (VoodooStore.selection.Count > 1)
            {
                PackagePreset selection = new PackagePreset {Name = "Selection"};
                foreach (IDownloadable downloadable in VoodooStore.selection)
                {
                    selection.Add(downloadable as Package);
                }

                EditorRetailer.OnGUI(ContentId, selection);
            }
            else
            {
                Package package = packages[0];
                EditorGUILayout.BeginHorizontal();
                {
                    Rect labelRect = EditorGUILayout.GetControlRect(false, ContentHelper.StyleTitle.lineHeight, ContentHelper.StyleTitle);
                    string newName = ContentHelper.GetEllipsisString(package.displayName, ContentHelper.StyleTitle, labelRect);
                    EditorGUI.LabelField(labelRect, newName, ContentHelper.StyleTitle);

                    EditorGUIHelper.ShowPackagesButtons(package);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(ContentHelper.StyleTitle.fontSize);

                // Dependency
                if (package.dependencies?.dataList?.Count > 0)
                {
                    if (Event.current.isKey)
                    {
                        return;
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Box(ContentHelper.UIDependency, GUIStyle.none, GUILayout.Width(30), GUILayout.Height(30));
                        EditorGUILayout.LabelField(package.dependencies.dataList[0], ContentHelper.StyleSubTitle);
                    }
                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginVertical();
                    for (int i = 1; i < package.dependencies.dataList.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Box(GUIContent.none, GUIStyle.none, GUILayout.Width(30), GUILayout.Height(30));
                            EditorGUILayout.LabelField(package.dependencies.dataList[i], ContentHelper.StyleSubTitle);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                {
                    if (Event.current.isKey)
                        return;

                    EditorGUILayout.LabelField(package.name, ContentHelper.StyleSubTitle);
                    GUILayout.FlexibleSpace();
                    GUI.contentColor = ContentHelper.VSTBlue;
                    if (GUILayout.Button(ContentHelper.UIQuestionMark, GUILayout.Height(25), GUILayout.Width(25)))
                    {
                        Application.OpenURL(package.documentationLink);
                    }

                    GUI.contentColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("v" + package.version, ContentHelper.StyleNormal);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField(package.unityVersion, ContentHelper.StyleSubTitle);
                    GUILayout.Space(10);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(10);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Author: " + package.author, ContentHelper.StyleNormal);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("Size: " + package.size.ToOctetsSize(), ContentHelper.StyleSubTitle);
                    GUILayout.Space(10);
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(20);

                EditorGUILayout.LabelField("Description", ContentHelper.StyleNormal);
                GUILayout.Space(5);

                EditorGUILayout.LabelField(package.description, ContentHelper.StyleNormal);

                GUILayout.Space(ContentHelper.StyleNormal.fontSize);
            }
        }
    }
}