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
        private static void SaveEditorSkin()
        { //I have to compare the old files
            string dir = Path.Combine(Application.dataPath, "Saved Skins/");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            int c = Directory.GetFiles(dir, "*.guiskin", SearchOption.AllDirectories).Length;
            List<string> enums = new List<string>();
            Array vals = Enum.GetValues(typeof(EditorSkin));
            foreach (EditorSkin es in vals)
            {
                GUISkin skin = Instantiate(EditorGUIUtility.GetBuiltinSkin(es)) as GUISkin;
                string n = string.Format("SceneSkin{0} - {1}.guiskin", es, c / 3);
                enums.Add(n);
                n = "Assets/Saved Skins/" + n;
                AssetDatabase.CreateAsset(skin, n); //There should be an dialog to set the name!
            }
            EditorUtility.DisplayDialog("API Message", string.Format("GUI Skin saved in 'Saved Skins' folder all {0} scripts with names: ({1})!", vals.Length, string.Join(", ", enums.ToArray())), "Ok");
        }
    }
}