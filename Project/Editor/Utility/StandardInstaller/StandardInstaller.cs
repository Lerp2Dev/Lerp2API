using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using Lerp2API.Hepers.JSON_Extensions;
using Lerp2APIEditor.Utility.GUI_Extensions;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using Lerp2API.Hepers.Serializer_Extensions;

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

        /// <summary>
        /// Gets the editor key.
        /// </summary>
        /// <value>The editor key.</value>
        public static string EditorKey
        {
            get
            { //This checks if the current session has already create a package to export.
                return string.Concat(PlayerSettings.companyName, ".", PlayerSettings.productName, ".StandardInstaller.DontInstall");
            }
        }

        /// <summary>
        /// Gets the text file.
        /// </summary>
        /// <value>The text file.</value>
        public static string TxtFile
        {
            get
            {
                return Path.Combine(Application.dataPath, "Lerp2API/StandardAssets.txt");
            }
        }

        /// <summary>
        /// Gets the json file.
        /// </summary>
        /// <value>The json file.</value>
        public static string JSONFile
        {
            get
            {
                return Path.Combine(Application.dataPath, "Lerp2API/StandardAssets.json");
            }
        }

        private static float defHeight
        {
            get
            {
                return EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).GetStyle("textfield").CalcHeight(new GUIContent("abcd"), defTextFieldWidth);
            }
        }

        private const int windowWidth = 600,
                          windowHeight = 400, //Estaria bien q automaticamente cogiese la altura solo
                          defTextFieldWidth = 450;

        private Vector2 windowScroll,
                        fromScroll;

        private static AssetsLocation[] fromWhere = new AssetsLocation[3];
        private static int selectedTab = 0;
        private readonly static LerpedList[] m_Lists = new LerpedList[2];

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [MenuItem("Lerp2Dev Team Tools/Set Standard packages ready to be uploaded...")]
        public static void Init()
        {
            if (Directory.Exists(string.Format(EditorApplication.applicationContentsPath.Replace("Data", "{0}"), "Standard Packages")))
                Debug.LogWarning("You are using an old version of Unity!");

            if (File.Exists(JSONFile))
            {
                AssetsLocation[] aLocs = JSONHelpers.DeserializeFromFile<AssetsLocation[]>(JSONFile);
                if (!IsEmpty(aLocs))
                    fromWhere = aLocs;
                else
                    fromWhere = new AssetsLocation[3];
            }

            bool edited = false; //This can be optimized (I have to)

            //From Local
            if (IsNullLocation(AssetLocation.Local))
            {
                string standardAssetsPath = string.Format(EditorApplication.applicationContentsPath.Replace("Data", "{0}"), "Standard Assets");

                if (!Directory.Exists(standardAssetsPath))
                {
                    Debug.LogWarning("Cannot find Standard Assets folder, you may deleted it or you didn't install it or you are using an old version of Unity!");
                    return;
                }

                string[] paths = Directory.GetFiles(standardAssetsPath);

                List<ActiveAsset> aAsset = new List<ActiveAsset>(paths.Length);

                foreach (string path in paths)
                {
                    //Aqui le vamos a ir añadiendo valor a valor
                    aAsset.Add(new ActiveAsset(Path.GetFileName(path), path));
                }

                AssetsLocation aLoc = CreateInstance<AssetsLocation>();

                aLoc.assets = aAsset;
                aLoc.location = AssetLocation.Local;

                fromWhere[0] = aLoc;
                edited = true;
            }

            foreach (int val in new int[2] { 1, 2 })
                if (IsNullLocation((AssetLocation)val))
                {
                    AssetsLocation aLoc = CreateInstance<AssetsLocation>();

                    aLoc.assets = new List<ActiveAsset>();
                    aLoc.location = (AssetLocation)val;

                    fromWhere[val] = aLoc; //new AssetsLocation(AssetLocation.Local, aAsset);
                    edited = true;
                }

            if (edited)
                SnapJSON(); //Esto no parece que salte, hay algun tipo de problema...

            me = GetWindow<StandardInstaller>();
            me.titleContent = new GUIContent("Export Standard Assets (easy way)");

            //float height = standardAssetsFiles.Length * 18.4f + 20; //Esto lo arreglo con: https://docs.unity3d.com/ScriptReference/GUI.BeginScrollView.html

            me.position = new Rect(Screen.currentResolution.width / 2 - windowWidth / 2, Screen.currentResolution.height / 2 - windowHeight / 2, windowWidth, windowHeight);
            me.Show();

            for (int i = 0; i < m_Lists.Length; ++i)
            {
                m_Lists[i] = new LerpedList(me, fromWhere[i + 1].serializedObj, fromWhere[i + 1].serializedObj.FindProperty("assets"), string.Format("From {0}", ((AssetLocation)(i + 1)).ToString()));

                m_Lists[i].SetElementCallback(i, (idx) =>
                { //ESTO TODAVIA NO FUNCIONA... TENGO QUE HACER WL FSW, EL WWWHANDLER PARA LA DESCARGA DE ESTOS ARCHIVOS Y DEMÁS...
                    return (rect, index, active, focused) =>
                    {
                        int ivalue = idx + 1,
                            len = m_Lists[idx].PropLength;

                        //Debug.Log(m_Lists[idx].m_Prop.isArray);

                        //Better than a try-catch structure, Thanks ElektroStudios
                        if (m_Lists[idx].m_Prop.isArray && m_Lists[idx].m_Prop.GetAt(index) == null)
                            m_Lists[idx].m_Prop.Add(new ActiveAsset());

                        //SerializedProperty sProp = m_Lists[idx].m_Prop.GetAt(index);

                        //Debug.Log(sProp.GetEndProperty().type);

                        if (index >= len)
                            for (int j = 0; j < len - index + 1; ++j)
                                m_Lists[idx].m_Prop.Add(new ActiveAsset());

                        /*GUILayout.BeginArea(new Rect(rect.x, rect.y + 43, windowWidth - 10, defHeight + 4));
                        GUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField(string.Format("{0} {1}", ((AssetLocation)(idx + 1)).ToString(), index), GUILayout.Width(60));

                        sProp.FindPropertyRelative("active").boolValue = EditorGUILayout.Toggle(sProp.FindPropertyRelative("active").boolValue, GUILayout.Width(20));
                        sProp.FindPropertyRelative("path").stringValue = EditorGUILayout.TextField(sProp.FindPropertyRelative("path").stringValue, GUILayout.Width(defTextFieldWidth));

                        GUILayout.EndHorizontal();
                        GUILayout.EndArea();*/

                        //m_Lists[idx].m_Obj.Update();
                        AssetsLocation aLoc = m_Lists[idx].m_Obj.targetObject as AssetsLocation;
                        ActiveAsset aAss = aLoc.assets[index];

                        if (aAss == null)
                        {
                            Debug.Log("Shitty shit!");
                            aAss = new ActiveAsset();
                        }

                        GUILayout.BeginArea(new Rect(rect.x, rect.y + 43, windowWidth - 10, defHeight + 4));
                        GUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField(string.Format("{0} {1}", ((AssetLocation)(idx + 1)).ToString(), index), GUILayout.Width(60));

                        aAss.active = EditorGUILayout.Toggle(aAss.active, GUILayout.Width(20));
                        aAss.path = EditorGUILayout.TextField(aAss.path, GUILayout.Width(defTextFieldWidth));

                        GUILayout.EndHorizontal();
                        GUILayout.EndArea();

                        //aLoc.assets[index] = aAss;
                        m_Lists[idx].m_Obj.FindProperty("assets").GetAt(index).SetValue(aAss);
                        m_Lists[idx].m_Obj.ApplyModifiedProperties();
                    };
                });

                m_Lists[i].SetElementBackgroundCallback(i, (idx) =>
                {
                    return (rect, index, active, focused) =>
                    {
                        rect.height = defHeight + 4;
                        if (active)
                            GUI.DrawTexture(rect, m_Lists[idx].backgroundTex);
                    };
                });

                m_Lists[i].SetHeightElementCallback(i, (idx) =>
                {
                    return (index) =>
                    {
                        if (m_Lists[idx].m_Type == ReferType.Editor)
                            ((Editor)m_Lists[idx].m_Refer).Repaint();
                        else
                            ((EditorWindow)m_Lists[idx].m_Refer).Repaint();

                        return defHeight + 4;
                    };
                });

                m_Lists[i].SetAddDropdownCallback(i, (idx) =>
                {
                    return (rect, li) =>
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Add Element"), false, () =>
                        {
                            m_Lists[idx].m_Obj.Update();
                            li.serializedProperty.Add(new ActiveAsset());
                            m_Lists[idx].m_Obj.ApplyModifiedProperties();
                        });

                        menu.ShowAsContext();
                    };
                });
            }
        }

        private void OnGUI()
        {
            selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Local Unity packages", "Unitypackage from URL", "Unitypackage from HDD" });

            AssetsLocation aLoc = GetLocationFromTab(selectedTab);

            GUILayout.BeginVertical();
            switch (selectedTab)
            {
                case 0:
                    if (aLoc != null)
                    {
                        windowScroll = GUILayout.BeginScrollView(windowScroll);
                        for (int i = 0; i < aLoc.assets.Count(); ++i)
                        {
                            ActiveAsset aAsset = aLoc.assets.ElementAt(i);
                            aAsset.active = GUILayout.Toggle(aAsset.active, aAsset.name);
                        }
                        GUILayout.EndScrollView();
                    }
                    break;

                case 1:
                case 2:
                    if (aLoc != null)
                        m_Lists[selectedTab - 1].Draw();
                    break;
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Prepare Asset File Name List"))
                Prepare();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reset Standard Asset List"))
                ResetAll();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void Update()
        {
            if (EditorApplication.isCompiling)
                me.Close();
        }

        private void OnDestroy()
        {
            SnapJSON();
        }

        private static void SnapJSON()
        {
            fromWhere.SerializeToFile(JSONFile, true);
        }

        private static void ResetAll()
        {
            File.Delete(JSONFile);
            File.Delete(JSONFile + ".meta");
            AssetDatabase.Refresh();
            me.Close();
        }

        private void Prepare()
        {
            SnapJSON();
            SetEditorPref();

            if (EditorUtility.DisplayDialog("Editor message", "A file has been created called 'StandardAssets.json', it contains the Standard Assets to be included. Don't delete it, and of course, include it when you upload this Asset (including the API also in it).\nThanks for using this!", "Ok"))
                me.Close();
        }

        /// <summary>
        /// Sets the editor preference.
        /// </summary>
        public static void SetEditorPref()
        { //I use editorpref because they cannot be exported. And the api with json (LerpedCore) can be uploaded.
            EditorPrefs.SetBool(EditorKey, true);
        }

        internal static AssetsLocation GetLocationFromTab(int tab)
        {
            if (fromWhere == null || (fromWhere != null && tab >= fromWhere.Length - 1))
                return null;

            AssetLocation aLoc = (AssetLocation)tab;

            return fromWhere.Where(x => x != null).SingleOrDefault(x => x.location == aLoc);
        }

        internal static bool IsNullLocation(AssetLocation aLoc)
        {
            AssetsLocation aLocs = GetLocationFromTab((int)aLoc);
            return aLocs == null || (aLocs != null && aLocs.assets.Count() == 0);
        }

        internal static bool IsEmpty(AssetsLocation[] aLocs = null)
        {
            if (aLocs == null)
                aLocs = fromWhere;
            return aLocs == null || (aLocs != null && (aLocs.All(x => x == null) || aLocs.Length == 0));
        }
    }

    /// <summary>
    /// Class AssetsLocation.
    /// </summary>
    /// <seealso cref="UnityEngine.ScriptableObject" />
    [Serializable]
    public class AssetsLocation : ScriptableObject
    {
        /// <summary>
        /// The location
        /// </summary>
        [SerializeField]
        public AssetLocation location;

        /// <summary>
        /// The assets
        /// </summary>
        [SerializeField]
        public List<ActiveAsset> assets = new List<ActiveAsset>(1);

        private SerializedObject _serObj;

        /// <summary>
        /// Gets the serialized object.
        /// </summary>
        /// <value>The serialized object.</value>
        public SerializedObject serializedObj
        {
            get
            {
                if (_serObj == null)
                    _serObj = new SerializedObject(this);
                return _serObj;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetsLocation"/> class.
        /// </summary>
        /// <param name="aLoc">a loc.</param>
        /// <param name="aas">The aas.</param>
        public AssetsLocation(AssetLocation aLoc, List<ActiveAsset> aas)
        {
            location = aLoc;
            assets = aas;
        }
    }

    /// <summary>
    /// Class ActiveAsset.
    /// </summary>
    /// <seealso cref="UnityEngine.Object" />
    [Serializable]
    public class ActiveAsset : Object
    {
        /// <summary>
        /// The active
        /// </summary>
        public bool active;

        /// <summary>
        /// The name
        /// </summary>
        public string name = "",
                      /// <summary>
                      /// The path
                      /// </summary>
                      path = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveAsset"/> class.
        /// </summary>
        public ActiveAsset()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveAsset"/> class.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="p">The p.</param>
        public ActiveAsset(string n, string p)
        {
            name = n;
            path = p;
        }
    }
}