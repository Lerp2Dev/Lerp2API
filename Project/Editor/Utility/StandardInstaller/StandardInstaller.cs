using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace Lerp2API.Utility.StandardInstaller
{
    /// <summary>
    /// Class StandardInstaller.
    /// </summary>
    /// <seealso cref="UnityEditor.EditorWindow" />
    public class StandardInstaller : EditorWindow
    {
        /// <summary>
        /// Me
        /// </summary>
        public static StandardInstaller me;

        public static string EditorKey
        {
            get
            { //This checks if the current session has already create a package to export.
                return string.Concat(PlayerSettings.companyName, ".", PlayerSettings.productName, ".StandardInstaller.DontInstall");
            }
        }

        public static string TxtFile
        {
            get
            {
                return Path.Combine(Application.dataPath, "Lerp2API/StandardAssets.txt");
            }
        }

        private const int windowWidth = 600;

        private static string standardAssetsPath = "";
        private static string[] standardAssetsFiles;
        private static bool[] includedStandardAssets;
        private Vector2 windowScroll;

        private static ActiveAssets[] fromLocal,
                                      fromUrl,
                                      fromHDD;

        private static int selectedTab = 0;

        [MenuItem("Lerp2Dev Team Tools/Set Standard packages ready to be uploaded...")]
        public static void Init()
        {
            if (Directory.Exists(string.Format(EditorApplication.applicationContentsPath.Replace("Data", "{0}"), "Standard Packages")))
                Debug.LogWarning("You are using an old version of Unity!");
            standardAssetsPath = string.Format(EditorApplication.applicationContentsPath.Replace("Data", "{0}"), "Standard Assets");
            if (!Directory.Exists(standardAssetsPath))
            {
                Debug.LogWarning("Cannot find Standard Assets folder, you may deleted it or you didn't install it or you are using an old version of Unity!");
                return;
            }
            standardAssetsFiles = Directory.GetFiles(standardAssetsPath);
            includedStandardAssets = new bool[standardAssetsFiles.Length];
            me = GetWindow<StandardInstaller>();
            me.titleContent = new GUIContent("Export Standard Assets (easy way)");
            float height = standardAssetsFiles.Length * 18.4f + 20;
            me.position = new Rect(Screen.currentResolution.width / 2 - windowWidth / 2, Screen.currentResolution.height / 2 - height / 2, windowWidth, height);
            me.Show();
        }

        private void OnGUI()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Local Unity packages", "Unitypackage from URL", "Unitypackage from HDD" });
            switch (selectedTab)
            {
                case 0:
                    GUILayout.BeginVertical();
                    windowScroll = GUILayout.BeginScrollView(windowScroll);
                    for (int i = 0; i < standardAssetsFiles.Length; ++i)
                        includedStandardAssets[i] = GUILayout.Toggle(includedStandardAssets[i], standardAssetsFiles[i].Replace(standardAssetsPath, "").Substring(1));
                    GUILayout.EndScrollView();
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Prepare Asset File Name List"))
                        Prepare();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    break;

                case 1:
                    break;

                case 2:
                    break;
            }
        }

        private void Update()
        {
            if (EditorApplication.isCompiling)
                me.Close();
        }

        private void Prepare()
        {
            using (StreamWriter w = new StreamWriter(TxtFile))
                for (int i = 0; i < standardAssetsFiles.Length; ++i)
                    if (includedStandardAssets[i])
                        w.WriteLine(standardAssetsFiles[i].Replace(standardAssetsPath, "").Substring(1));
            SetEditorPref();
            //File.Create(Path.Combine(Application.dataPath.Replace("/Assets", ""), ".dontDelete"));
            if (EditorUtility.DisplayDialog("Editor message", "A file has been created called 'standardAssets.txt', it contains the Standard Assets to be included. Don't delete it, and of course, add it with two .cs scripts in your Unity Package.\nThanks for using this!", "Ok"))
                me.Close();
        }

        public static void SetEditorPref()
        { //I use editorpref because they cannot be exported. And the api with json (LerpedCore) can be uploaded.
            EditorPrefs.SetBool(EditorKey, true);
        }

        [Serializable]
        internal class ActiveAssets
        {
            public bool active;
            public string name;
        }
    }
}