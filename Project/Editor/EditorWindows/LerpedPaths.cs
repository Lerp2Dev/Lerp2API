using UnityEngine;
using UnityEditor;
using Lerp2API;
using Lerp2API.Utility;

namespace Lerp2APIEditor.EditorWindows
{
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

        public void iInit(LerpedPaths rf, bool auto = false)
        {
            if (rf != null)
                rf.Close();

            bool rt = false;
            if (LerpedEditorCore.availablePaths)
                if (LerpedEditorCore.buildPath.Contains(Application.dataPath))
                    EditorUtility.DisplayDialog("API Message", "You have reversed the path, you should put in Project API the path from this Project,\nand in Build Path, the path where the DLL are builded!", "Ok");
                else
                {
                    int r = EditorUtility.DisplayDialogComplex("API Message", auto ? "Origin files has been changed, do you want to update them?" : "Do you want to update dependence files?", "Yes", "No", "Change path");
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
}
