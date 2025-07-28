using UnityEditor;
using UnityEngine;

namespace VoodooPackages.Tool.VST
{
    public class AuthentificationEditor : IEditor
    {
        private float fetchProgressNormalized;

        public void OnEnable()
        {
            GitHubBridge.fetchChanged += OnFetchProgressionChanged;

            fetchProgressNormalized = 0f;
        }

        public void OnDisable()
        {
            GitHubBridge.fetchChanged -= OnFetchProgressionChanged;
        }

        public void Controls()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                Event e = Event.current;
                if (VoodooStoreEditor.state == State.UNSIGNED && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter))
                {
                    TryLogin();
                }
            }
        }

        public void OnGUI()
        {
            if (VoodooStoreEditor.state != State.UNSIGNED &&
                VoodooStoreEditor.state != State.SIGNING_IN &&
                VoodooStoreEditor.state != State.FETCHING)
            {
                return;
            }

            ShowBanner();

            if (VoodooStoreEditor.state == State.UNSIGNED)
            {
                ShowLogin();
            }

            if (string.IsNullOrEmpty(GitHubBridge.errorMessage) == false)
            {
                EditorGUILayout.HelpBox(GitHubBridge.errorMessage, MessageType.Error);
                if (VoodooStoreEditor.state == State.FETCHING)
                {
                    if (GUILayout.Button(ContentHelper.UIrefresh, GUILayout.Height(40.0f)))
                    {
                        _ = GitHubBridge.FetchMaster();
                    }
                }
            }
            else if (VoodooStoreEditor.state == State.SIGNING_IN)
            {
                EditorGUILayout.HelpBox("Signing in, please wait...", MessageType.Info);
            }
            else if (VoodooStoreEditor.state == State.FETCHING)
            {
                EditorGUILayout.HelpBox("Fetching package, progress..." + fetchProgressNormalized * 100 + "%" , MessageType.Info);
            }
        }

        private void ShowBanner()
        {
            GUILayout.Space(10);
            GUILayout.Box(ContentHelper.UIBanner, ContentHelper.StyleBanner);
        }

        private void ShowLogin()
        {
            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("Token", GUILayout.Width(70));
                VoodooStore.signInToken = EditorGUILayout.PasswordField(VoodooStore.signInToken, GUILayout.Width(300));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Login", GUILayout.Height(40.0f), GUILayout.Width(370.0f)))
                {
                    TryLogin();
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(6f);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                
                GUILayout.Space(6f);
                if (GUILayout.Button(ContentHelper.UIQuestionMark, GUILayout.Height(40.0f), GUILayout.Width(40.0f)))
                {
                    UnityEngine.Application.OpenURL("https://voodoo.atlassian.net/wiki/spaces/VST/overview");
                }

                GUILayout.Space(6f);
                if (GUILayout.Button(ContentHelper.UIBug, GUILayout.Height(40.0f), GUILayout.Width(40.0f)))
                {
                    UnityEngine.Application.OpenURL("https://voodoo.atlassian.net/servicedesk/customer/portal/11");
                }

                GUILayout.Space(6f);
                if (GUILayout.Button(ContentHelper.UISlack, GUILayout.Height(40.0f), GUILayout.Width(40.0f)))
                {
                    UnityEngine.Application.OpenURL("https://app.slack.com/client/T07ELDMJ9/CD8PDM5EC");
                    //System.Diagnostics.Process.Start("slack.exe");
                }

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            //VoodooStore.autoSignIn = EditorGUILayout.Toggle("Remember me", VoodooStore.autoSignIn);
            //GUILayout.FlexibleSpace();
            //EditorGUILayout.EndHorizontal();
        }

        private void TryLogin()
        {
            // Security
            if (VoodooStoreEditor.state == State.SIGNING_IN || VoodooStore.signInToken == "")
            {
                return;
            }

            VoodooStoreEditor.state = State.SIGNING_IN;

            GitHubBridge.SetupClient();
            _ = GitHubBridge.TryGetMaster();
        }

        private void OnFetchProgressionChanged(float value)
        {
            fetchProgressNormalized = value;
        }
    }
}