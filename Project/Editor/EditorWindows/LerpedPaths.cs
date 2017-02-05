using UnityEngine;
using UnityEditor;
using Lerp2API;
using Lerp2API.Utility;
using System.IO;

namespace Lerp2APIEditor.EditorWindows
{
    public enum LerpedAPIChange { Auto, InEnter, Default }
    public class LerpedPaths : EditorWindow
    {
        public static LerpedPaths me;

        internal BrowserWindow bBrowser,
                                      eBrowser;
        internal string bPath = "",
                        ePath = "";
        internal const string buildPath = "BUILD_PATH",
                              editorPath = "EDITOR_PATH";

        [MenuItem("Lerp2Dev Team Tools/Refesh Project API Dependencies...")]
        static void Init()
        {
            me = GetWindow<LerpedPaths>();
            me.iInit(me);
        }

        [InitializeOnLoad]
        class LerpedStartCheck
        {
            static LerpedStartCheck()
            {
                if (!LerpedCore.GetBool(LerpedEditorCore.UnityBoot))
                {
                    LerpedCore.SetBool(LerpedEditorCore.UnityBoot, true);
                    string bPath = Path.GetDirectoryName(LerpedCore.GetString(LerpedEditorCore.buildPath));
                    if (string.IsNullOrEmpty(bPath))
                        return;

                    long estimatedModTime = LerpedCore.GetLong(LerpedEditorCore.lastBuildTime),
                         lastModTime = Helpers.LatestModification(bPath);

                    if (estimatedModTime != lastModTime)
                    {
                        LerpedPaths lp = GetWindow<LerpedPaths>();
                        lp.iInit(lp, LerpedAPIChange.InEnter);
                    }
                }
            }
        }

        public void iInit(LerpedPaths rf, LerpedAPIChange change = LerpedAPIChange.Default)
        {
            if (rf != null)
                rf.Close();

            bool rt = false;
            if (LerpedEditorCore.availablePaths)
                if (LerpedEditorCore.buildPath.Contains(Application.dataPath))
                    EditorUtility.DisplayDialog("API Message", "You have reversed the path, you should put in Project API the path from this Project,\nand in Build Path, the path where the DLL are builded!", "Ok");
                else
                {
                    int r = EditorUtility.DisplayDialogComplex("API Message", GetCaption(change), "Yes", "No", "Change path");
                    switch (r)
                    {
                        case 0:
                            LerpedEditorCore.UpdateDependencies();
                            rt = true;
                            return;
                        case 1:
                            rt = true;
                            return;
                        case 2:
                            break;
                    }
                }

            if (rt)
                return;

            if (me == null)
            {
                if (rf == null)
                    me = GetWindow<LerpedPaths>();
                else
                    me = rf;
            }

            bPath = LerpedCore.GetString(buildPath);
            ePath = LerpedCore.GetString(editorPath);

            bPath = string.IsNullOrEmpty(bPath) ? Application.dataPath : bPath;
            ePath = string.IsNullOrEmpty(ePath) ? Application.dataPath : ePath;

            bBrowser = CreateInstance<BrowserWindow>();
            bBrowser.fbt = FileBrowserType.Directory;
            bBrowser.name = "Build Dependencies Path";
            bBrowser.height = 200;
            bBrowser.cb = (x) =>
            {
                bPath = x;
            };

            eBrowser = CreateInstance<BrowserWindow>();
            eBrowser.fbt = FileBrowserType.Directory;
            eBrowser.name = "Project Dependencies Path";
            eBrowser.height = 200;
            eBrowser.cb = (x) =>
            {
                bPath = x;
            };

            me.position = new Rect(Screen.resolutions[0].width / 2 - 200, Screen.resolutions[0].height / 2 - 150, 400, 300);
            me.Show();
        }

        private static string GetCaption(LerpedAPIChange change)
        {
            switch(change)
            {
                case LerpedAPIChange.Default:
                    return "Do you want to update dependence files?";
                case LerpedAPIChange.Auto:
                    return "Origin files has been changed, do you want to update them?";
                case LerpedAPIChange.InEnter:
                    return "Origin files has been changed since your last visit, do you want to update them?";
                default:
                    return "";
            }
        }

        void OnGUI()
        {
            if (me == null) //Now, it isn't needed to check static variables to be null, me englobes everything
                return;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Build Dependencies Path");
            bPath = GUILayout.TextField(bPath, GUILayout.Width(180));
            if (GUILayout.Button("...", GUILayout.Width(16), GUILayout.Height(16)))
                bBrowser.Init();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Project API Dependencies Path");
            ePath = GUILayout.TextField(ePath, GUILayout.Width(180));
            if (GUILayout.Button("...", GUILayout.Width(16), GUILayout.Height(16)))
                eBrowser.Init();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save changes"))
                SaveChanges();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        internal void SaveChanges()
        {
            LerpedCore.SetString(buildPath, bPath);
            LerpedCore.SetString(editorPath, ePath);
            me.Close();
        }
    }
    [ExecuteInEditMode]
    [InitializeOnLoad]
    public class LerpedEditorHook : MonoBehaviour
    {
        static LerpedEditorHook()
        {
            LerpedEditorCore.AutoHook<LerpedEditorHook>();
        }
        void OnDestroy()
        {
            LerpedCore.SetBool(LerpedEditorCore.UnityBoot, false);
        }
    }
}
