using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

namespace Lerp2API.Utility
{
    /// <summary>
    /// Class DummyEditor.
    /// </summary>
    /// <seealso cref="UnityEditor.EditorWindow" />
    public class DummyEditor : EditorWindow
    {
        /// <summary>
        /// Saves the editor skin.
        /// </summary>
        /// <param name="command">The command.</param>
        [MenuItem("Lerp2Dev Team Tools/Save Scene Skin...")]
        static public void SaveEditorSkin(MenuCommand command)
        {
            int c = Directory.GetFiles(Path.Combine(Application.dataPath, "Saved Skins/"), "*.guiskin", SearchOption.AllDirectories).Length;
            List<string> enums = new List<string>();
            Array vals = Enum.GetValues(typeof(EditorSkin));
            foreach (EditorSkin es in vals)
            {
                GUISkin skin = Instantiate(EditorGUIUtility.GetBuiltinSkin(es)) as GUISkin;
                string n = string.Format("SceneSkin{0} - {1}.guiskin", es, c);
                enums.Add(n);
                n = "Assets/Saved Skins/" + n;
                AssetDatabase.CreateAsset(skin, n); //There should be an dialog to set the name!
            }
            EditorUtility.DisplayDialog("API Message", string.Format("GUI Skin saved in 'Saved Skins' folder all {0} scripts with names: ({1})!", vals.Length, string.Join(", ", enums.ToArray())), "Ok");
        }
    }
}