using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Net;
using Lerp2API;
using Lerp2APIEditor.Utils;
using Debug = Lerp2API.DebugHandler.Debug;
using System.Collections;

namespace L2APIEditor
{
    [InitializeOnLoad]
    public class LerpedUpdater
    {
        internal const string curVersion = "1.0.1a",
                              versionUrl = "http://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Lerp2API.version";

        internal static string localVersionFilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Lerp2API.version");
        internal static bool noConnection;
        internal static string[] updateUrl = new string[] { "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Lerp2API.dll", "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Lerp2API.pdb" };

        public string versionName = "1.0.1 Alpha Release",
                      versionStr = "1.0.1a",
                      versionChangelog = "Splitted the dll into Editor and Game part.\nAdded internal XML documentation file.\nCleaned some unused references.\nAdded some more utilities to the repo.\nFixed some errors in Editor.";

        static LerpedUpdater()
        {
            UpdateCheck();
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
            string location = Assembly.GetExecutingAssembly().Location;
            string path = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".pdb");
            WWW www = new WWW(updateUrl[0]);
            WWW www2 = new WWW(updateUrl[1]);
            if (!File.Exists(location) || !File.Exists(path))
                Debug.LogErrorFormat("Some Library files doesn't exist, aborting update mission!\nDownload them manually from {0}, and put them in '{1}' folder.", new object[] { @"<a href=""https://github.com/Ikillnukes/Lerp2API/tree/master/Build"">here</a>", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) });
            else
            {
                WWWHandler.Handle(www, () => 
                {
                    File.Delete(location);
                    File.WriteAllBytes(location, www.bytes);
                });
                WWWHandler.Handle(www2, () => 
                {
                    File.Delete(path);
                    File.WriteAllBytes(path, www2.bytes);
                });
                Debug.LogFormat("Update to '{0}' successfully done!", new object[] { newerVersion });
                AssetDatabase.Refresh();
            }
        }

        internal static void WarnOutdated(string newerVersion)
        {
            if (EditorUtility.DisplayDialog("Asset message", string.Format("Newer version '{0}' is available. You are using '{1}'.\nDo you want to update the DLL API?", newerVersion, "1.0a"), "Yes", "No"))
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
    }
}
