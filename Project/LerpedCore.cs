using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HtmlAgilityPack;
using Debug = Lerp2API.DebugHandler.Debug;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Lerp2API
{
    public class LerpedCore
    {
        private static Dictionary<string, object> _storedInfo;
        private static string storePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "L2API.sav");

        public static bool CheckUnityVersion(int mainVer, int subVer)
        {
            char[] separator = new char[] { '.' };
            int[] numArray = Application.unityVersion.Split(separator).Select(x => GetVersionValue(x)).ToArray();
            return ((mainVer < numArray[0]) || ((mainVer == numArray[0]) && (subVer <= numArray[1])));
        }

        public static bool GetBool(string key)
        {
            return (storedInfo.ContainsKey(key) && ((bool)storedInfo[key]));
        }

        private static int GetVersionValue(string strNumber)
        {
            int result = 0;
            int.TryParse(strNumber, out result);
            return result;
        }

        public static void SetBool(string key, bool value)
        {
            if (!storedInfo.ContainsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        private static Dictionary<string, object> storedInfo
        {
            get
            {
                if (_storedInfo == null)
                    if (File.Exists(storePath))
                        _storedInfo = JSONHelpers.DeserializeFromFile<Dictionary<string, object>>(storePath);
                    else
                        _storedInfo = new Dictionary<string, object>();
                return _storedInfo;
            }
            set
            {
                _storedInfo = value;
            }
        }
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class LerpedUpdater
    {
        internal const string curVersion = "1.0a",
                              versionUrl = "http://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Lerp2API.version";

        internal static string localVersionFilepath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Lerp2API.version");
        internal static bool noConnection;
        internal static string[] updateUrl = new string[] { "http://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Lerp2API.dll", "https://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Build/Lerp2API.pdb" };

        public string versionName = "1.0 Alpha Release",
                      versionStr = "1.0a",
                      versionChangelog = "Initial release.";

        static LerpedUpdater()
        {
            if (!File.Exists(localVersionFilepath))
                JSONHelpers.SerializeToFile(localVersionFilepath, new LerpedUpdater(), true);
            if (!noConnection)
                try
                {
                    HtmlWeb web = new HtmlWeb();
                    LerpedUpdater updater = web.Load("http://raw.githubusercontent.com/Ikillnukes/Lerp2API/master/Lerp2API.version").DocumentNode.OuterHtml.Deserialize<LerpedUpdater>();
                    if (updater.versionStr != "1.0a")
                        WarnOutdated(updater.versionStr);
                }
                catch
                {
                    Debug.LogError("Internet connection couldn't be detected, Updates are disabled!");
                    noConnection = true;
                }
            else
                CheckForConnection();
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
                File.Delete(location);
                File.Delete(path);
                File.WriteAllBytes(location, www.bytes);
                File.WriteAllBytes(path, www2.bytes);
                Debug.LogFormat("Update to '{0}' successfully done!", new object[] { newerVersion });
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
        }

        internal static void WarnOutdated(string newerVersion)
        {
            if (EditorUtility.DisplayDialog("Asset message", string.Format("Newer version '{0}' is available. You are using '{1}'.\nDo you want to update the DLL API?", newerVersion, "1.0a"), "Yes", "No"))
                DoUpdate(newerVersion);
        }
    }
}