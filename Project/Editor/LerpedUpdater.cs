using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Net;
using Lerp2API;
using Lerp2API.Utility;
using Lerp2APIEditor.Utility;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2APIEditor
{
    [InitializeOnLoad]
    public class LerpedUpdater
    {
        internal const string curVersion = "1.1a",
                              versionUrl = "http://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Lerp2API.version";

        internal static string localVersionFilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Lerp2API.version");
        public static bool noConnection;
        internal static string[] updateUrl = new string[] {
            "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Lerp2API.dll",
            "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Lerp2API.pdb",
            "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Lerp2API.xml" };

        public string versionName = "1.1 Alpha Release",
                      versionStr = "1.1a",
                      versionChangelog = "Deleted Unity Analytics popup.\nImproved Lerped Updater (I added the XML file to download, and I made several optimizations to it).\nFixed problem with Debug Manager's editor menu option.\nAdded Debug Manager to the game.\nNow there is a working console (the unique command is /debug, the shortcut to this command is the key 'P').\nImproved WWW System.\nThe Falling Avoider has been fixed (it has a minor bug, that had not detect the Start event of MonoBehaviour) and improved.\nAdded support to unistalled Assets by creating two new options 'Build Scene' & 'Init Asset', if the asset is not present the Editor will show you a message to download/buy it!\nIf the API is not active in the Editor, the DownloadAPI class will help it you to promote it.\nGame Console simple format system.\nAdded shortcuts to editor.\nUtility to get Skin from Editor, Inspector and Game.\nAdded some commands.\nAdded a File Browser Utility.\nAdded custom inspector draws for some classes.\nImproved GUI from console.\nUpdate and compile external files from the API Project to update changes.";

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
            string location = Assembly.GetExecutingAssembly().Location,
                   path = location.Replace(".dll", ".pdb"),
                   path1 = location.Replace(".dll", ".xml");
            WWW www1 = new WWW(updateUrl[0]);
            WWW www2 = new WWW(updateUrl[1]);
            WWW www3 = new WWW(updateUrl[2]);
            if (!File.Exists(location) || !File.Exists(path))
                Debug.LogErrorFormat("Some Library files doesn't exist, aborting update mission!\nDownload them manually from {0}, and put them in '{1}' folder.", 
                    @"<a href=""https://github.com/Ikillnukes/Lerp2API/tree/master/Build"">here</a>", 
                    Path.GetDirectoryName(location));
            else
            {
                WWWHandler.Handle(www1, () => 
                {
                    File.Delete(location);
                    File.WriteAllBytes(location, www1.bytes);
                    WWWHandler.Handle(www2, () =>
                    {
                        File.Delete(path);
                        File.WriteAllBytes(path, www2.bytes);
                        WWWHandler.Handle(www3, () =>
                        {
                            File.Delete(path1);
                            File.WriteAllBytes(path1, www3.bytes);
                            Debug.LogFormat("Update to '{0}' successfully done!", newerVersion);
                            AssetDatabase.Refresh();
                        });
                    });
                });
                /*WWWHandler.Add(www1);
                WWWHandler.Add(www2);
                WWWHandler.Add(www3);
                WWWHandler.Start(() => {
                    File.Delete(location);
                    File.WriteAllBytes(location, www1.bytes);
                    File.Delete(path);
                    File.WriteAllBytes(path, www2.bytes);
                    File.Delete(path1);
                    File.WriteAllBytes(path1, www3.bytes);
                    Debug.LogFormat("Update to '{0}' successfully done!", newerVersion);
                    AssetDatabase.Refresh();
                });*/
            }
        }

        internal static void WarnOutdated(string newerVersion)
        {
            if (EditorUtility.DisplayDialog("Asset message", string.Format("Newer version '{0}' is available. You are using '{1}'.\nDo you want to update the DLL API?", 
                newerVersion, "1.0a"), 
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
                    WWW www = new WWW("https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Lerp2API.version");
                    WWWHandler.Handle(www, () =>
                    {
                        var updater = www.text.Deserialize<LerpedUpdater>();
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
                    Debug.LogError("Internet connection couldn't be detected, Updates are disabled!\n"+e.Message);
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
            WWW www = new WWW("http://lerp2dev.com/unityassets/");
            WWWHandler.Handle(www, () => {
                File.WriteAllText(path, www.text);
                AssetDatabase.Refresh();
            });
        }
    }
}
