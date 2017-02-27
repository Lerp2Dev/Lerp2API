using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FullSerializer;
using Object = UnityEngine.Object;
using Debug = Lerp2API.DebugHandler.Debug;
using Color = Lerp2API.Optimizers.Color;
using Lerp2API.Game;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using Lerp2API.Optimizers;
using System.Reflection;
using UnityEngine.Assertions;

namespace Lerp2API
{
    public enum Position { UpperLeft, UpperRight, BottomLeft, BottomRight }
    public static class Helpers
    {
        #region "String Extensions"

        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string ReplaceLast(this string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }

        public static bool IsEmptyOrWhiteSpace(this string value)
        {
            return value.All(char.IsWhiteSpace);
        }

        public static string ReplaceAt(this string str, int index, int length, string replace)
        {
            return str.Remove(index, Math.Min(length, str.Length - index))
                    .Insert(index, replace);
        }

        #endregion "String Extensions"

        #region "Texture Extensions"

        public static Texture2D ToTexture(this UnityEngine.Color c, int w = 1, int h = 1)
        {
            if (w < 1 || h < 1)
                throw new Exception("This method doesn't avoid zero or negative dimensions.");
            Texture2D t = new Texture2D(w, h);
            if (w > 1 && h > 1)
                for (int i = 0; i < w; ++i)
                    for (int j = 0; j < h; ++j)
                        t.SetPixel(i, j, c);
            else if (w > 1 && h == 1)
                for (int i = 0; i < w; ++i)
                    t.SetPixel(i, 0, c);
            else if (w == 1 && h > 1)
                for (int j = 0; j < h; ++j)
                    t.SetPixel(0, j, c);
            else
                t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }

        #endregion "Texture Extensions"

        #region "Array Extensions"

        public static T[] Push<T>(this T[] array, T item)
        {
            List<T> l = new List<T>();
            if (array != null)
                l = array.ToList();
            l.Add(item);
            array = l.ToArray();
            return array;
        }

        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            List<T> l = array.ToList();
            l.RemoveAt(index);
            array = l.ToArray();
            return array;
        }

        public static T[] Merge<T>(params object[] arrays)
        {
            List<T> arr = new List<T>();
            foreach (object t in arrays)
                if (t is Array)
                    foreach (object st in (Array)t)
                        arr.Add((T)st);
                else
                    arr.Add((T)t);
            return arr.ToArray();
        }

        public static WWW[] GetWWW(this string[] a)
        {
            List<WWW> w = new List<WWW>();
            foreach (string p in a)
                w.Add(new WWW(p));
            return w.ToArray();
        }

        #endregion "Array Extensions"

        #region "Dictionary Extensions"

        public static Dictionary<TKey, TValue> RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
                                          TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            if (!dic.ContainsKey(toKey))
                dic[toKey] = value;
            else
            {
                TValue v = dic[toKey];
                dic.Remove(toKey);
                dic[fromKey] = v;
                dic[toKey] = value;
            }
            return (Dictionary<TKey, TValue>)dic;
        }

        #endregion "Dictionary Extensions"

        #region "Iteration Extensions"

        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
                action(i);
        }

        #endregion "Iteration Extensions"

        #region "IO Extensions"

        public static void DeleteDirectory(this string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
                DeleteDirectory(dir);

            Directory.Delete(target_dir, false);
        }

        public static long LatestModification(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            List<long> dates = new List<long>();
            foreach (string f in files)
                dates.Add(File.GetAttributes(f) == FileAttributes.Directory ? Directory.GetCreationTime(f).ToEpoch() : File.GetCreationTime(f).ToEpoch());
            if (files != null)
                return dates.Max();
            else
                return -1;
        }

        #endregion "IO Extensions"

        #region "GameObject Extensions"

        public static GameObject getRoot(this GameObject go)
        {
            Transform t = go.transform;
            while (t.parent != null)
                t = t.parent.transform;
            return t.gameObject;
        }

        #endregion "GameObject Extensions"

        #region "Object Extensions"

        public static string ToUString(this object message) //To Universal String, converts an object to string by checking if it's a string...
        {
            return message.GetType().Equals(typeof(string)) ? (string)message : message.ToString();
        }

        #endregion "Object Extensions"

        #region "Color Extensions"

        public static string ColorToHex(this UnityEngine.Color color) 
        {
            return ColorToHex((Color32)color);
        }

        public static string ColorToHex(this Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }
         
        public static UnityEngine.Color HexToColor(this string hex)
        {
            byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber),
                 g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber),
                 b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        #endregion

        #region "Action Extensions"

        public static Action<T2> Convert<T1, T2>(this Action<T1> myActionT)
        {
            if (myActionT == null) return null;
            else return new Action<T2>(o => myActionT((T1)(object)o)); //Doesn't convert correctly types
        }

        #endregion

        #region "IEnumerable Extensions"

        public static T Clone<T>(this T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item) { return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i)); }

        #endregion

        #region "Misc Extensions"

        public static string UnpackNl(string str)
        {
            return str.Replace("\\n", Environment.NewLine);
        }

        public static string PackNl(string str)
        {
            return str.Replace(Environment.NewLine, "\\n");
        }

        #endregion

    }

    #region "JSON Helpers"

    public static class JSONHelpers
    {
        private static readonly fsSerializer _serializer = new fsSerializer();

        public static string Serialize<T>(this T value, bool pretty = true)
        {
            fsData data;
            fsResult res = _serializer.TrySerialize(typeof(T), value, out data).AssertSuccessWithoutWarnings();

            if (res.Failed)
                Debug.LogError(res.FormattedMessages);

            if (pretty)
                return fsJsonPrinter.PrettyJson(data);
            return fsJsonPrinter.CompressedJson(data);
        }

        public static T Deserialize<T>(this string serializedState)
        {
            fsData data = fsJsonParser.Parse(serializedState);

            object deserialized = null;
            _serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccessWithoutWarnings();

            return (T)deserialized;
        }

        public static void SerializeToFile<T>(string path, T value, bool pretty = true)
        {
            File.WriteAllText(path, Serialize(value, pretty));
        }

        public static T DeserializeFromFile<T>(string path)
        {
            return Deserialize<T>(File.ReadAllText(path));
        }
    }

    #endregion "JSON Helpers"

    #region "Serializer Helpers"

    public static class SerializerHelpers
    {
        public class Transporter
        {
            public Type type;
            public bool write;
            public object obj;

            public Transporter(object o, bool w = true)
            {
                type = o.GetType();
                obj = o;
            }
        }

        public static string SaveAsAsset<T>(this string str, T obj, string name = "", bool w = true)
        {
            return SaveAsAsset(new Transporter(str, w), obj, name);
        }

        public static string SaveAsAsset<T>(this string[] strs, T obj, string name = "")
        {
            return SaveAsAsset(new Transporter(strs), obj, name);
        }

        public static string SaveAsAsset<T>(this byte[] arr, T obj, string name = "")
        {
            return SaveAsAsset(new Transporter(arr), obj, name);
        }

        public static string SaveAsAsset<T>(Transporter value, T obj, string name)
        {
            bool emptyName = string.IsNullOrEmpty(name),
                 derivedClass = obj != null && typeof(T).IsSubclassOf(typeof(Object)), alreadyExists = false;
            string n = name, path = "", fpath = "", rpath = "";
            if (emptyName && derivedClass)
                n = ((Object)(object)obj).name;
            else if (emptyName && !derivedClass)
                n = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds.ToString("F0");
            path = Path.Combine(Application.streamingAssetsPath, typeof(T).Name);
            fpath = Path.Combine(path, n + ".asset");
            rpath = typeof(T).Name + "/" + n + ".asset";
            alreadyExists = File.Exists(fpath);
            if (!alreadyExists)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (value.type.Equals(typeof(string)))
                    if (value.write)
                        File.WriteAllText(fpath, (string)value.obj);
                    else
                        File.AppendAllText(fpath, (string)value.obj + Environment.NewLine);
                else if (value.type.Equals(typeof(string[])))
                    File.WriteAllLines(fpath, (string[])value.obj);
                else if (value.type.Equals(typeof(byte[])))
                    File.WriteAllBytes(fpath, (byte[])value.obj);
                else
                    throw new Exception("Not recognised type!");
                Debug.Log("Asset saved at " + fpath + "!");
            }
            if (typeof(T).Equals(typeof(GameObject)))
            {
                //if (Application.isPlaying)
                //    Object.Destroy(GameObject.Find(((GameObject)(object)obj).name));
                //else
                if (Application.isEditor)
                    Object.DestroyImmediate(GameObject.Find(((GameObject)(object)obj).name));
            }
            return rpath;
        }

        public static byte[] LoadAsset(string fpath)
        {
            string p = Path.Combine(Application.streamingAssetsPath, fpath);
            if (!File.Exists(p))
            {
                Debug.LogError("Any file found at " + p + "!");
                return null;
            }
            return File.ReadAllBytes(p);
        }
    }

    public partial class PluginsHelper
    {
        public static Dictionary<string, GameObject> objList = new Dictionary<string, GameObject>();

        public static void Add(GameObject go)
        {
            GameObject root = go.getRoot();
            if (!objList.ContainsKey(root.name))
                objList.Add(root.name, root);
        }

        public static MeshFilter FindMeshFilter(GameObject go, ref GameObject foundGo)
        {
            if (go == null)
            {
                foundGo = null;
                return null;
            }
            if (go.GetComponent<MeshFilter>() != null)
            {
                foundGo = go;
                return go.GetComponent<MeshFilter>();
            }
            foreach (Transform child in go.transform)
            {
                MeshFilter mf = FindMeshFilter(child.gameObject, ref foundGo);
                if (mf != null)
                {
                    foundGo = child.gameObject;
                    return mf;
                }
                else
                    continue;
            }
            return null;
        }

        public static SkinnedMeshRenderer FindSkinnedMeshRenderer(GameObject go, ref GameObject foundGo)
        {
            if (go == null)
            {
                foundGo = null;
                return null;
            }
            if (go.GetComponent<SkinnedMeshRenderer>() != null)
            {
                foundGo = go;
                return go.GetComponent<SkinnedMeshRenderer>();
            }
            foreach (Transform child in go.transform)
            {
                SkinnedMeshRenderer mr = FindSkinnedMeshRenderer(child.gameObject, ref foundGo);
                if (mr != null)
                {
                    foundGo = child.gameObject;
                    return mr;
                }
                else
                    continue;
            }
            return null;
        }

        public static Mesh FindMesh(GameObject obj, ref GameObject foundGo)
        {
            MeshFilter mf = FindMeshFilter(obj, ref foundGo);
            SkinnedMeshRenderer smr = FindSkinnedMeshRenderer(obj, ref foundGo);
            if (mf != null)
            {
                if (Application.isPlaying)
                    return mf.mesh;
                else
                    return mf.sharedMesh;
            }
            else if (smr != null)
                return smr.sharedMesh;
            else
                return null;
        }
    }

    #endregion "Serializer Helpers"

    #region "Rect Helpers"

    public static class RectHelpers
    {
        public static Rect GetPosition(this Position pos, int w, int h, int mh = 5, int mv = 5)
        {
            switch(pos)
            {
                case Position.UpperLeft:
                    return new Rect(mh, mv, w, h);
                case Position.UpperRight:
                    return new Rect(Screen.width - w - mh, mv, w, h);
                case Position.BottomLeft:
                    return new Rect(mh, Screen.height - h - mv, w, h);
                case Position.BottomRight:
                    return new Rect(Screen.width - w - mh, Screen.height - h - mv, w, h);
                default:
                    return new Rect(mh, mv, w, h);
            }
        }
    }

    #endregion

}

//This sould be in separate file

#region "Debug Handler & Game Console"

namespace Lerp2API.DebugHandler
{ //using Debug = Lerp2API.DebugHandler.Debug;
    public class Debug : GameConsole
    {
        private static ConsoleSender sender;
        public static bool isEnabled
        {
            get
            {
                return GetBool(enabledDebug);
            }
        }

        /*private static string _logPath;
        private static string logPath
        {
            get
            {
                if (string.IsNullOrEmpty(_logPath))
                    _logPath = GetString(loggerPath);
                if(string.IsNullOrEmpty(_logPath))
                {
                    _logPath = Path.Combine(Application.dataPath, defaultLogFilePath);
                    SetString(loggerPath, _logPath);
                }
                return _logPath;
            }
        }*/

        public static void HookLog(string path)
        {
            //UnityEngine.Debug.Log("Hooking debug!");
            Application.logMessageReceived += LogToFile;
            sender = new ConsoleSender(path);
        }

        public static void UnhookLog()
        {
            Application.logMessageReceived -= LogToFile;
        }

        public static bool isGamingEnabled {get; set;}
        public static bool developerConsoleVisible
        {
            get { return UnityEngine.Debug.developerConsoleVisible; }
            set { UnityEngine.Debug.developerConsoleVisible = value; }
        }

        public static bool isDebugBuild
        {
            get { return UnityEngine.Debug.isDebugBuild; }
        }

        public static ILogger logger
        {
            get { return UnityEngine.Debug.logger; }
        }

        public static void Assert(bool condition)
        {
            Assert(condition, "", null, false);
        }

        public static void Assert(bool condition, string message)
        {
            Assert(condition, message, null, true);
        }

        public static void Assert(bool condition, object message)
        {
            Assert(condition, message, null, false);
        }

        public static void Assert(bool condition, string message, Object context)
        {
            Assert(condition, message, context, true);
        }

        public static void Assert(bool condition, object message, Object context, bool str = false)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.Assert(condition, str ? message : (string)message, context);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(message.ToUString(), DebugColor.assert);
            }
        }

        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.AssertFormat(condition, format, args);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.assert);
            }
        }

        public static void Break()
        {
            if (isEnabled)
                UnityEngine.Debug.Break();
        }

        public static void ClearDeveloperConsole()
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.ClearDeveloperConsole();
                if (Application.isPlaying && isGamingEnabled)
                    Clear();
            }
        }

        public static void DrawLine(Vector3 start, Vector3 end, UnityEngine.Color color = default(UnityEngine.Color), float duration = 0.0f, float width = 1.0f, bool depthTest = true)
        {
            if (color.Equals(default(UnityEngine.Color)))
                color = DebugColor.normal;
            if (isEnabled)
            {
                if (Application.isEditor && Application.isPlaying)
                    UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
                if (Application.isPlaying && isGamingEnabled)
                    DebugLine.DrawLine(start, end, color, duration, width);
            }
        }

        public static void DrawRay(Vector3 start, Vector3 dir, UnityEngine.Color color = default(UnityEngine.Color), float duration = 0.0f, float width = 1.0f, bool depthTest = true)
        {
            if (color.Equals(default(UnityEngine.Color)))
                color = DebugColor.normal;
            if (isEnabled)
            {
                if (Application.isEditor && Application.isPlaying)
                    UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
                if (Application.isPlaying && isGamingEnabled)
                    DebugLine.DrawRay(start, dir, color, duration, width);
            }
        }

        public static void LogAssertion(object message, Object context = null)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    if (context != null)
                        UnityEngine.Debug.LogAssertion(message, context);
                    else
                        UnityEngine.Debug.LogAssertion(message);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(message.ToUString(), DebugColor.assertion);
            }
        }

        public static void LogAssertionFormat(string message)
        {
            LogAssertionFormat(message, null);
        }

        public static void LogAssertionFormat(string message, params object[] args)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.LogAssertionFormat(message, args);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(message, DebugColor.assertion);
            }
        }

        public static void Log(string message)
        {
            Log((object)message);
        }

        public static void Log(object message)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.Log(message);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(message.ToUString(), DebugColor.normal);
            }
        }

        public static void LogFormat(string format, params object[] args)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.LogFormat(format, args);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.normal);
            }
        }

        public static void LogFormat(Object context, string format, params object[] args)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.LogFormat(context, format, args);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.normal);
            }
        }

        public static void LogWarning(string message)
        {
            LogWarning((object)message);
        }

        public static void LogWarning(object message)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.LogWarning(message);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(message.ToUString(), DebugColor.warning);
            }
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.LogWarningFormat(format, args);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.warning);
            }
        }

        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.LogWarningFormat(context, format, args);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.warning);
            }
        }

        public static void LogError(string message)
        {
            LogError((object)message);
        }

        public static void LogError(object message)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.LogError(message);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(message.ToUString(), DebugColor.error);
            }
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.LogErrorFormat(format, args);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.error);
            }
        }

        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.LogErrorFormat(context, format, args);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.error);
            }
        }

        public static void LogException(Exception exception, Object context = null)
        {
            if (isEnabled)
            {
                if (Application.isEditor)
                    UnityEngine.Debug.LogException(exception, context);
                if (Application.isPlaying && isGamingEnabled)
                    AddFormattedMessage(exception.Message, DebugColor.exception);
            }
        }

        internal static void LogToFile(string logString, string stackTrace, LogType type)
        {
            sender.SendMessage(type, logString, stackTrace);
        }

        public static void DrawCube(Vector3 pos, UnityEngine.Color col, Vector3 scale)
        {
            if (isEnabled)
            {
                Vector3 halfScale = scale * 0.5f;

                Vector3[] points = new Vector3[]
                {
                    pos + new Vector3(halfScale.x,      halfScale.y,    halfScale.z),
                    pos + new Vector3(-halfScale.x,     halfScale.y,    halfScale.z),
                    pos + new Vector3(-halfScale.x,     -halfScale.y,   halfScale.z),
                    pos + new Vector3(halfScale.x,      -halfScale.y,   halfScale.z),
                    pos + new Vector3(halfScale.x,      halfScale.y,    -halfScale.z),
                    pos + new Vector3(-halfScale.x,     halfScale.y,    -halfScale.z),
                    pos + new Vector3(-halfScale.x,     -halfScale.y,   -halfScale.z),
                    pos + new Vector3(halfScale.x,      -halfScale.y,   -halfScale.z)
                };

                DrawLine(points[0], points[1], col);
                DrawLine(points[1], points[2], col);
                DrawLine(points[2], points[3], col);
                DrawLine(points[3], points[0], col);
            }
        }

        public static void DrawRect(Rect rect, UnityEngine.Color col)
        {
            if (isEnabled)
            {
                Vector3 pos = new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 0.0f);
                Vector3 scale = new Vector3(rect.width, rect.height, 0.0f);

                DrawRect(pos, col, scale);
            }
        }

        public static void DrawRect(Vector3 pos, UnityEngine.Color col, Vector3 scale)
        {
            if (isEnabled)
            {
                Vector3 halfScale = scale * 0.5f;

                Vector3[] points = new Vector3[]
                {
                    pos + new Vector3(halfScale.x,      halfScale.y,    halfScale.z),
                    pos + new Vector3(-halfScale.x,     halfScale.y,    halfScale.z),
                    pos + new Vector3(-halfScale.x,     -halfScale.y,   halfScale.z),
                    pos + new Vector3(halfScale.x,      -halfScale.y,   halfScale.z)
                };

                DrawLine(points[0], points[1], col);
                DrawLine(points[1], points[2], col);
                DrawLine(points[2], points[3], col);
                DrawLine(points[3], points[0], col);
            }
        }

        public static void DrawPoint(Vector3 pos, UnityEngine.Color col, float scale)
        {
            if (isEnabled)
            {
                Vector3[] points = new Vector3[]
                {
                    pos + (Vector3.up * scale),
                    pos - (Vector3.up * scale),
                    pos + (Vector3.right * scale),
                    pos - (Vector3.right * scale),
                    pos + (Vector3.forward * scale),
                    pos - (Vector3.forward * scale)
                };

                DrawLine(points[0], points[1], col);
                DrawLine(points[2], points[3], col);
                DrawLine(points[4], points[5], col);

                DrawLine(points[0], points[2], col);
                DrawLine(points[0], points[3], col);
                DrawLine(points[0], points[4], col);
                DrawLine(points[0], points[5], col);

                DrawLine(points[1], points[2], col);
                DrawLine(points[1], points[3], col);
                DrawLine(points[1], points[4], col);
                DrawLine(points[1], points[5], col);

                DrawLine(points[4], points[2], col);
                DrawLine(points[4], points[3], col);
                DrawLine(points[5], points[2], col);
                DrawLine(points[5], points[3], col);
            }
        }
    }
}

public class DebugColor
{
    public static Color normal
    {
        get { return new Color(1, 1, 1); }
    }

    public static Color warning
    {
        get { return new Color(1, 1, 0); }
    }

    public static Color error
    {
        get { return new Color(1, 0, 0); }
    }

    public static Color assert
    {
        get { return new Color(1, 1, 1); }
    }

    public static Color assertion
    {
        get { return new Color(0, 1, 1); }
    }

    public static Color exception
    {
        get { return new Color(1, 0, 0); }
    }
}

#endregion "Debug Handler & Game Console"

#region "GUI Utils"

public static class ShadowAndOutline
{
        public static void DrawOutline(Rect rect, string text, GUIStyle style, UnityEngine.Color outColor, UnityEngine.Color inColor, float size)
        {
            float halfSize = size * 0.5F;
            GUIStyle backupStyle = new GUIStyle(style);
            UnityEngine.Color backupColor = GUI.color;
 
            style.normal.textColor = outColor;
            GUI.color = outColor;
 
            rect.x -= halfSize;
            GUI.Label(rect, text, style);
 
            rect.x += size;
            GUI.Label(rect, text, style);
 
            rect.x -= halfSize;
            rect.y -= halfSize;
            GUI.Label(rect, text, style);
 
            rect.y += size;
            GUI.Label(rect, text, style);
 
            rect.y -= halfSize;
            style.normal.textColor = inColor;
            GUI.color = backupColor;
            GUI.Label(rect, text, style);
 
            style = backupStyle;
        }

        public static void DrawShadow(Rect rect, GUIContent content, GUIStyle style, UnityEngine.Color txtColor, UnityEngine.Color shadowColor,
                                        Vector2 direction)
        {

            rect.x += direction.x;
            rect.y += direction.y;
            GUI.Label(rect, content, new GUIStyle(style) { normal = new GUIStyleState() { textColor = shadowColor } });
 
            rect.x -= direction.x;
            rect.y -= direction.y;
            GUI.Label(rect, content, new GUIStyle(style) { normal = new GUIStyleState() { textColor = txtColor } });
        }

        public static void DrawLayoutOutline(string text, GUIStyle style, UnityEngine.Color outColor, UnityEngine.Color inColor, float size, params GUILayoutOption[] options)
        {
            DrawOutline(GUILayoutUtility.GetRect(new GUIContent(text), style, options), text, style, outColor, inColor, size);
        }

        public static void DrawLayoutShadow(GUIContent content, GUIStyle style, UnityEngine.Color txtColor, UnityEngine.Color shadowColor,
                                        Vector2 direction, params GUILayoutOption[] options)
        {
            DrawShadow(GUILayoutUtility.GetRect(content, style, options), content, style, txtColor, shadowColor, direction);
        }
 
        public static bool DrawButtonWithShadow(Rect r, GUIContent content, GUIStyle style, float shadowAlpha, Vector2 direction)
        {
            GUIStyle letters = new GUIStyle(style);
            letters.normal.background = null;
            letters.hover.background = null;
            letters.active.background = null;
 
            bool result = GUI.Button(r, content, style);

            UnityEngine.Color color = r.Contains(Event.current.mousePosition) ? letters.hover.textColor : letters.normal.textColor;
 
            DrawShadow(r, content, letters, color, new UnityEngine.Color(0f, 0f, 0f, shadowAlpha), direction);
 
            return result;
        }
 
        public static bool DrawLayoutButtonWithShadow(GUIContent content, GUIStyle style, float shadowAlpha,
                                                       Vector2 direction, params GUILayoutOption[] options)
        {
            return DrawButtonWithShadow(GUILayoutUtility.GetRect(content, style, options), content, style, shadowAlpha, direction);
        }
}

#endregion

#region "Color Utils"

public static class ColorHelpers
{
    public static Color[,] GetBixels(this Texture2D t)
    {
        int w = t.width, h = t.height;
        Color[,] cs = new Color[w, h];
        for (int i = 0; i < w; ++i)
            for (int j = 0; j < h; ++j)
                cs[i, j] = (Color)t.GetPixel(i, h - j - 1); //c[i + j * w];
        return cs;
    }
    public static UnityEngine.Color[] GetColor(this Color[,] c, int w, int h)
    {
        UnityEngine.Color[] cs = new UnityEngine.Color[w * h];
        for (int i = 0; i < w; ++i)
            for (int j = 0; j < h; ++j)
                cs[i + j * w] = c[i, h - j - 1];
        return cs;
    }
    public static Color[,] Fill(this Color c, int w, int h)
    {
        Color[,] cs = new Color[w, h];
        for (int i = 0; i < w; ++i)
            for (int j = 0; j < h; ++j)
                cs[i, j] = c;
        return cs;
    }

    internal static Dictionary<string, Texture2D> assocColor = new Dictionary<string, Texture2D>();
    public static Texture2D ToTexture(this Color c)
    {
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, c);
        t.Apply();
        if (assocColor.ContainsKey(c.ToString()))
            return assocColor[c.ToString()];
        else
        {
            assocColor.Add(c.ToString(), t);
            return t;
        }
    }
    public static IEnumerator Clone(this Color[,] c, int w, int h, int step, Action upt, Action<Color[,]> f)
    {
        Color[,] nc = new Color[w, h];
        int k = 0;
        for (int i = 0; i < w; ++i)
            for (int j = 0; j < h; ++j)
            {
                //Color cc = ((Color[])new Color[] { c[i, j] }.Clone())[0];
                nc[i, j] = (Color)c[i, j].Clone(); //new Color(cc.r, cc.g, cc.b);
                upt();
                ++k;
                if (k % step == 0)
                    yield return null;
            }
        f(nc);
    }
    public static void UptPixel(this Texture2D t, Point p, Color c)
    {
        t.SetPixel(p.x, t.height - p.y - 1, c);
        t.Apply();
    }
}

#endregion

#region "Color Extensions"

public static class PointHelpers
{
    public static Vector2[] GetVecArr(this Point[] ps)
    {
        return Array.ConvertAll(ps, (Point item) => (Vector2)item);
    }

    public static Point[] GetPointArr(this Vector2[] ps)
    {
        return Array.ConvertAll(ps, (Vector2 item) => (Point)item);
    }
}

#endregion

#region "Math Extensions"

public static class MathHelpers
{
    public static bool IsClockwise(this IEnumerable<Point> vertices)
    {
        double sum = 0.0;
        for (int i = 0; i < vertices.Count(); i++)
        {
            Point v1 = vertices.ElementAt(i),
                  v2 = vertices.ElementAt((i + 1) % vertices.Count()); // % is the modulo operator
            sum += (v2.x - v1.x) * (v2.y + v1.y);
        }
        return sum > 0.0;
    }

    public static int SortCornersClockwise(Point A, Point B, Point center)
    {
        //  Variables to Store the atans
        double aTanA, aTanB;

        //  Fetch the atans
        aTanA = Math.Atan2(A.y - center.y, A.x - center.x);
        aTanB = Math.Atan2(B.y - center.y, B.x - center.x);

        //  Determine next point in Clockwise rotation
        if (aTanA < aTanB) return -1;
        else if (aTanA > aTanB) return 1;
        return 0;
    }

    public static int InRange(this int value, int max) //Zero-index bases
    {
        if (value >= max) return value % max;
        else if (value < 0) return max - Mathf.Abs(value);
        else return value;
    }

    /*public static bool Orientation(this IEnumerable<Point> polygon, Point up)
    {
        var sum = polygon
            .Buffer(2, 1) // from Interactive Extensions Nuget Pkg
            .Where(b => b.Count == 2)
            .Aggregate
              (Vector3.Zero
              , (p, b) => p + Vector3.Cross(b[0], b[1])
                              / b[0].Length() / b[1].Length());

        return Vector3.Dot(up, sum) > 0;
    }*/
}

#endregion

#region "DateTime Extensions"

public static class DateTimeHelpers
{

    /// <summary>
    /// Converts a DateTime to the long representation which is the number of seconds since the unix epoch.
    /// </summary>
    /// <param name="dateTime">A DateTime to convert to epoch time.</param>
    /// <returns>The long number of seconds since the unix epoch.</returns>
    public static long ToEpoch(this DateTime dateTime) => (long)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;

    /// <summary>
    /// Converts a long representation of time since the unix epoch to a DateTime.
    /// </summary>
    /// <param name="epoch">The number of seconds since Jan 1, 1970.</param>
    /// <returns>A DateTime representing the time since the epoch.</returns>
    public static DateTime FromEpoch(this long epoch) => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(epoch);

    /// <summary>
    /// Converts a DateTime? to the long? representation which is the number of seconds since the unix epoch.
    /// </summary>
    /// <param name="dateTime">A DateTime? to convert to epoch time.</param>
    /// <returns>The long? number of seconds since the unix epoch.</returns>
    public static long? ToEpoch(this DateTime? dateTime) => dateTime.HasValue ? (long?)ToEpoch(dateTime.Value) : null;

    /// <summary>
    /// Converts a long? representation of time since the unix epoch to a DateTime?.
    /// </summary>
    /// <param name="epoch">The number of seconds since Jan 1, 1970.</param>
    /// <returns>A DateTime? representing the time since the epoch.</returns>
    public static DateTime? FromEpoch(this long? epoch) => epoch.HasValue ? (DateTime?)FromEpoch(epoch.Value) : null;

}

#endregion

#region "Reflection Extensions"

public class ReflectionHelpers
{
    // All error checking omitted. In particular, check the results
    // of Type.GetType, and make sure you call it with a fully qualified
    // type name, including the assembly if it's not in mscorlib or
    // the current assembly. The method has to be a public instance
    // method with no parameters. (Use BindingFlags with GetMethod
    // to change this.)
    public static void Invoke(string typeName, string methodName)
    {
        Type type = Type.GetType(typeName);
        object instance = Activator.CreateInstance(type);
        MethodInfo method = type.GetMethod(methodName);
        method.Invoke(instance, null);
    }

    public static void Invoke<T>(string methodName) where T : new()
    {
        T instance = new T();
        MethodInfo method = typeof(T).GetMethod(methodName);
        method.Invoke(instance, null);
    }
}

#endregion

#region "Assertions Extensions"

public class AssertExt
{
    public static void AreSame(object a, object b)
    {
        Assert.IsTrue(ReferenceEquals(a, b));
    }
}

#endregion