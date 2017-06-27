using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lerp2API.Hepers.Serializer_Helpers
{
    /// <summary>
    /// Class SerializerHelpers.
    /// </summary>
    public static class SerializerHelpers
    {
        /// <summary>
        /// Class Transporter.
        /// </summary>
        public class Transporter
        {
            /// <summary>
            /// The type
            /// </summary>
            public Type type;

            /// <summary>
            /// The write
            /// </summary>
            public bool write;

            /// <summary>
            /// The object
            /// </summary>
            public object obj;

            /// <summary>
            /// Initializes a new instance of the <see cref="Transporter"/> class.
            /// </summary>
            /// <param name="o">The o.</param>
            /// <param name="w">if set to <c>true</c> [w].</param>
            public Transporter(object o, bool w = true)
            {
                type = o.GetType();
                obj = o;
            }
        }

        /// <summary>
        /// Saves as asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str">The string.</param>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <param name="w">if set to <c>true</c> [w].</param>
        /// <returns>System.String.</returns>
        public static string SaveAsAsset<T>(this string str, T obj, string name = "", bool w = true)
        {
            return SaveAsAsset(new Transporter(str, w), obj, name);
        }

        /// <summary>
        /// Saves as asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strs">The STRS.</param>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public static string SaveAsAsset<T>(this string[] strs, T obj, string name = "")
        {
            return SaveAsAsset(new Transporter(strs), obj, name);
        }

        /// <summary>
        /// Saves as asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">The arr.</param>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public static string SaveAsAsset<T>(this byte[] arr, T obj, string name = "")
        {
            return SaveAsAsset(new Transporter(arr), obj, name);
        }

        /// <summary>
        /// Saves as asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="Exception">Not recognised type!</exception>
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
                //if (IsPlaying())
                //    Object.Destroy(GameObject.Find(((GameObject)(object)obj).name));
                //else
                if (Application.isEditor)
                    Object.DestroyImmediate(GameObject.Find(((GameObject)(object)obj).name));
            }
            return rpath;
        }

        /// <summary>
        /// Loads the asset.
        /// </summary>
        /// <param name="fpath">The fpath.</param>
        /// <returns>System.Byte[].</returns>
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
}