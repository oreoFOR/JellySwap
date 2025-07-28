using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{
    public class VoodooStoreEditor : EditorWindow
    {
        private static readonly int ContentId  = "Content".GetHashCode();

        public static State     state = State.UNSIGNED;

        private Vector2         listScrollPosition;
        
        private static VoodooStoreEditor window;
        private static ExporterEditor exporterEditor;

        private FiltersEditor   filterEditor;
        private IEditor[]       editors;

        private DownloadableTreeView<PackageItem> downloadablePackageTreeView;
        private DownloadableTreeView<PackagePresetItem> downloadableCompositionTreeView;

        [MenuItem("VoodooPackages/Voodoo Store %#v", false, -50)]
        static void Init()
        {
            if (window != null)
            {
                window.Close();
            }

            window = GetWindow<VoodooStoreEditor>(false, "Voodoo Store");
            window.Show();
        }

        private void OnEnable()
        {
            AddFactories();
            ParseInfo();

            filterEditor  =  new FiltersEditor();
            filterEditor.OnEnable();

            editors = new IEditor[]
            {
                new AuthentificationEditor()
            };
            
            downloadablePackageTreeView = new DownloadableTreeView<PackageItem>(new PackageTreeView(VoodooStore.packages));
            
            downloadableCompositionTreeView = new DownloadableTreeView<PackagePresetItem>(new PackagePresetTreeView(new List<PackagePreset>()));
            downloadableCompositionTreeView.viewContent.Add(VoodooStore.favorites);
            downloadableCompositionTreeView.viewContent.Add(VoodooStore.cart);
            foreach (PackagePreset packagePreset in VoodooStore.presets)
            {
                downloadableCompositionTreeView.viewContent.Add(packagePreset);
            }
            
            for (int i = 0; i < editors.Length; i++)
            {
                editors[i].OnEnable();
            }

            if (window == null)
            {
                window = GetWindow<VoodooStoreEditor>("Voodoo Store", false);
            }

            int lastState = PlayerPrefs.GetInt("editorState", -1);
            if (lastState >= 0)
            {
                state = (State)lastState;
            }

            GitHubBridge.OnEnable();
            GitHubBridge.fetchChanged += OnFetchChanged;
            GitHubBridge.fetchCompleted += OnFetchCompleted;
        }

        private void OnDisable()
        {
            PlayerPrefs.SetInt("editorState", (int)state);

            WriteInfo();

            filterEditor.OnDisable();
            
            for (int i = 0; i < editors.Length; i++)
            {
                editors[i].OnDisable();
            }

            editors = null;
            EditorRetailer.Clear();

            VoodooStore.Dispose();
            GitHubBridge.Dispose();
        }

        private void OnDestroy()
        {
            state = State.UNSIGNED;
            PlayerPrefs.SetInt("editorState", -1);
        }

        private void ParseInfo()
        {
            VoodooStoreSerializer.Read();
            ExporterSerializer.Read();
            ExporterValidationEditor.Validate();
        }

        private void WriteInfo()
        {
            VoodooStoreSerializer.Write();
            ExporterSerializer.Write();
        }

        private void AddFactories() 
        {
            BaseFactory contentFactory = new BaseFactory
            {
                context = ContentId,
                factors = new List<IEditorTarget> 
                {
                    new StoreContentEditor(),
                    // new PackageCompositionEditor(),
                    new PackageEditor(),
                    new PresetEditor()
                }
            };

            EditorRetailer.AddFactory(contentFactory);
        }

        private void OnFetchChanged(float obj)
        {
            Repaint();
        }

        private void OnFetchCompleted()
        {
            Repaint();
        }

        private void OnGUI()
        {
            for (int i = 0; i < editors?.Length; i++)
            {
                editors[i].OnGUI();
            }

            if (state == State.FETCH_OK)
            {
                ShowStore();
            }
			else if (state == State.DOWNLOADING)
			{
				EditorGUILayout.HelpBox("Downloading package, please wait...", MessageType.Info);
			}
			else if (state == State.DOWNLOAD_ERROR)
			{
                _ = GitHubBridge.FetchMaster();
            }
			else if (state == State.EXPORTER)
			{
				if (exporterEditor == null)
				{
					exporterEditor = new ExporterEditor();
					ExporterValidationEditor.Validate();
				}
				
				ShowButtonsBar();
				
				
				exporterEditor?.OnGUI();
			}
		}

        private void ShowButtonsBar()
        {
        	EditorGUILayout.Space();
        	
            if (state == State.FETCH_OK)
            {
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(ContentHelper.UIplus, GUILayout.Height(25), GUILayout.Width(25)))
                    {
                        Exporter.data = new ExporterData();
                        state = State.EXPORTER;
                    }

                    if (GUILayout.Button(ContentHelper.UIrefresh, GUILayout.Height(25), GUILayout.Width(25)))
                    {
                        _ =GitHubBridge.FetchMaster();
                    }

                    GUILayout.FlexibleSpace();


                    GUI.contentColor = ContentHelper.VSTBlue;
                    if (GUILayout.Button(ContentHelper.UIQuestionMark, GUILayout.Height(25), GUILayout.Width(25)))
                    {
                        Application.OpenURL(PathHelper.vstPortal);
                    }
                    GUI.contentColor = Color.white;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

            }
            else if (state == State.DOWNLOAD_ERROR)
            {
                if (GUILayout.Button(ContentHelper.UIreturn, GUILayout.Height(25), GUILayout.Width(25)))
                {
                    state = State.FETCH_OK;
					GUI.FocusControl(null);
				}

                GUILayout.Space(10);
            }
            else if (state == State.EXPORTER)
            {
	            GUILayout.BeginHorizontal();

                if (GUILayout.Button(ContentHelper.UIreturn, GUILayout.Height(25), GUILayout.Width(25)))
                {
					state = State.FETCH_OK;
					exporterEditor = null;
					ExporterValidationEditor.ClearValidation();
					GUI.FocusControl(null);
				}

                GUILayout.FlexibleSpace();
                GUI.contentColor = ContentHelper.VSTBlue;
                if (GUILayout.Button(ContentHelper.UIQuestionMark, GUILayout.Height(25), GUILayout.Width(25)))
                {
                    Application.OpenURL(PathHelper.vstPortal);
                }
                GUI.contentColor = Color.white;
                
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }
        }


        #region ShowStore
       
        private void ShowStore()
        {
            ShowButtonsBar();

            filterEditor.OnGUI();

            EditorGUILayout.BeginHorizontal();
            { 
                ShowScrollableSelection();
                GUI.backgroundColor = Color.white;
                ShowContent();
            }

            EditorGUILayout.EndHorizontal();
		}

        private void ShowScrollableSelection() 
        {
            GUILayout.Space(10);
            
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            {
                listScrollPosition = GUILayout.BeginScrollView(listScrollPosition, false, false, GUILayout.Height(window.position.height - 110));
                {
                    ShowPresetsList();
            
                    EditorGUILayout.Space();

                    ShowPackageList();
                }
                GUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void ShowPresetsList()
        {
            DrawCompositionTreeView();
        }

        private void ShowPackageList()
        {
            DrawPackageTreeView();
        }

        private void DrawCompositionTreeView()
        {
            int compositionNumber = 2 + VoodooStore.presets.Count;
            if (compositionNumber != downloadableCompositionTreeView.viewContent.GetData().Count)
            {
                List<IDownloadable> compositions = new List<IDownloadable> {VoodooStore.favorites, VoodooStore.cart};
                compositions.AddRange(VoodooStore.presets);
                downloadableCompositionTreeView.viewContent.UpdateContent(compositions);
            }
            
            downloadableCompositionTreeView?.Refresh();
			
            Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MaxHeight(141));
            downloadableCompositionTreeView?.OnGUI(controlRect);
        }

        private void DrawPackageTreeView()
        {
            downloadablePackageTreeView?.Refresh();
			
            Rect controlRect = EditorGUILayout.GetControlRect(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            downloadablePackageTreeView?.OnGUI(controlRect);
        }

        private void ShowContent()
        {
            EditorGUILayout.BeginVertical();
            IList<IDownloadable> target = VoodooStore.selection;
            if (target == null || target.Count == 0)
            {
                EditorRetailer.OnGUI(ContentId, this);
            }
            else
            {
                if (target[0] is PackagePreset preset)
                {
                    EditorRetailer.OnGUI(ContentId, preset);
                }
                else if (target[0] is Package)
                {
                    List<Package> packages = target.Cast<Package>().ToList();
                    EditorRetailer.OnGUI(ContentId, packages);
                }
            }
            EditorGUILayout.EndVertical();
        }

        #endregion
    }

    public class Category
    {
        public string name;
        public List<SubCategory> subCategories = new List<SubCategory>();

        public Category(string _name)
        {
            name = _name;
        }
        
        public void AddSubCategory(string _subName)
        {
            for (int i = 0; i < subCategories.Count; i++)
            {
                if (String.Equals(subCategories[i].name, _subName))
                {
                    return;
                }
            }
            
            SubCategory _subCategory = new SubCategory(_subName, this);
            subCategories.Add(_subCategory);
            
        }

        public bool IsEmpty()
        {
            return subCategories.Count == 0;
        }

        public int Contain(string _subName)
        {
            for (int i = 0; i < subCategories.Count; i++)
            {
                if (subCategories[i].name == _subName)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public class SubCategory
    {
        public string name;
        public Category category;

        public SubCategory(string _name, Category _category)
        {
            name = _name;
            category = _category;
        }
    }
}
