using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Lerp2API
{
    public class LerpedCore : MonoBehaviour
    {
        private static Dictionary<string, object> _storedInfo;
        private static string storePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "L2API.sav");

        public static float UnityTick
        {
            get
            {
                return Time.fixedDeltaTime;
            }
        }

        public static bool CheckUnityVersion(int mainVer, int subVer)
        {
            char[] separator = new char[] { '.' };
            int[] numArray = Application.unityVersion.Split(separator).Select(x => GetVersionValue(x)).ToArray();
            return ((mainVer < numArray[0]) || ((mainVer == numArray[0]) && (subVer <= numArray[1])));
        }

        private static int GetVersionValue(string strNumber)
        {
            int result = 0;
            int.TryParse(strNumber, out result);
            return result;
        }

        public static bool GetBool(string key)
        {
            return storedInfo.ContainsKey(key) && ((bool)storedInfo[key]);
        }

        public static void SetBool(string key, bool value)
        {
            if (!storedInfo.ContainsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        public static string GetString(string key)
        {
            if (storedInfo.ContainsKey(key))
                return (string)storedInfo[key];
            else
                return "";
        }

        public static void SetString(string key, string value)
        {
            if (!storedInfo.ContainsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        public static int GetInt(string key)
        {
            if (storedInfo.ContainsKey(key))
                return (int)storedInfo[key];
            else
                return 0;
        }

        public static void SetInt(string key, int value)
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
}