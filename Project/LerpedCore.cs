using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2API
{
    public class LerpedCore : MonoBehaviour
    {
        private static Dictionary<string, object> _storedInfo;
        private static string storePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "L2API.sav");
        public const string UnityBoot = "UNITY_STARTED_UP", enabledDebug = "ENABLE_DEBUG", loggerPath = "LOG_PATH", 
                            defaultLogFilePath = "Logs/Console.log";

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

        public static long GetLong(string key)
        {
            if (storedInfo.ContainsKey(key))
                return (long)storedInfo[key];
            else
                return 0;
        }

        public static void SetLong(string key, long value)
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

    public class ConsoleListener
    {
        public static void StartConsole()
        {
            string console = Path.Combine(Application.dataPath, "/Lerp2API/Console/Lerp2Console.exe");
            if (!File.Exists(console))
                return;
            var process = new Process
            {
                StartInfo =
                {
                    FileName = console,
                    Arguments = string.Format(@"-projectPath={0}{1}", Application.dataPath, Application.isEditor ? " -editor" : "")
                }
            };
            process.Start();
        }
    }

    public class ConsoleSender
    {
        private static List<string> paths = new List<string>();
        public static void SendMessage(string path, LogType lt, string ls, string st)
        {
            if (!paths.Contains(path))
                paths.Add(path);
            File.AppendAllText(path, Environment.NewLine + JsonUtility.ToJson(new ConsoleMessage(lt, ls, st)));
        }
        public static void Quit()
        {
            //Tengo q comprimir el log... Todo esto cuando tenga una carpeta q se llame logs y este codigo este dentro de la api
            //Al tener la api ya no me va a hacer falta tener q estar diciendole q se ejecute todos los eventos, ni los archivos definidos en otros scripts ni nada
            //Ni tampoco estar diciendole a esta clase donde esta el ejecutable pq debe ir siempre acompañado de la api al tenerlo como referencia dentro de si
            //Y todo estar mas compactado tanto las clases como los metodos...
            foreach (string p in paths)
                if (File.Exists(p))
                    File.Delete(p);
        }
    }

    public class ConsoleMessage
    {
        public LogType logType;
        public string logString, stackTrace;
        public ConsoleMessage(LogType lt, string ls, string st)
        {
            logType = lt;
            logString = ls;
            stackTrace = st;
        }
    }
}