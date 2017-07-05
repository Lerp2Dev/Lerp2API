using Lerp2API.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TeamUtility.IO;
using UnityEditor;
using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2APIEditor
{
    /// <summary>
    /// Class EditorHelpers.
    /// </summary>
    public static class EditorHelpers
    {
        #region "Editor Extensions"

        private static SerializedObject _tagMan;

        private static SerializedObject tagManager
        {
            get
            {
                // Open tag manager
                if (_tagMan == null)
                    _tagMan = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                return _tagMan;
            }
        }

        private static SerializedProperty _tagsProp;

        private static SerializedProperty tagsProp
        {
            get
            {
                if (_tagsProp == null)
                    _tagsProp = tagManager.FindProperty("tags");
                return _tagsProp;
            }
        }

        private static SerializedProperty _layersProp;

        private static SerializedProperty layersProp
        {
            get
            {
                if (_layersProp == null)
                    _layersProp = tagManager.FindProperty("layers");
                return _layersProp;
            }
        }

        /// <summary>
        /// Defines the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        public static void DefineTag(this string tagName)
        { //Credits: http://answers.unity3d.com/questions/33597/is-it-possible-to-create-a-tag-programmatically.html
            bool found = tagName.CheckTag();

            // if not found, add it
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = tagName;
            }

            // and to save the changes
            tagManager.ApplyModifiedProperties();
        }

        /// <summary>
        /// Checks the tag.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckTag(this string tagName)
        {
            // Adding a Tag
            // First check if it is not already present
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tagName)) { found = true; break; }
            }

            return found;
        }

        /// <summary>
        /// Checks the layer.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckLayer(this string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            return layersProp.GetArrayElementAtIndex(layer) != null && layer != 0;
        }

        /// <summary>
        /// Checks the axis.
        /// </summary>
        /// <param name="axisName">Name of the axis.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckAxis(this string axisName)
        {
            try
            {
                InputManager.GetAxis(axisName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks the button.
        /// </summary>
        /// <param name="btnName">Name of the BTN.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CheckButton(string btnName)
        {
            try
            {
                InputManager.GetButton(btnName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Defines the layer.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        public static void DefineLayer(this string layerName)
        {
            int i = 8;
            for (i = 8; i <= 31; i++)
            {
                string nm = "User Layer " + i;
                SerializedProperty sp = tagManager.FindProperty(nm);
                if (sp == null || (sp != null && string.IsNullOrEmpty(sp.stringValue)))
                    break;
            }
            layerName.DefineLayer(i);
        }

        /// <summary>
        /// Defines the layer.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="layer">The layer.</param>
        public static void DefineLayer(this string layerName, int layer)
        {
#if UNITY_4
            // --- Unity 4 ---
            SerializedProperty sp = tagManager.FindProperty("User Layer "+layer);
            if (sp != null) sp.stringValue = layerName;
#else //Deberia ser un #elif UNITY_5, pero como estamos en la api y las directivas de preprocesador estan un poco cascadas...
            // --- Unity 5 ---
            SerializedProperty sp = layersProp.GetArrayElementAtIndex(layer);
            if (sp != null) sp.stringValue = layerName;
#endif
            // and to save the changes
            tagManager.ApplyModifiedProperties();
        }

        /// <summary>
        /// Gets the defined tags.
        /// </summary>
        /// <returns>NamedData[].</returns>
        public static NamedData[] GetDefinedTags()
        {
            List<NamedData> tags = new List<NamedData>();
            for (int i = 0; i < tagsProp.arraySize; ++i)
            {
                SerializedProperty sp = tagsProp.GetArrayElementAtIndex(i);
                tags.Add(new NamedData(sp.stringValue));
            }
            return tags.ToArray();
        }

        /// <summary>
        /// Gets the defined layers.
        /// </summary>
        /// <returns>LayerData[].</returns>
        public static LayerData[] GetDefinedLayers()
        {
            List<LayerData> layers = new List<LayerData>();
            for (int i = 8; i < layersProp.arraySize - 8; ++i)
            {
                SerializedProperty sp = layersProp.GetArrayElementAtIndex(i);
                layers.Add(new LayerData(sp.stringValue, i));
            }
            return layers.ToArray();
        }

        #endregion "Editor Extensions"
    }

    #region "Editor Reflection Extensions"

    /// <summary>
    /// Class EditorReflectionHelpers.
    /// </summary>
    public class EditorReflectionHelpers
    {
        /// <summary>
        /// The fin
        /// </summary>
        public Action fin;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorReflectionHelpers"/> class.
        /// </summary>
        /// <param name="f">The f.</param>
        public EditorReflectionHelpers(Action f)
        {
            fin = f;
        }

        /// <summary>
        /// Waits the until class is available.
        /// </summary>
        /// <param name="type">The type.</param>
        public void WaitUntilClassIsAvailable(string type)
        {
            Debug.LogFormat("Waiting for '{0}' class.", type);
            ThreadSafeEditor tse = new ThreadSafeEditor();
            Thread th = new Thread(() =>
            {
                tse.Message("echo prueba xd", "echo hola");
                int secs = 30;
                bool av = false;
                while (--secs > 0)
                {
                    av = Type.GetType(type) != null;
                    if (av)
                        break;
                    Thread.Sleep(1000);
                }
                if (av) fin();
            });
            th.Start();
        }
    }

    #endregion "Editor Reflection Extensions"

    #region "Thread Safe Editor Extensions"

    /// <summary>
    /// Class ThreadSafeEditor.
    /// </summary>
    public class ThreadSafeEditor
    {
        private Process cmd;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeEditor"/> class.
        /// </summary>
        public ThreadSafeEditor()
        {
            cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
        }

        /// <summary>
        /// Messages the specified commands.
        /// </summary>
        /// <param name="commands">The commands.</param>
        public void Message(params string[] commands)
        {
            cmd.StartInfo.Arguments = string.Format("/c {0} & pause", string.Join(" & ", commands));
            cmd.Start();
            cmd.Close();
        }
    }

    #endregion "Thread Safe Editor Extensions"
}