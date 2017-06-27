using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using Lerp2API;
using Lerp2API.Utility;
using Lerp2APIEditor.Utility;
using Lerp2APIEditor.EditorWindows;
using Debug = Lerp2API._Debug.Debug;
using System.Reflection;

namespace Lerp2APIEditor
{
    /// <summary>
    /// Class LerpedEditorCore.
    /// </summary>
    public class LerpedEditorCore
    {
        /// <summary>
        /// The lerped build target
        /// </summary>
        public static BuildTargetGroup[] LerpedBuildTarget = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.WebGL };
        internal const string buildPath = "BUILD_PATH",
                              editorPath = "EDITOR_PATH",
                              hookShortcut = "UPT_DEP_HOOK",
                              t_CompileWatcher = "COMPILE_WATCHER",
                              timesCompiled = "TIMES_COMPILED",
                              lastBuildTime = "LAST_BUILD_TIME";
        /// <summary>
        /// The main folder
        /// </summary>
        public static string mainFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static string[] resourceFiles = new string[] {
            Path.Combine(mainFolder, "Resources/Images/folder.png"),
            Path.Combine(mainFolder, "Resources/Images/file.png"),
            Path.Combine(mainFolder, "Resources/Skins/File Browser.guiskin")
        },
                                 repFiles = new string[] {
            "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Editor/Resources/Images/folder.png",
            "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Editor/Resources/Images/folder.png",
            "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Editor/Resources/Skins/File%20Browser.guiskin"
        };
        private static LerpedThread<FileSystemWatcher> m_Watcher;
        internal const double threadSeek = .1f; //Decreasing this will make that the Watcher works more accurately
        internal static double nextSeek = 0;

        internal static WWWHandler wh;

        /// <summary>
        /// Gets a value indicating whether [available paths].
        /// </summary>
        /// <value><c>true</c> if [available paths]; otherwise, <c>false</c>.</value>
        public static bool availablePaths
        {
            get
            {
                return !((string.IsNullOrEmpty(LerpedCore.GetString(buildPath)) || !string.IsNullOrEmpty(LerpedCore.GetString(buildPath)) && !Directory.Exists(LerpedCore.GetString(buildPath))) ||
                       (string.IsNullOrEmpty(LerpedCore.GetString(editorPath)) || !string.IsNullOrEmpty(LerpedCore.GetString(editorPath)) && !Directory.Exists(LerpedCore.GetString(editorPath))));
            }
        }

        [InitializeOnLoadMethod]
        static void OnLoadMethods()
        { //This is called at the recompile to set what we need at its start
            AddHook();
            DownloadEditorFiles();
            HookWatchers();
            //Debug.Log("Assigning LerpedCore GameObject: " + LerpedCore.SystemTime);
            //LerpedCore.lerpedCore = LerpedCore.AutoHookCore();
        }

        /*private static GameObject AutoHookCore()
        {
            GameObject core = GameObject.Find("Lerp2Core");
            if (core == null)
                core = new GameObject("Lerp2Core");
            return core;
            EditorReflectionHelpers erh_EditorHook = new EditorReflectionHelpers(() => {
                Type leh = Type.GetType("Lerp2Raw.LerpedEditorHook");
                if (core.GetComponent(leh) == null)
                    core.AddComponent(leh);
            });
            erh_EditorHook.WaitUntilClassIsAvailable("LerpedEditorHook");
            EditorReflectionHelpers erh_Hook = new EditorReflectionHelpers(() => {
                Type lh = Type.GetType("Lerp2Raw.LerpedHook");
                if (core.GetComponent(lh) == null)
                    core.AddComponent(lh);
            });
            erh_Hook.WaitUntilClassIsAvailable("LerpedHook");
        }*/

        private static void AddHook()
        {
            if (!LerpedShortcuts.keyActions.ContainsKey(hookShortcut))
                LerpedShortcuts.keyActions.Add(hookShortcut, new LerpedKeyAction(KeyCode.F5, () => {
                    LerpedPaths lp = EditorWindow.GetWindow<LerpedPaths>(); //I have to improve both of this implementations
                    lp.iInit(lp);
                }));
        }

        private static void DownloadEditorFiles()
        {
            if (resourceFiles.Length != repFiles.Length)
            {
                Debug.LogError("Internal error ocurred, Resource Files and Repository Files Array have to had the same length!");
                return;
            }
            bool ae = resourceFiles.All(x => File.Exists(x));
            if (ae)
                return;
            if (!ae && LerpedUpdater.noConnection)
            {
                Debug.LogError("No conection available to download!");
                return;
            }
            for(int i = 0; i < resourceFiles.Length; ++i)
                if(!File.Exists(resourceFiles[i]))
                {
                    string fp = Path.GetDirectoryName(resourceFiles[i]);
                    if (!Directory.Exists(fp))
                        Directory.CreateDirectory(fp);
                    WWW www = new WWW(repFiles[i]);
                    wh = new WWWHandler();
                    wh.Add(www);
                    wh.Start<WWW>(false, (x) => {
                        File.WriteAllBytes(resourceFiles[i], x.bytes);
                    });
                }
        }

        private static void HookWatchers() //I have to fix this because it deletes on reload in some editors...
        { //Detect new file and add them to the solution? (I have to)
            EditorApplication.update += OnEditorApplicationUpdate;
            if (!availablePaths)
            {
                Debug.LogError("There are one or more paths unsetted, go to Lerp2Dev Team Tools > Refresh Project API Dependencies... to set the paths of the Project in your HDD and the path of this project.");
                return;
            }
            string bPath = LerpedCore.GetString(buildPath);
            m_Watcher = new LerpedThread<FileSystemWatcher>(t_CompileWatcher, new FSWParams(Path.Combine(Path.GetDirectoryName(bPath), "Project"), "*.cs", NotifyFilters.LastWrite | NotifyFilters.Size, true));

            if(m_Watcher != null)
                m_Watcher.matchedMethods.Add(WatcherChangeTypes.Changed.ToString(), () => {
                    LerpedPaths lp = EditorWindow.GetWindow<LerpedPaths>();
                    lp.iInit(lp,  LerpedAPIChange.Auto);
                });

            //m_Watcher.Created += new FileSystemEventHandler(); //I have to add files to the raw solution before compile
            //m_Watcher.Renamed += new FileSystemEventHandler(); //I have to rename files to the raw solution before compile
            //m_Watcher.Deleted += new FileSystemEventHandler(); //I have to remove files to the raw solution before compile

            m_Watcher.StartFSW();
        }

        /// <summary>
        /// Updates the dependencies.
        /// </summary>
        public static void UpdateDependencies()
        {
            string bPath = LerpedCore.GetString(buildPath),
                   bePath = Path.Combine(bPath, "Editor"),
                   bcPath = Path.Combine(bPath, "Console"),
                   brPath = Path.Combine(Path.GetDirectoryName(bPath), "Project/Lerp2Raw"),
                   ePath = LerpedCore.GetString(editorPath),
                   eePath = Path.Combine(ePath, "Editor"),
                   ecPath = Path.Combine(ePath, "Console"),
                   batchPath = Path.Combine(Path.GetDirectoryName(bPath), "Compile/compile.bat");

            LerpedCore.SetLong(lastBuildTime, NativeHelpers.LatestModification(Path.GetDirectoryName(bPath)));

            if(!Directory.Exists(bePath))
            {
                Debug.LogError("Editor path couldn't not be found in Build Path!");
                return;
            }
            if (!Directory.Exists(eePath))
            {
                Debug.LogError("Editor path couldn't not be found in Project Path!");
                return;
            }
            if(!File.Exists(batchPath))
            {
                Debug.LogError("Batch compile file couldn't be found, please, download it from Lerp2Dev Repository.");
                return;
            }

            using (var proc = new Process())
            {
                proc.StartInfo.FileName = batchPath;
                proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(batchPath); //Gracias Elektro: https://foro.elhacker.net/net/processstart_ejecuta_un_batch_y_hace_que_sus_rutas_contiguas_sean_innacesibles-t465478.0.html;msg2109889#msg2109889
                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; //This will be hidden when the console of the api compiles itsself and could send messages to Unity?
                proc.Start();
                proc.WaitForExit();
                //int exitCode = proc.ExitCode;
            }

            string[] fbFiles = new string[] { Path.Combine(bPath, "Lerp2API.dll"), Path.Combine(bPath, "Lerp2API.pdb"), Path.Combine(bPath, "Lerp2API.pdb"),
                                              Path.Combine(bePath, "Lerp2APIEditor.dll"), Path.Combine(bePath, "Lerp2APIEditor.pdb"), Path.Combine(bePath, "Lerp2APIEditor.xml"),
                                              Path.Combine(bcPath, "Lerp2Console.exe"), Path.Combine(bcPath, "Lerp2Console.exe.config"), Path.Combine(bcPath, "Lerp2Console.pdb"), Path.Combine(bcPath, "Lerp2Console.xml") },
                     feFiles = new string[] { Path.Combine(ePath, "Lerp2API.dll"), Path.Combine(ePath, "Lerp2API.pdb"), Path.Combine(ePath, "Lerp2API.pdb"),
                                              Path.Combine(eePath, "Lerp2APIEditor.dll"), Path.Combine(eePath, "Lerp2APIEditor.pdb"), Path.Combine(eePath, "Lerp2APIEditor.xml"),
                                              Path.Combine(ecPath, "Lerp2Console.exe"), Path.Combine(ecPath, "Lerp2Console.exe.config"), Path.Combine(ecPath, "Lerp2Console.pdb"), Path.Combine(ecPath, "Lerp2Console.xml")};

            //Detect file weight? No, by the moment the Editor makes everything...
            int i = 0;
            try
            {
                foreach (string b in fbFiles)
                {
                    if (File.Exists(b))
                        File.Copy(b, feFiles[i], true);
                    else
                        UnityEngine.Debug.LogErrorFormat("{0} not found!", b);
                    ++i;
                }
            }
            catch
            {
                Debug.LogError("Something gone wrong copying files at Refreshing Dependencies, cancel all new bugged API Messages to Auto Refresh it dependencies! (If this continues, please, restart Unity, we're looking this issue)");
            }

            //Also, copy Lerp2Raw files...
            foreach (string file in Directory.GetFiles(brPath, "*.cs"))
                File.Copy(file, Path.Combine(Application.dataPath, "Lerp2API/AttachedScripts/" + Path.GetFileName(file)), true);

            AssetDatabase.Refresh();
            int tc = LerpedCore.GetInt(timesCompiled);
            LerpedCore.SetInt(timesCompiled, ++tc);
        }

        static void OnEditorApplicationUpdate()
        {
            if (EditorApplication.timeSinceStartup > nextSeek) //I have to make an separate counter for different processes in a recent future...
            { //This can be useful for other utilities that needs this refresh rate
                //if (!availablePaths)
                //    return;
                if (availablePaths && m_Watcher != null && m_Watcher.isCalled)
                {
                    foreach (KeyValuePair<string, Action> kv in m_Watcher.matchedMethods)
                        if (m_Watcher.methodCalled == kv.Key)
                            kv.Value();
                    m_Watcher.isCalled = false;
                }
            }
            nextSeek = EditorApplication.timeSinceStartup + threadSeek;
        }

        /// <summary>
        /// Attaches the resource.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="contents">The contents.</param>
        public static void AttachResource(string name, string contents)
        {
            Debug.LogFormat("Attaching {0} file, with content:\n\n{1}", name, contents);
            string path = Path.Combine(Application.dataPath, "Lerp2API/AttachedScripts/"),
                   file = Path.Combine(path, name);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!File.Exists(file) || File.ReadAllText(file) != contents)
                File.WriteAllText(file, contents);
        }
    }
}