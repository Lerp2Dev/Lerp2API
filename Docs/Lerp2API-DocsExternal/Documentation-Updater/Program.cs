﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Documentation_Updater
{
    public class Program
    {
        private readonly static bool m_skipDoxygen = false,
                                     m_skipZiping = false,
                                     m_skipMSBuild = false,
                                     m_skipMerge = false,
                                     m_skipSetup = false,
                                     m_keepDocs = false;

        private const int m_phases = 5;
        private static long t = 0;
        private static int curPhase = 0;

        private static string ProgramFiles86
        {
            get
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }
        }

        private static string ProgramFiles
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            }
        }

        private static string DoxygenPath
        {
            get
            {
                return Path.Combine(ProgramFiles, "doxygen", "bin", "doxygen.exe");
            }
        }

        private static string InnoSetupPath
        {
            get
            {
                return Path.Combine(ProgramFiles, "InnoSetup", "ISCC.exe");
            }
        }

        private static string AppPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

        private static string ILMergePath
        {
            get
            {
                return Path.Combine(AppPath, "ILMerge", "ILMerge.exe");
            }
        }

        private static string ZIPManagerPath
        {
            get
            {
                return Path.Combine(AppPath, "ZIPManager", "ZIPManager.exe");
            }
        }

        private static string ResultPath
        {
            get
            {
                return Path.Combine(AppPath, "Lerp2API-Docs.exe");
            }
        }

        private static string MSBuildPath
        {
            get
            {
                return Path.Combine(AppPath, "MSBuild", "4.0", "MSBuild.exe");
            }
        }

        private static string DocsProject
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(AppPath), "Docs", "Lerp2API-Docs", "Lerp2API-Docs.sln");
            }
        }

        private static string Doxyfile
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(AppPath), "Doxyfile");
            }
        }

        private static string TempDocs
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(DocsProject)), "DocFiles");
            }
        }

        private static string TempBuildDocs
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(DocsProject)), "BuildFiles");
            }
        }

        private static string TemplatePath
        {
            get
            {
                return Path.Combine(AppPath, "InnoSetup Template", "Lerp2API-Docs.iss");
            }
        }

        private static bool IsAppInGoodLocation
        {
            get
            {
                return Directory.Exists(Path.Combine(Path.GetDirectoryName(AppPath), "Docs"))
                    && Directory.Exists(AppPath)
                    && File.Exists(Doxyfile);
            }
        }

        private static ProcessStartInfo DoxygenProcess
        {
            get
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd";
                startInfo.Arguments = string.Format(@"/k cd ""{0}"" & doxygen & exit", Path.GetDirectoryName(Doxyfile));
                //startInfo.UseShellExecute = false;
                //startInfo.RedirectStandardOutput = true;

                return startInfo;
            }
        }

        private static ProcessStartInfo ZIPMangerProcess
        {
            get
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ZIPManagerPath;
                startInfo.Arguments = string.Format(@"-c ""{0}"" ""{1}""", Path.Combine(Path.GetDirectoryName(DocsProject), "Lerp2API-Docs", "Resources", "DocsZIP.zip"), TempDocs);

                return startInfo;
            }
        }

        private static ProcessStartInfo MSBuildProcess
        {
            get
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd";
                startInfo.Arguments = string.Format(@"/k """"{0}"" ""{1}"" /t:Lerp2API-Docs /pp /p:Platform=""Any CPU"" /p:OutputPath=""{2}"" /p:Configuration=Debug"" & exit", MSBuildPath, DocsProject, TempBuildDocs);

                return startInfo;
            }
        }

        private static ProcessStartInfo ILMergeProcess
        {
            get
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd";
                startInfo.Arguments = string.Format(@"/k """"{0}"" /target:winexe /out:""{1}"" ""{2}"" ""{3}"""" & exit", ILMergePath, Path.Combine(Path.GetDirectoryName(AppPath), "Apps", "Lerp2API-Docs-Final.exe"), Path.Combine(TempBuildDocs, "Lerp2API-Docs.exe"), Path.Combine(TempBuildDocs, "Sharpcompress.dll"));

                return startInfo;
            }
        }

        private static ProcessStartInfo InnoSetupProcess
        {
            get
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd";
                startInfo.Arguments = string.Format(@"/k """"{0}"" ""{1}"""" & exit", InnoSetupPath, TemplatePath);

                return startInfo;
            }
        }

        private static void Main(string[] args)
        {
            //Preparar el chiringuito.

            //Comprobar si tenemos doxygen instalado, innosetup & istool, ilmerge.

            //Una vez hecho esto, comenzaremos con doxygen.
            //Una vez acabe doxygen, con el ZIPManager comprimiremos los archivos generados.
            //Una vez se acabe todo, moveremos el archivo ZIP a la carpeta Resource de Lerp2API-Docs.
            //Con un ejecutable local de MSBuild, compilaremos el chiringuito.
            //Una vez compilado todo, le pasaremos el ILMerge.
            //Una vez le pasemos el ILMerge, vamos a crear el instalador, llamando por command-line al ISTool, el archivo ISS estará por aquí.
            //Tengo que ver si se puede modificar de forma rápida algun parametro para cambiarle el output.

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Console.WriteLine("Starting process...");
            ChangeTitlePhase();

            //First, checks...
            if (!IsAppInGoodLocation)
            {
                Console.WriteLine("You must put this aplication inside of a folder called Apps, and next to this folder the Docs folder that has project of the Documentation.\nAlso, you need the Doxyfile on the root folder.");
                GoToFinish();
            }

            if (!File.Exists(DoxygenPath))
            {
                Console.WriteLine(DoxygenPath);
                Console.WriteLine("Doxygen is not installed. Please, install it!");
                GoToFinish();
            }

            if (!File.Exists(InnoSetupPath))
            {
                Console.WriteLine("InnoSetup is not installed. Please, install it!");
                GoToFinish();
            }

            if (!File.Exists(ILMergePath))
            {
                Console.WriteLine("ILMerge is not detected! Please, put ILMerge.exe inside Apps folder.");
                GoToFinish();
            }

            if (!File.Exists(ZIPManagerPath))
            {
                Console.WriteLine("ZIPManager is not detected! Please, put ZIPManager.exe inside Apps folder.");
                GoToFinish();
            }

            if (!File.Exists(MSBuildPath))
            {
                Console.WriteLine("MSBuild is not detected! Please, put MSBuild folder inside Apps folder.");
                GoToFinish();
            }

            if (!File.Exists(DocsProject))
            {
                Console.WriteLine("The Lerp2API-Docs solution, wasn't found in its default directory.");
                GoToFinish();
            }

            //Second, if we have passed this, then...
            if (!m_skipDoxygen)
                using (Process dx = Process.Start(DoxygenProcess))
                {
                    dx.WaitForExit();
                    Console.WriteLine("Doxygen created the documentation in {0} ms!", (sw.ElapsedMilliseconds - t));
                }

            ChangeTitlePhase();
            t = sw.ElapsedMilliseconds;

            if (!m_skipZiping)
            {
                string html = Path.Combine(Path.GetDirectoryName(AppPath), "html"),
                       latex = Path.Combine(Path.GetDirectoryName(AppPath), "latex");

                Directory.CreateDirectory(TempDocs);

                if (!m_keepDocs)
                {
                    Directory.Move(html, Path.Combine(TempDocs, "html"));
                    Directory.Move(latex, Path.Combine(TempDocs, "latex"));
                }
                else
                {
                    DirectoryCopy(html, TempDocs, true);
                    DirectoryCopy(latex, TempDocs, true);
                }

                Console.WriteLine("Dirs moved in {0} ms!", (sw.ElapsedMilliseconds - t));
                ChangeTitlePhase();
                t = sw.ElapsedMilliseconds;

                using (Process zip = Process.Start(ZIPMangerProcess))
                {
                    zip.WaitForExit();
                    Console.WriteLine("ZIPManager finished in {0} ms!", (sw.ElapsedMilliseconds - t));
                }

                Directory.Delete(TempDocs, true);
            }

            t = sw.ElapsedMilliseconds;

            if (!m_skipMSBuild)
            {
                Directory.CreateDirectory(TempBuildDocs);
                using (Process msb = Process.Start(MSBuildProcess))
                {
                    msb.WaitForExit();
                    Console.WriteLine("MSBuild finished in {0} ms!", (sw.ElapsedMilliseconds - t));
                }
            }

            ChangeTitlePhase();
            t = sw.ElapsedMilliseconds;

            if (!m_skipMerge)
            {
                using (Process ilm = Process.Start(ILMergeProcess))
                {
                    ilm.WaitForExit();
                    Console.WriteLine("ILMerge finished in {0} ms!", (sw.ElapsedMilliseconds - t));
                }
                if (Directory.Exists(TempBuildDocs))
                    Directory.Delete(TempBuildDocs, true);
                File.Delete(Path.Combine(AppPath, "Lerp2API-Docs-Final.exe"));
                File.Delete(Path.Combine(AppPath, "Lerp2API-Docs-Final.pdb"));
            }

            ChangeTitlePhase();
            t = sw.ElapsedMilliseconds;

            if (!m_skipSetup)
                using (Process set = Process.Start(InnoSetupProcess))
                {
                    set.WaitForExit();
                    Console.WriteLine("Setup finished in {0} ms!", (sw.ElapsedMilliseconds - t));
                }

            ChangeTitlePhase();

            //Third, stop and show results
            sw.Stop();

            Console.WriteLine("Finished in {0:F2} s!", sw.ElapsedMilliseconds / 1000d);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void GoToFinish()
        {
            Console.WriteLine("Press any key to finish...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private static void ChangeTitlePhase()
        {
            Console.Title = string.Format("Documentation Updater: {0}", curPhase == m_phases ? "Finished!" : string.Format("{0} of {1} phases completed", curPhase, m_phases));
            ++curPhase;
        }
    }
}