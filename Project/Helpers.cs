using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FullSerializer;
using Object = UnityEngine.Object;
using Debug = Lerp2API.DebugHandler.Debug;
using Lerp2API.Game;

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

        public static Texture2D ToTexture(this Color c, int w = 1, int h = 1)
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

        public static string ToUString(this object message) //To Universal String, converts an object to string by checking if it's a string
        {
            return message.GetType().Equals(typeof(string)) ? (string)message : message.ToString();
        }

        #endregion "Object Extensions"

        #region "Color Extensions"

        public static string ColorToHex(this Color color) 
        {
            return ColorToHex((Color32)color);
        }

        public static string ColorToHex(this Color32 color)
        {
            string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
            return hex;
        }
         
        public static Color HexToColor(this string hex)
        {
            byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
            return new Color32(r,g,b, 255);
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
        public static bool isEnabled
        {
            get
            {
                return GetBool("ENABLE_DEBUG");
            }
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

        public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, float width = 1.0f, bool depthTest = true)
        {
            if (color.Equals(default(Color)))
                color = DebugColor.normal;
            if (isEnabled)
            {
                if (Application.isEditor && Application.isPlaying)
                    UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
                if (Application.isPlaying && isGamingEnabled)
                    DebugLine.DrawLine(start, end, color, duration, width);
            }
        }

        public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, float width = 1.0f, bool depthTest = true)
        {
            if (color.Equals(default(Color)))
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

        public static void DrawCube(Vector3 pos, Color col, Vector3 scale)
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

        public static void DrawRect(Rect rect, Color col)
        {
            if (isEnabled)
            {
                Vector3 pos = new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 0.0f);
                Vector3 scale = new Vector3(rect.width, rect.height, 0.0f);

                DrawRect(pos, col, scale);
            }
        }

        public static void DrawRect(Vector3 pos, Color col, Vector3 scale)
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

        public static void DrawPoint(Vector3 pos, Color col, float scale)
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
        public static void DrawOutline(Rect rect, string text, GUIStyle style, Color outColor, Color inColor, float size)
        {
            float halfSize = size * 0.5F;
            GUIStyle backupStyle = new GUIStyle(style);
            Color backupColor = GUI.color;
 
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

        public static void DrawShadow(Rect rect, GUIContent content, GUIStyle style, Color txtColor, Color shadowColor,
                                        Vector2 direction)
        {

            rect.x += direction.x;
            rect.y += direction.y;
            GUI.Label(rect, content, new GUIStyle(style) { normal = new GUIStyleState() { textColor = shadowColor } });
 
            rect.x -= direction.x;
            rect.y -= direction.y;
            GUI.Label(rect, content, new GUIStyle(style) { normal = new GUIStyleState() { textColor = txtColor } });
        }

        public static void DrawLayoutOutline(string text, GUIStyle style, Color outColor, Color inColor, float size, params GUILayoutOption[] options)
        {
            DrawOutline(GUILayoutUtility.GetRect(new GUIContent(text), style, options), text, style, outColor, inColor, size);
        }

        public static void DrawLayoutShadow(GUIContent content, GUIStyle style, Color txtColor, Color shadowColor,
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
 
            Color color = r.Contains(Event.current.mousePosition) ? letters.hover.textColor : letters.normal.textColor;
 
            DrawShadow(r, content, letters, color, new Color(0f, 0f, 0f, shadowAlpha), direction);
 
            return result;
        }
 
        public static bool DrawLayoutButtonWithShadow(GUIContent content, GUIStyle style, float shadowAlpha,
                                                       Vector2 direction, params GUILayoutOption[] options)
        {
            return DrawButtonWithShadow(GUILayoutUtility.GetRect(content, style, options), content, style, shadowAlpha, direction);
        }
}

#endregion