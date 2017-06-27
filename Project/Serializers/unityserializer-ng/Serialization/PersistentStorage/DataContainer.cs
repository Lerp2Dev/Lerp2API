using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

namespace UnitySerializerNG.FilePreferences
{
    /// <summary>
    /// Class DataContainer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DataContainer<T>
    {
#pragma warning disable 0414
        private static GameObject QuitObject;
#pragma warning restore 0414

        private static string root = Path.GetFullPath(Application.persistentDataPath) + Path.DirectorySeparatorChar + "persistentData";
        private string profileName;
        private string path;

        private Dictionary<string, T> dict = new Dictionary<string, T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContainer{T}"/> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="profile">The profile.</param>
        public DataContainer(string filename, string profile = "default")
        {
            profileName = profile;
            path = root + Path.DirectorySeparatorChar + profile + Path.DirectorySeparatorChar + filename;

            if (File.Exists(path))
            {
                IFormatter formatter = new BinaryFormatter();
                using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    try
                    {
                        dict = (Dictionary<string, T>)formatter.Deserialize(stream);
                    }
                    catch (SerializationException e)
                    {
                        Debug.LogException(e);
                        RebuildFile();
                    }
            }
            else
                RebuildFile();

            if (Application.isPlaying && SaveOnQuit.Instances < 1)
                QuitObject = new GameObject("FilePrefs_QuitObject", typeof(SaveOnQuit));
        }

        private void RebuildFile()
        {
            if (File.Exists(path))
                File.Delete(path);

            if (!Directory.Exists(root + Path.DirectorySeparatorChar + profileName))
                Directory.CreateDirectory(root + Path.DirectorySeparatorChar + profileName);

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                try
                {
                    formatter.Serialize(stream, dict);
                }
                catch (SerializationException e)
                {
                    Debug.LogException(e);
                    stream.Close();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    stream.Close();
                }
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            try
            {
                Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

                try
                {
                    formatter.Serialize(stream, dict);
                }
                catch (SerializationException e)
                {
                    Debug.LogException(e);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                finally
                {
                    stream.Close();
                }
            }
            catch (Exception)
            {
                RebuildFile();
                Save();
            }
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        public T Get(string key)
        {
            try
            {
                return dict[key];
            }
            catch (KeyNotFoundException)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Sets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Set(string key, T value)
        {
            dict[key] = value;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            dict.Clear();
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(string key)
        {
            return dict.Remove(key);
        }

        /// <summary>
        /// Finds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Find(string key)
        {
            return dict.ContainsKey(key);
        }

        // Only for debugging purposes!
        //public int Count() {
        //    return dict.Count;
        //}

        //public void PrintAll() {
        //    string s = "";
        //    foreach (KeyValuePair<string, T> key in dict) {
        //        s += key.Value.ToString() + ", ";
        //    }

        //    Debug.Log(s);
        //}
    }
}