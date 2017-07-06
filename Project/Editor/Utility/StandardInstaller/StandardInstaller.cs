using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace Lerp2API.Utility.StandardInstaller
{
    public enum AssetLocation
    {
        Local,
        URL,
        HDD
    }

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

        public static string JSONFile
        {
            get
            {
                return Path.Combine(Application.dataPath, "Lerp2API/StandardAssets.json");
            }
        }

        private const int windowWidth = 600,
                          windowHeight = 400; //Estaria bien q automaticamente cogiese la altura solo

        //private static string standardAssetsPath = "";

        //private static string[] standardAssetsFiles;
        //private static bool[] includedStandardAssets;
        private Vector2 windowScroll;

        private static AssetsLocation[] fromWhere = new AssetsLocation[3];

        private static int selectedTab = 0;

        [MenuItem("Lerp2Dev Team Tools/Set Standard packages ready to be uploaded...")]
        public static void Init()
        {
            if (Directory.Exists(string.Format(EditorApplication.applicationContentsPath.Replace("Data", "{0}"), "Standard Packages")))
                Debug.LogWarning("You are using an old version of Unity!");

            if (fromWhere == null)
                fromWhere = JsonUtility.FromJson<AssetsLocation[]>(JSONFile);

            //From Local
            if (IsNullLocation(AssetLocation.Local) == null)
            {
                string standardAssetsPath = string.Format(EditorApplication.applicationContentsPath.Replace("Data", "{0}"), "Standard Assets");

                if (!Directory.Exists(standardAssetsPath))
                {
                    Debug.LogWarning("Cannot find Standard Assets folder, you may deleted it or you didn't install it or you are using an old version of Unity!");
                    return;
                }

                string[] paths = Directory.GetFiles(standardAssetsPath);

                ActiveAsset[] aAsset = new ActiveAsset[paths.Length];

                for (int i = 0; i < paths.Length; ++i)
                {
                    //Aqui le vamos a ir añadiendo valor a valor
                    aAsset[i] = new ActiveAsset(Path.GetFileName(paths[i]), paths[i]);
                }

                fromWhere[0] = new AssetsLocation(AssetLocation.Local, aAsset);

                //standardAssetsFiles = ;
                //includedStandardAssets = new bool[standardAssetsFiles.Length];
            }

            if (IsNullLocation(AssetLocation.HDD) == null)
            {
            }

            if (IsNullLocation(AssetLocation.URL) == null)
            {
            }

            me = GetWindow<StandardInstaller>();
            me.titleContent = new GUIContent("Export Standard Assets (easy way)");

            //float height = standardAssetsFiles.Length * 18.4f + 20; //Esto lo arreglo con: https://docs.unity3d.com/ScriptReference/GUI.BeginScrollView.html

            me.position = new Rect(Screen.currentResolution.width / 2 - windowWidth / 2, Screen.currentResolution.height / 2 - windowHeight / 2, windowWidth, windowHeight);
            me.Show();
        }

        private void OnGUI()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Local Unity packages", "Unitypackage from URL", "Unitypackage from HDD" });

            AssetsLocation aLoc = GetLocationFromTab(selectedTab);

            if (aLoc != null)
            {
                switch (selectedTab)
                {
                    case 0:
                        GUILayout.BeginVertical();
                        windowScroll = GUILayout.BeginScrollView(windowScroll);
                        //for (int i = 0; i < standardAssetsFiles.Length; ++i)
                        //    includedStandardAssets[i] = GUILayout.Toggle(includedStandardAssets[i], standardAssetsFiles[i].Replace(standardAssetsPath, "").Substring(1));
                        for (int i = 0; i < aLoc.assets.Length; ++i)
                        {
                            ActiveAsset aAsset = aLoc.assets[i];
                            aAsset.active = GUILayout.Toggle(aAsset.active, aAsset.name);
                        }
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
        }

        private void Update()
        {
            if (EditorApplication.isCompiling)
                me.Close();
        }

        private void Prepare()
        {
            /*using (StreamWriter w = new StreamWriter(TxtFile))
                for (int i = 0; i < standardAssetsFiles.Length; ++i)
                    if (includedStandardAssets[i])
                        w.WriteLine(standardAssetsFiles[i].Replace(standardAssetsPath, "").Substring(1));*/
            File.WriteAllText(JSONFile, JsonUtility.ToJson(fromWhere));
            SetEditorPref();
            //File.Create(Path.Combine(Application.dataPath.Replace("/Assets", ""), ".dontDelete"));
            if (EditorUtility.DisplayDialog("Editor message", "A file has been created called 'StandardAssets.txt', it contains the Standard Assets to be included. Don't delete it, and of course, include it when you upload this Asset (including the API also in it).\nThanks for using this!", "Ok"))
                me.Close();
        }

        public static void SetEditorPref()
        { //I use editorpref because they cannot be exported. And the api with json (LerpedCore) can be uploaded.
            EditorPrefs.SetBool(EditorKey, true);
        }

        internal static AssetsLocation GetLocationFromTab(int tab)
        {
            if (fromWhere == null || (fromWhere != null && tab >= fromWhere.Length - 1))
                return null;

            AssetLocation aLoc = (AssetLocation)tab;

            return fromWhere.SingleOrDefault(x => x.location == aLoc);
        }

        internal static bool IsNullLocation(AssetLocation aLoc)
        {
            AssetsLocation aLocs = GetLocationFromTab((int)aLoc);
            return aLocs == null || (aLocs != null && aLocs.assets.Length == 0);
        }

        [Serializable]
        internal class AssetsLocation
        {
            public AssetLocation location;
            public ActiveAsset[] assets;

            public AssetsLocation(AssetLocation aLoc, ActiveAsset[] aas)
            {
                location = aLoc;
                assets = aas;
            }
        }

        [Serializable]
        internal class ActiveAsset
        {
            public bool active;
            public string name, path;

            public ActiveAsset(string n, string p)
            {
                name = n;
                path = p;
            }
        }
    }
}