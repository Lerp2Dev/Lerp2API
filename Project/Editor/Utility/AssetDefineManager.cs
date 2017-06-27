/*
Author: Jon Kenkel (nonathaj)
Created: 1/23/2016
*/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace Lerp2API.Utility
{
    /// <summary>
    /// Class AssetDefineManager.
    /// </summary>
    /// <seealso cref="UnityEditor.AssetPostprocessor" />
    [InitializeOnLoad]
    public class AssetDefineManager : AssetPostprocessor
    {
        /// <summary>
        /// Custom defines to add based on the file to detect the asset by, and the desired platforms
        /// </summary>
        private static List<AssetDefine> CustomDefines = new List<AssetDefine>();

        private struct AssetDefine
        {
            /// <summary>
            /// The asset detection file
            /// </summary>
            public readonly string assetDetectionFile;              //the file used to detect if the asset exists
            /// <summary>
            /// The asset defines
            /// </summary>
            public readonly string[] assetDefines;                  //series of defines for this asset
            /// <summary>
            /// The define platforms
            /// </summary>
            public readonly BuildTargetGroup[] definePlatforms;     //platform this define will be used for (null is all platforms)

            /// <summary>
            /// Initializes a new instance of the <see cref="AssetDefine"/> struct.
            /// </summary>
            /// <param name="fileToDetectAsset">The file to detect asset.</param>
            /// <param name="platformsForDefine">The platforms for define.</param>
            /// <param name="definesForAsset">The defines for asset.</param>
            public AssetDefine(string fileToDetectAsset, BuildTargetGroup[] platformsForDefine, params string[] definesForAsset)
            {
                assetDetectionFile = fileToDetectAsset;
                definePlatforms = platformsForDefine;
                assetDefines = definesForAsset;
            }

            /// <summary>
            /// Returns true if ... is valid.
            /// </summary>
            /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
            public bool IsValid { get { return assetDetectionFile != null && assetDefines != null; } }
            /// <summary>
            /// The invalid
            /// </summary>
            public static AssetDefine Invalid = new AssetDefine(null, null, null);

            /// <summary>
            /// Removes all defines.
            /// </summary>
            public void RemoveAllDefines()
            {
                foreach (string define in assetDefines)
                    RemoveCompileDefine(define, definePlatforms);
            }

            /// <summary>
            /// Adds all defines.
            /// </summary>
            public void AddAllDefines()
            {
                foreach (string define in assetDefines)
                    AddCompileDefine(define, definePlatforms);
            }
        }

        static AssetDefineManager()
        {
            ValidateDefines();
        }

        private static void ValidateDefines()
        {
            foreach (AssetDefine def in CustomDefines)
            {
                string[] fileCodes = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(def.assetDetectionFile));
                foreach (string fileCode in fileCodes)
                {
                    string fileName = Path.GetFileName(AssetDatabase.GUIDToAssetPath(fileCode));
                    if (fileName == def.assetDetectionFile)
                    {
                        if (def.IsValid)        //this is an asset we are tracking for defines
                            def.AddAllDefines();
                    }
                }
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string deletedFile in deletedAssets)
            {
                AssetDefine def = AssetDefine.Invalid;
                {
                    string file = Path.GetFileName(deletedFile);
                    foreach (AssetDefine define in CustomDefines)
                    {
                        if (define.assetDetectionFile == file)
                        {
                            def = define;
                            break;
                        }
                    }
                }

                if (def.IsValid)            //this is an asset we are tracking for defines
                    def.RemoveAllDefines();
            }
        }

        /// <summary>
        /// Attempts to add a new #define constant to the Player Settings
        /// </summary>
        /// <param name="newDefineCompileConstant">constant to attempt to define</param>
        /// <param name="targetGroups">platforms to add this for (null will add to all platforms)</param>
        public static void AddCompileDefine(string newDefineCompileConstant, BuildTargetGroup[] targetGroups = null)
        {
            if (targetGroups == null)
                targetGroups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));

            foreach (BuildTargetGroup grp in targetGroups)
            {
                if (grp == BuildTargetGroup.Unknown)        //the unknown group does not have any constants location
                    continue;

                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(grp);
                if (!defines.Contains(newDefineCompileConstant))
                {
                    if (defines.Length > 0)         //if the list is empty, we don't need to append a semicolon first
                        defines += ";";

                    defines += newDefineCompileConstant;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(grp, defines);
                }
            }
        }

        /// <summary>
        /// Attempts to remove a #define constant from the Player Settings
        /// </summary>
        /// <param name="defineCompileConstant"></param>
        /// <param name="targetGroups"></param>
        public static void RemoveCompileDefine(string defineCompileConstant, BuildTargetGroup[] targetGroups = null)
        {
            if (targetGroups == null)
                targetGroups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));

            foreach (BuildTargetGroup grp in targetGroups)
            {
                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(grp);
                int index = defines.IndexOf(defineCompileConstant);
                if (index < 0)
                    continue;           //this target does not contain the define
                else if (index > 0)
                    index -= 1;         //include the semicolon before the define
                                        //else we will remove the semicolon after the define

                //Remove the word and it's semicolon, or just the word (if listed last in defines)
                int lengthToRemove = Math.Min(defineCompileConstant.Length + 1, defines.Length - index);

                //remove the constant and it's associated semicolon (if necessary)
                defines = defines.Remove(index, lengthToRemove);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(grp, defines);
            }
        }
    }
}