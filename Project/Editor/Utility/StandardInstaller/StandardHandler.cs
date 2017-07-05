using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lerp2API.Utility.StandardInstaller
{
    /// <summary>
    /// Class StandardHandler.
    /// </summary>
    [InitializeOnLoad]
    public static class StandardHandler
    {
        private static string standardAssetsPath, lastFile;
        private static string[] standardAssetsFiles;
        private static List<string> availableAssets = new List<string>();

        static StandardHandler()
        {
            if (!File.Exists(Path.Combine(Application.dataPath.Replace("/Assets", ""), ".dontDelete")) && File.Exists(Path.Combine(Application.dataPath, "standardAssets.txt")))
            {
                if (Directory.Exists(string.Format(EditorApplication.applicationContentsPath.Replace("Data", "{0}"), "Standard Packages")))
                    Debug.LogWarning("You are using an old version of Unity!");
                standardAssetsPath = string.Format(EditorApplication.applicationContentsPath.Replace("Data", "{0}"), "Standard Assets");
                if (!Directory.Exists(standardAssetsPath))
                {
                    Debug.LogWarning("Cannot find Standard Assets folder, you may deleted it or you didn't install it or you are using an old version of Unity!");
                    return;
                }
                standardAssetsFiles = File.ReadAllLines(Path.Combine(Application.dataPath, "standardAssets.txt"));
                foreach (string path in Directory.GetFiles(standardAssetsPath))
                {
                    string fileName = path.Replace(standardAssetsPath, "").Substring(1);
                    if (standardAssetsFiles.Contains(fileName))
                        availableAssets.Add(fileName);
                }
                if (EditorUtility.DisplayDialog("Editor message", string.Format("The following {0} assets needs to be added:\n\n{1}\n\nPress the button to start the installation.", availableAssets.Count, string.Join("\n", availableAssets.ToArray())), "Ok"))
                { //Falta esto
                    EditorApplication.LockReloadAssemblies();
                    ImportAssets(availableAssets[0]); //Hacer que se puedan marcar y desmarcar los assets. Incluyendo los que ya esten
                }
            }
        }

        /// <summary>
        /// Imports the assets.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void ImportAssets(string file)
        {
            string gzTemp = Path.Combine(Application.dataPath.ReplaceLast("Assets", ""), "GZipTemp"),
                   gzFile = file.Replace("unitypackage", "tar.gz");
            if (!Directory.Exists(gzTemp))
                Directory.CreateDirectory(gzTemp);
            if (!File.Exists(Path.Combine(gzTemp, gzFile)))
                File.Copy(Path.Combine(standardAssetsPath, file), Path.Combine(gzTemp, gzFile));
            SharpZibLibUtility.ExtractUnityPackage(Path.Combine(gzTemp, gzFile), gzTemp, availableAssets);
        }
    }//90
}