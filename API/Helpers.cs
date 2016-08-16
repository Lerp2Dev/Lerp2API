using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FullSerializer;
using Object = UnityEngine.Object;

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
    #endregion

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
    #endregion

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

    #endregion

    #region "Dictionary Extensions"
    public static Dictionary<TKey, TValue> RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
                                      TKey fromKey, TKey toKey)
    {
        TValue value = dic[fromKey];
        dic.Remove(fromKey);
        if(!dic.ContainsKey(toKey))
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
    #endregion

    #region "Iteration Extensions"
    public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
    {
        foreach (var i in ie)
            action(i);
    }
    #endregion

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
    #endregion


#if UNITY_EDITOR
    #region "Editor Extensions"
    public static void DefineTag(this string tagName)
    { //Credits: http://answers.unity3d.com/questions/33597/is-it-possible-to-create-a-tag-programmatically.html
        // Open tag manager
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // For Unity 5 we need this too
        //SerializedProperty layersProp = tagManager.FindProperty("layers");

        // Adding a Tag
        // First check if it is not already present
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tagName)) { found = true; break; }
        }

        // if not found, add it
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tagName;
        }
    }
    #endregion
#endif

    #region "GameObject Extensions"
    public static GameObject getRoot(this GameObject go)
    {
        Transform t = go.transform;
        while (t.parent != null)
            t = t.parent.transform;
        return t.gameObject;
    }
    #endregion
}

#region "JSON Helpers"
public class JSONHelpers<T>
{
    private static readonly fsSerializer _serializer = new fsSerializer();
    //public static bool readyDeserialization = false;
    public static Action<T> readyAction = delegate { };

    public static string Serialize(object value, bool pretty = true)
    {
        fsData data;
        _serializer.TrySerialize(typeof(T), value, out data).AssertSuccessWithoutWarnings();

        if(pretty)
            return fsJsonPrinter.PrettyJson(data);
        return fsJsonPrinter.CompressedJson(data);
    }

    public static T Deserialize(string serializedState)
    {
        fsData data = fsJsonParser.Parse(serializedState);

        T deserialized = default(T);
        _serializer.TryDeserialize(data, ref deserialized).AssertSuccessWithoutWarnings();

        if (readyAction != null)
            readyAction(deserialized);

        return deserialized;
    }
}
#endregion

#region "Serializer Helpers"
public static class SerializerHelpers
{
    public class Transporter
    {
        public string type;
        public bool write;
        public string str;
        public string[] strs;
        public byte[] arr;
        public Transporter(string s) : this(s, true)
        {
        }
        public Transporter(string s, bool w)
        {
            write = w;
            str = s;
            type = "String";
        }
        public Transporter(string[] s)
        {
            strs = s;
            type = "String[]";
        }
        public Transporter(byte[] a)
        {
            arr = a;
            type = "Byte[]";
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
        fpath = Path.Combine(path, n+".asset");
        rpath = fpath.Replace(Application.streamingAssetsPath, "");
        alreadyExists = File.Exists(fpath);
        if (!alreadyExists)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (value.type == "String")
                if (value.write)
                    File.WriteAllText(fpath, value.str);
                else
                    File.AppendAllText(fpath, value.str + Environment.NewLine);
            else if (value.type == "String[]")
                File.WriteAllLines(fpath, value.strs);
            else if (value.type == "Byte[]")
                File.WriteAllBytes(fpath, value.arr);
            else
                throw new Exception("Not recognised type!");
            Debug.Log("Asset saved at " + fpath + "!");
        }
        if (typeof(T).Equals(typeof(GameObject)))
        {
            if (Application.isPlaying)
                Object.Destroy(GameObject.Find(((GameObject)(object)obj).name));
            else
                Object.DestroyImmediate(GameObject.Find(((GameObject)(object)obj).name));
        }
        return rpath;
    }

    public static byte[] LoadAsset(string fpath, bool flood = true)
    {
        string p = Application.streamingAssetsPath + fpath; //Path.Combine(Application.streamingAssetsPath, fpath);
        if (!File.Exists(p))
        {
            if(flood)
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
        if(!objList.ContainsKey(root.name))
            objList.Add(root.name, root);
    }

    public static void makeHierarchyHidden(GameObject obj)
    {
        if (obj == null)
            return;
        if(obj.gameObject != null)
            obj.gameObject.AddComponent<MyHideFlags>();
        foreach (Transform child in obj.transform)
            makeHierarchyHidden(child.gameObject);
    }

    public static void makeHierarchyVisible(GameObject obj)
    {
        if (obj == null)
            return;
        if (obj.gameObject != null)
            obj.gameObject.RemoveComponent(typeof(MyHideFlags));
        foreach (Transform child in obj.transform)
            makeHierarchyVisible(child.gameObject);
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
#endregion

#region "Debug Helpers"
public class DebugParams
{
    public static void Log(string msg, params string[] subject)
    {
        Debug.Log(string.Format(msg, subject));
    }
    public static void LogWarning(string msg, params string[] subject)
    {
        Debug.LogWarning(string.Format(msg, subject));
    }
    public static void LogError(string msg, params string[] subject)
    {
        Debug.LogError(string.Format(msg, subject));
    }
}
#endregion