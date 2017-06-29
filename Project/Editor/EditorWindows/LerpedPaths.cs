using UnityEngine;
using UnityEditor;
using Lerp2API;
using Lerp2API.Utility;
using System.IO;
using System;

namespace Lerp2APIEditor.EditorWindows
{
    /// <summary>
    /// Class LerpedPaths.
    /// </summary>
    /// <seealso cref="UnityEditor.EditorWindow" />
    public class LerpedPaths : EditorWindow
    {
        /// <summary>
        /// Me
        /// </summary>
        public static LerpedPaths me;

        internal BrowserWindow bBrowser,
                                      eBrowser;

        internal string bPath = "",
                        ePath = "";

        internal const string buildPath = "BUILD_PATH",
                              editorPath = "EDITOR_PATH";

        [MenuItem("Lerp2Dev Team Tools/Refesh Project API Dependencies...")]
        private static void Init()
        {
            me = GetWindow<LerpedPaths>();
            me.iInit(me);
        }

        [InitializeOnLoad]
        private class LerpedStartCheck
        {
            static LerpedStartCheck()
            {
                if (!LerpedCore.GetBool(LerpedCore.UnityBoot))
                {
                    LerpedCore.SetBool(LerpedCore.UnityBoot, true);
                    string dir = LerpedCore.GetString(LerpedEditorCore.buildPath),
                           bPath = Directory.Exists(dir) ? Path.GetDirectoryName(dir) : "";
                    if (string.IsNullOrEmpty(bPath))
                    {
                        bPath = "";
                        return;
                    }
                    else if (!Directory.Exists(bPath))
                    {
                        Debug.LogErrorFormat("Your fork from Lerp2API has been moved, re-edit the path under: 'Lerp2Dev Team Tools > Refresh Project API References > Change Path > Build Dependencies Path', click '...' and set the new path, pressing 'Save'.\n'{0}' directory path doesn't exists!", bPath);
                        return;
                    }

                    long estimatedModTime = LerpedCore.GetLong(LerpedEditorCore.lastBuildTime),
                         lastModTime = NativeHelpers.LatestModification(bPath);

                    if (estimatedModTime != lastModTime)
                    {
                        LerpedPaths lp = GetWindow<LerpedPaths>();
                        lp.iInit(lp, LerpedAPIChange.InEnter);
                    }
                }
            }
        }

        /// <summary>
        /// is the initialize.
        /// </summary>
        /// <param name="rf">The rf.</param>
        /// <param name="change">The change.</param>
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
            ePath = LerpedCore.GetString(editorPath); //Is null?

            bPath = string.IsNullOrEmpty(bPath) ? Application.dataPath : bPath;
            ePath = string.IsNullOrEmpty(ePath) ? Application.dataPath : ePath;

            me.position = new Rect(Screen.resolutions[0].width / 2 - 200, Screen.resolutions[0].height / 2 - 150, 400, 300);
            me.Show();
        }

        private static string GetCaption(LerpedAPIChange change)
        {
            switch (change)
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

        private void OnGUI()
        {
            if (me == null) //Now, it isn't needed to check static variables to be null, me englobes everything
                return;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Build Dependencies Path");
            bPath = GUILayout.TextField(bPath, GUILayout.Width(180));
            if (GUILayout.Button("...", GUILayout.Width(16), GUILayout.Height(16)))
                ShowBrowser("Build Dependencies Path", out bBrowser);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Project API Dependencies Path");
            ePath = GUILayout.TextField(ePath, GUILayout.Width(180));
            if (GUILayout.Button("...", GUILayout.Width(16), GUILayout.Height(16)))
                ShowBrowser("Project Dependencies Path", out bBrowser);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Save changes"))
                SaveChanges();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void ShowBrowser(string name, out BrowserWindow win)
        {
            win = GetWindow<BrowserWindow>();
            win.fbt = FileBrowserType.Directory;
            win.name = name;
            win.height = 200;
            win.cb = (x) =>
            {
                bPath = x;
            };
            win.Init();
        }

        internal void SaveChanges()
        {
            LerpedCore.SetString(buildPath, bPath);
            LerpedCore.SetString(editorPath, ePath);
            me.Close();
        }
    }
}