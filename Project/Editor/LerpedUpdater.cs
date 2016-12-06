using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Net;
using System.Linq;
using Lerp2API;
using Lerp2API.Utility;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2APIEditor
{
    [InitializeOnLoad]
    public class LerpedUpdater
    {
        internal const string curVersion = "1.1.1a",
                              versionUrl = "http://raw.githubusercontent.com/Lerp2Dev/Lerp2API/master/Lerp2API.version";

        internal static string localVersionFilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Lerp2API.version");
        public static bool noConnection;
        internal static string[] updateUrls = new string[] {
            "https://raw.githubusercontent.com/Lerp2Dev/Lerp2API/master/Build/Lerp2API.dll",
            "https://raw.githubusercontent.com/Lerp2Dev/Lerp2API/master/Build/Lerp2API.pdb",
            "https://raw.githubusercontent.com/Lerp2Dev/Lerp2API/master/Build/Lerp2API.xml",
            "https://raw.githubusercontent.com/Lerp2Dev/Lerp2API/master/Build/Editor/Lerp2APIEditor.dll",
            "https://raw.githubusercontent.com/Lerp2Dev/Lerp2API/master/Build/Editor/Lerp2APIEditor.pdb",
            "https://raw.githubusercontent.com/Lerp2Dev/Lerp2API/master/Build/Editor/Lerp2APIEditor.xml",
            versionUrl
        };

        public string versionName = "1.1.1 Alpha Release",
                      versionStr = "1.1.1a",
                      versionChangelog = "Fixed wrong url to update editor dependencies.\nImproved WWWHandler system, less code lines are needed to make it work.\nSome utilities to Array Extensions Helpers.";

        internal static WWWHandler wh;

        static LerpedUpdater()
        {
            UpdateCheck();
            DefineCheck();
            GetMissingAssets();
        }

        public LerpedUpdater()
        {
        }

        public LerpedUpdater(string vn, string vs, string vc)
        {
            versionName = vn;
            versionStr = vs;
            versionChangelog = vc;
        }

        internal static void CheckForConnection()
        {
            try
            {
                using (WebClient client = new WebClient())
                using (client.OpenRead("http://www.google.com"))
                    noConnection = false;
            }
            catch
            {
                noConnection = true;
            }
        }

        internal static void DoUpdate(string newerVersion)
        {
            string depPath = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                   editorPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string[] deps = new string[] {
                Path.Combine(depPath, "Lerp2API.dll"),
                Path.Combine(depPath, "Lerp2API.pdb"),
                Path.Combine(depPath, "Lerp2API.xml"),
                Path.Combine(editorPath, "Lerp2APIEditor.dll"),
                Path.Combine(editorPath, "Lerp2APIEditor.pdb"),
                Path.Combine(editorPath, "Lerp2APIEditor.xml"),
                Path.Combine(editorPath, "Lerp2API.version")
            };
            WWW[] wwws = updateUrls.GetWWW();
            if (!deps.All(x => File.Exists(x)))
                Debug.LogErrorFormat("Some Library files doesn't exist, aborting update mission!\nDownload them manually from {0}, and put them in '{1}' folder.", 
                    @"<a href=""https://github.com/Lerp2Dev/Lerp2API/tree/master/Build"">here</a>", 
                    depPath);
            else
            {
                wh = new WWWHandler();
                wh.Add(wwws);
                wh.Start<WWW[]>(false, (x) => 
                {
                    int v = 0;
                    try
                    {
                        for (int i = 0; i < x.Length; ++i)
                        {
                            File.Delete(deps[i]);
                            v = i;
                            File.WriteAllBytes(deps[i], x[i].bytes);
                        }
                    }
                    catch(Exception ex)
                    {
                        Debug.LogFormat("Oh guy! You are so fast, {0} has not been downloaded!\n{1}\n{2}", x[v].url, ex.Message, ex.StackTrace);
                    }
                    Debug.LogFormat("Update to '{0}' successfully done!", newerVersion);
                    AssetDatabase.Refresh();
                });
            }
        }

        internal static void WarnOutdated(string newerVersion)
        {
            if (EditorUtility.DisplayDialog("Asset message", string.Format("Newer version '{0}' is available. You are using '{1}'.\nDo you want to update the DLL API?", 
                newerVersion, curVersion), 
                "Yes", "No"))
                DoUpdate(newerVersion);
        }

        [MenuItem("Lerp2Dev Team Tools/Check for API Updates...", false, 1000)]
        public static void CheckForUpdates()
        {
            UpdateCheck(true);
        }
        internal static void UpdateCheck(bool successEnabled = false)
        {
            if (!File.Exists(localVersionFilepath))
                JSONHelpers.SerializeToFile(localVersionFilepath, new LerpedUpdater(), true);
            if (!noConnection)
                try
                {
                    WWW www = new WWW("https://raw.githubusercontent.com/Lerp2Dev/Lerp2API/master/Lerp2API.version");
                    wh = new WWWHandler();
                    wh.Add(www);
                    wh.Start<WWW>(false, (x) =>
                    {
                        var updater = x.text.Deserialize<LerpedUpdater>();
                        if (updater.versionStr != curVersion)
                            WarnOutdated(updater.versionStr);
                        else
                        {
                            if (successEnabled)
                                Debug.Log("Lerp2API is up-to-date!");
                        }
                    });
                }
                catch(Exception e)
                {
                    Debug.LogError("Internet connection couldn't be detected, Updates are disabled!\nMaybe it can be another problem, check the log by clicking this message.\n"+e.Message+"\n"+e.StackTrace);
                    noConnection = true;
                }
            else
                CheckForConnection();
        }
        internal static void DefineCheck()
        {
            AssetDefineManager.AddCompileDefine("LERP2API", LerpedEditorCore.LerpedBuildTarget);
            AssetDefineManager.AddCompileDefine("LERP2APIEDITOR", LerpedEditorCore.LerpedBuildTarget);
        }
        internal static void GetMissingAssets()
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MissingAssets.cs");
            if (File.Exists(path))
                return;
            WWW www = new WWW("http://lerp2dev.x10host.com/unityassets/"); //We have to keep this url and the proper hosting (to give support to the upcoming users of the API) until we don't launch the new web design with all this data updated...
            wh = new WWWHandler();
            wh.Add(www);
            wh.Start<WWW>(false, (x) => {
                File.WriteAllText(path, x.text);
                AssetDatabase.Refresh();
            });
        }
    }
}