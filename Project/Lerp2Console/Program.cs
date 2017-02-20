using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using Lerp2API;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;

namespace Lerp2Console
{
    class Program
    {
        //private static FileSystemWatcher l2dWatcher;
        private static string listenPath = "", listenFile = "", lastLine = ""; //executionPath = "",
        private static Parameter[] parameters;
        private static ulong calls;
        private const int msWait = 500;
        private static bool stacktrace = true, editor = false, fmsg = true;
        private static string[] sPath = LerpedCore.defaultLogFilePath.Split('/');

        /*
         * 
         * TODO:
         
            - Command system, inherited from lerp2api
             
        */

        //... -path=C:/.../ -file=example.txt
        static void Main(string[] args)
        { //Check when Unity button closes up...
            Console.Title = "Lerp2Dev Console";

            Work(args);

            Console.Read();
        }

        static void Work(string[] args)
        {
            parameters = Parameter.GetParams(args);
            //string ePath = GetParam("path"),
            //       lFile = GetParam("file");
            string projectPath = GetParam("projectPath");
            stacktrace = !ExistParam("nostacktrace");
            editor = ExistParam("editor");
            //executionPath = string.IsNullOrWhiteSpace(ePath) ? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ePath;
            listenPath = Path.Combine(projectPath, sPath[0]); //string.IsNullOrWhiteSpace(lFile) ? "debug.log" : lFile; //Tengo que probar a renombrar el archivo a ver que pasa...
            listenFile = Path.Combine(listenPath, sPath[1]);
            /*l2dWatcher = new FileSystemWatcher(listenPath, listenFile); //executionPath
            l2dWatcher.Changed += L2dWatcher_Changed;
            l2dWatcher.Deleted += L2dWatcher_Deleted;
            l2dWatcher.EnableRaisingEvents = true;*/
            QueryWork();
        }

        static void L2dWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ++calls;
        }

        static void L2dWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            ExitEvent();
        }

        private static void QueryWork()
        { //Se podria hacer una clase con esto...
            bool quick = false;
            if (calls > 0)
            {
                quick = GetResults();
                --calls;
            }
            Thread.Sleep(msWait / (quick ? 5 : 1));
            QueryWork();
        }

        private static bool GetResults()
        {
            string //path = Path.Combine(executionPath, listenFile),
                   line = "";
            bool r = false;
            if (!IsFileLocked(new FileInfo(listenFile)))
            {
                line = File.ReadAllLines(listenFile).Last();
                r = !string.IsNullOrWhiteSpace(line) && !string.IsNullOrWhiteSpace(lastLine) && line == lastLine;
                if (!r)
                {
                    ConsoleMessage msg = JsonUtility.FromJson<ConsoleMessage>(line);
                    Console.ForegroundColor = GetColor(msg.logType);
                    if (!editor && !fmsg) Console.WriteLine();
                    if (fmsg) fmsg = false;
                    if (!string.IsNullOrEmpty(msg.logString)) Console.WriteLine(msg.logString);
                    if (!string.IsNullOrEmpty(msg.stackTrace)) Console.WriteLine(msg.stackTrace);
                    Console.ResetColor();
                }
            }
            lastLine = line;
            return r;
        }

        private static string GetParam(string name)
        {
            Parameter p = parameters.FirstOrDefault(x => x.name == name);
            return p != null ? p.value : "";
        }

        private static bool ExistParam(string name)
        {
            return parameters.FirstOrDefault(x => x.name == name) != null;
        }

        private static string UnpackNl(string str)
        {
            return str.Replace("\\n", Environment.NewLine);
        }

        private static string PackNl(string str)
        {
            return str.Replace(Environment.NewLine, "\\n");
        }

        private static ConsoleColor GetColor(LogType type)
        {
            switch(type)
            {
                case LogType.Assert:
                    return ConsoleColor.Blue;
                case LogType.Error:
                    return ConsoleColor.Red;
                case LogType.Exception:
                    return ConsoleColor.Red;
                case LogType.Log:
                    return ConsoleColor.White;
                case LogType.Warning:
                    return ConsoleColor.Yellow;
                default:
                    return ConsoleColor.Black;
            }
        }

        private static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private static void ExitEvent()
        {
            //We create a compressed version of the log before we close...
            CreateCompressedLog();

            //And finally, we end up everything...
            Environment.Exit(0);
        }

        private static void CreateCompressedLog()
        {
            FastZip fastZip = new FastZip();

            string now = DateTime.Now.ToString("yyyy-MM-dd");
            int count = new DirectoryInfo(listenPath).GetFiles(string.Format("{0}*.{1}", now, sPath[1].Split('.')[1])).Length;

            fastZip.CreateZip(string.Format("{0}{1}.gz", now, (count > 0 ? "-" + (count - 1).ToString() : "")), listenPath, false, sPath[1]);
        }
    }

    class Parameter
    {
        public Parameter(string n, string v)
        {
            name = n;
            value = v;
        }
        public string name,
                      value;
        public static Parameter[] GetParams(string[] args)
        {
            List<Parameter> prms = new List<Parameter>();
            foreach(string a in args)
                if(a.StartsWith("-"))
                    if (a.Contains("="))
                    {
                        string[] param = a.Substring(1).Split('=');
                        prms.Add(new Parameter(param[0], param[1]));
                    }
                    else
                        prms.Add(new Parameter(a.Substring(1), ""));
            return prms.ToArray();
        }
    }
}