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
using Debug = Lerp2API.DebugHandler.Debug;
using System.Reflection;

namespace Lerp2APIEditor
{
    public class LerpedEditorCore
    {
        public static BuildTargetGroup[] LerpedBuildTarget = new BuildTargetGroup[] { BuildTargetGroup.Standalone, BuildTargetGroup.WebGL };
        internal const string buildPath = "BUILD_PATH",
                              editorPath = "EDITOR_PATH",
                              hookShortcut = "UPT_DEP_HOOK",
                              t_CompileWatcher = "COMPILE_WATCHER",
                              timesCompiled = "TIMES_COMPILED",
                              lastBuildTime = "LAST_BUILD_TIME";
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

        public static bool availablePaths
        {
            get
            {
                return !((string.IsNullOrEmpty(LerpedCore.GetString(buildPath)) || !string.IsNullOrEmpty(LerpedCore.GetString(buildPath)) && !Directory.Exists(LerpedCore.GetString(buildPath))) ||
                       (string.IsNullOrEmpty(LerpedCore.GetString(editorPath)) || !string.IsNullOrEmpty(LerpedCore.GetString(editorPath)) && !Directory.Exists(LerpedCore.GetString(editorPath))));
            }
        }

        [InitializeOnLoadMethod]
        static void AddHook()
        {
            if (!LerpedShortcuts.keyActions.ContainsKey(hookShortcut))
                LerpedShortcuts.keyActions.Add(hookShortcut, new LerpedKeyAction(KeyCode.F5, () => {
                    LerpedPaths lp = EditorWindow.GetWindow<LerpedPaths>(); //I have to improve both of this implementations
                    lp.iInit(lp);
                }));
        }

        [InitializeOnLoadMethod]
        static void DownloadEditorFiles()
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

        [InitializeOnLoadMethod]
        static void HookWatchers() //I have to fix this because it deletes on reaload in some editors
        { //Detect new file and add them to the solution? (I have to)
            EditorApplication.update += OnEditorApplicationUpdate;
            if (!availablePaths)
            {
                Debug.LogError("There's any Path setted already, go to Lerp2Dev Team Tools > Refresh Project API Dependencies... to set the paths of the Project in your HDD and the path of this project.");
                return;
            }
            string bPath = LerpedCore.GetString(buildPath);
            m_Watcher = new LerpedThread<FileSystemWatcher>(t_CompileWatcher, new FSWParams(Path.Combine(Path.GetDirectoryName(bPath), "Project"), "*.cs", NotifyFilters.LastWrite | NotifyFilters.Size, true));

            m_Watcher.matchedMethods.Add(WatcherChangeTypes.Changed.ToString(), () => {
                LerpedPaths lp = EditorWindow.GetWindow<LerpedPaths>();
                lp.iInit(lp,  LerpedAPIChange.Auto);
            });

            //m_Watcher.Created += new FileSystemEventHandler(); //I have to add files to the raw solution before compile
            //m_Watcher.Renamed += new FileSystemEventHandler(); //I have to rename files to the raw solution before compile
            //m_Watcher.Deleted += new FileSystemEventHandler(); //I have to remove files to the raw solution before compile

            m_Watcher.StartFSW();
        }

        public static void UpdateDependencies()
        {
            string bPath = LerpedCore.GetString(buildPath),
                   bePath = Path.Combine(bPath, "Editor"),
                   ePath = LerpedCore.GetString(editorPath),
                   eePath = Path.Combine(ePath, "Editor"),
                   batchPath = Path.Combine(Path.GetDirectoryName(bPath), "Compile/compile.bat");

            LerpedCore.SetLong(lastBuildTime, Helpers.LatestModification(bPath));

            if(!Directory.Exists(bePath))
            {
                Debug.Log("Editor path couldn't not be found in Build Path!");
                return;
            }
            if (!Directory.Exists(eePath))
            {
                Debug.Log("Editor path couldn't not be found in Project Path!");
                return;
            }

            using (Process proc = new Process())
            {
                proc.StartInfo.FileName = batchPath;
                //proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
                proc.WaitForExit();
                int exitCode = proc.ExitCode;
            }

            string[] fbFiles = new string[] { Path.Combine(bPath, "Lerp2API.dll"), Path.Combine(bPath, "Lerp2API.pdb"), Path.Combine(bPath, "Lerp2API.pdb"),
                                              Path.Combine(bePath, "Lerp2APIEditor.dll"), Path.Combine(bePath, "Lerp2APIEditor.pdb"), Path.Combine(bePath, "Lerp2APIEditor.xml") },
                     feFiles = new string[] { Path.Combine(ePath, "Lerp2API.dll"), Path.Combine(ePath, "Lerp2API.pdb"), Path.Combine(ePath, "Lerp2API.pdb"),
                                              Path.Combine(eePath, "Lerp2APIEditor.dll"), Path.Combine(eePath, "Lerp2APIEditor.pdb"), Path.Combine(eePath, "Lerp2APIEditor.xml") };

            //Detect file weight? No, by the moment the Editor makes everything...
            int i = 0;
            foreach (string b in fbFiles)
            {
                if (File.Exists(b))
                    File.Copy(b, feFiles[i], true);
                else
                    UnityEngine.Debug.LogErrorFormat("{0} not found!", b);
                ++i;
            }
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
    }
}