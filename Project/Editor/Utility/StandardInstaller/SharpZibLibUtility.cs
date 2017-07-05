using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using UnityEngine;
using UnityEditor;

namespace Lerp2API.Utility.StandardInstaller
{
    /// <summary>
    /// Class SharpZibLibUtility.
    /// </summary>
    public static class SharpZibLibUtility
    { //A esto solo le falta comprobar si un script ya existe en el proyecto actual
        private static long LastSize;
        private static bool StartChecking, Moving;
        private static double CheckEvery = 1, InternalTimer, LastTime;
        private static string DestFolder;

        private static List<string> PackagesList = new List<string>(),
                                    PackageFolders = new List<string>();

        private static Action upt;
        private static int curIndex, folderLength;

        /// <summary>
        /// Extracts the unity package.
        /// </summary>
        /// <param name="gzArchiveName">Name of the gz archive.</param>
        /// <param name="destFolder">The dest folder.</param>
        /// <param name="packList">The pack list.</param>
        public static void ExtractUnityPackage(string gzArchiveName, string destFolder, List<string> packList)
        {
            DestFolder = destFolder;
            PackagesList = packList;
            upt = CheckFor;
            EditorApplication.update += Update;
            using (Stream inStream = File.OpenRead(gzArchiveName))
            using (Stream gzipStream = new GZipInputStream(inStream))
            using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream))
            {
                tarArchive.ProgressMessageEvent += new ProgressMessageHandler((TarArchive archive, TarEntry entry, string message) =>
                {
                    if (!StartChecking) StartChecking = true;
                });
                tarArchive.ExtractContents(destFolder);
            }
        }

        private static void MovePackageFolders()
        {
            CheckEvery = .05;
            PackageFolders = Directory.GetDirectories(DestFolder).ToList();
            folderLength = PackageFolders.Count - 1;
            upt = SyncAssetImporting;
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (Moving)
                EditorUtility.DisplayProgressBar("Importing progress", string.Format("{0} of {1} files imported.", curIndex, folderLength), (float)curIndex / folderLength);
            double DeltaTime = EditorApplication.timeSinceStartup - LastTime;
            if (InternalTimer < CheckEvery)
                InternalTimer += DeltaTime;
            else
            {
                InternalTimer = 0;
                upt();
            }
            LastTime = EditorApplication.timeSinceStartup;
        }

        private static void CheckFor()
        {
            int fileCount = Directory.GetFiles(DestFolder).Length;
            if (StartChecking && LastSize == fileCount)
            {
                EditorApplication.update -= Update;
                StartChecking = false;
                Moving = true;
                MovePackageFolders();
            }
            LastSize = fileCount;
        }

        private static void SyncAssetImporting()
        { //Esto sobraría pero igualmente me gusta como ha quedado
            if (PackageFolders.Count > 1)
            {
                ++curIndex;
                string folder = PackageFolders[0];
                string pathname = Path.Combine(Application.dataPath.ReplaceLast("Assets", ""), File.ReadAllLines(Path.Combine(folder, "pathname"))[0]);
                if (File.Exists(Path.Combine(folder, "asset")))
                {
                    if (!Directory.Exists(Path.GetDirectoryName(pathname)))
                        Directory.CreateDirectory(Path.GetDirectoryName(pathname));
                    string ext = Path.GetFileName(pathname).Replace(Path.GetFileNameWithoutExtension(pathname), "");
                    if (!File.Exists(pathname) && (!IsScript(ext) || (IsScript(ext) && Directory.GetFiles(Application.dataPath, Path.GetFileName(pathname), SearchOption.AllDirectories).Length == 0)))
                        File.Move(Path.Combine(folder, "asset"), pathname);
                }
                else
                {
                    if (!Directory.Exists(pathname))
                        Directory.CreateDirectory(pathname);
                }
                PackageFolders.RemoveAt(0);
            }
            else
            {
                upt = null;
                CheckEvery = 1;
                Moving = false;
                EditorApplication.update -= Update;
                FinishImporting();
            }
        }

        private static void FinishImporting()
        {
            Directory.Delete(DestFolder, true);
            if (PackagesList.Count > 1)
            {
                string lastAsset = PackagesList[0];
                PackagesList.RemoveAt(0);
                Debug.Log(string.Format("'{0}' Asset has been imported successfully, continuing with {1}!", lastAsset, PackagesList[0]));
                string sFile = Path.Combine(Application.dataPath, "standardAssets.txt");
                var lines = File.ReadAllLines(sFile).Skip(1).ToArray();
                File.WriteAllLines(sFile, lines);
                EditorApplication.update -= Update;
                curIndex = 0;
                upt = null;
                StandardHandler.ImportAssets(PackagesList[0]);
            }
            else
            {
                EditorApplication.update -= Update;
                curIndex = 0;
                upt = null;
                Moving = false;
                EditorApplication.UnlockReloadAssemblies();
                Debug.Log("Importing process has been finished!");
                File.Delete(Path.Combine(Application.dataPath, "standardAssets.txt"));
                File.Create(Path.Combine(Application.dataPath.ReplaceLast("Assets", ""), ".dontDelete"));
                AssetDatabase.Refresh();
            }
        }

        private static bool IsScript(string ext)
        {
            return ext == ".cs" || ext == ".js";
        }
    }//220
}