using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
//using Lerp2API;
using UnityEngine;
//using ICSharpCode.SharpZipLib.Zip;
//using Lerp2API.Communication.Sockets;
using System.Net.Sockets;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using System.Security.Principal;
//using ClientServerUsingNamedPipes.Server;
//using ClientServerUsingNamedPipes.Interfaces;

namespace Lerp2Console
{
    class Program
    {

        const int HWND_BROADCAST = 0xffff;
        const uint WM_SETTINGCHANGE = 0x001a;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SendNotifyMessage(IntPtr hWnd, uint Msg,
            UIntPtr wParam, string lParam);

        //private static FileSystemWatcher l2dWatcher;
        //private static PipeServer l2dStream;
        private static Lerp2API.Communication.Sockets.SocketServer l2dServer;
        private static string projectPath = "", listenPath = "", listenFile = "", lastLine = ""; //executionPath = "",
        private static Parameter[] parameters;
        private static ulong calls;
        private const int msWait = 500;
        private static bool stacktrace = true, editor = false, fmsg = true;
        private static string[] sPath = Lerp2API.LerpedCore.defaultLogFilePath.Split('/');
        private const string TIMES_OPENED = "timesOpened";
        private static int timesOpened = 0;

        /*
         * 
         * TODO:
         
            - Command system, inherited from lerp2api
             
        */

        //... -path=C:/.../ -file=example.txt
        static void Main(string[] args)
        { //Check when Unity button closes up

            timesOpened = ForceExistParam(args, TIMES_OPENED) ? int.Parse(ForceGetParam(args, TIMES_OPENED)) : 0;

            string EXEPath = Assembly.GetExecutingAssembly().Location,
                   DLLDir = Path.GetDirectoryName(Path.GetDirectoryName(EXEPath)),
                   curPATH = Environment.GetEnvironmentVariable("PATH");
            string[] splitPATHS = (curPATH[curPATH.Length - 1] == ';' ? curPATH.Remove(curPATH.Length - 1) : curPATH).Split(';');

            //Only for debug now:
            /*PreventiveRunAs(args, EXEPath, false);
            if(IsUserAdministrator()) ForcePathReset(splitPATHS, DLLDir);
            Console.ReadKey();

            return;*/

            bool reinit = !curPATH.Contains("Lerp2API") || curPATH.Contains("Lerp2API") && !splitPATHS.Any(x => x == DLLDir);

            Console.WriteLine(timesOpened);
            Console.WriteLine(IsUserAdministrator());
            Console.WriteLine(reinit);

            if (reinit)
                PreventiveRunAs(args, EXEPath);

            if (!curPATH.Contains("Lerp2API"))
                SetEnvPath(curPATH + ";" + DLLDir + ";");
            else if (!splitPATHS.Any(x => x == DLLDir))
                ForcePathReset(splitPATHS, DLLDir);

            if(reinit)
            {
                Process.Start(EXEPath);
                Environment.Exit(0);
            }

            Console.Title = "Lerp2Dev Console";

            parameters = Parameter.GetParams(args);
            projectPath = GetParam("projectPath");
            stacktrace = !ExistParam("nostacktrace");
            editor = ExistParam("editor");

            string dllDirectory = Path.Combine(projectPath, "Lerp2API");
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDirectory);

            Work(args);

            Console.Read();
        }

        static void PreventiveRunAs(string[] args, string exe, bool closeApp = true)
        {
            //Console.WriteLine(timesOpened);
            //Console.WriteLine(IsUserAdministrator());
            if (!IsUserAdministrator() && timesOpened <= 2)
            {
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = exe;
                    p.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                    p.StartInfo.Arguments = string.Join(" ", args.Where(x => !x.Contains("timesOpened"))) + " -timesOpened=" + (++timesOpened);
                    p.StartInfo.UseShellExecute = true;
                    if (Environment.OSVersion.Version.Major >= 6)
                        p.StartInfo.Verb = "runas";
                    try
                    {
                        Console.WriteLine("Trying to run as administrator...");
                        p.Start();
                    }
                    catch
                    {
                        Console.WriteLine("The very first time you start the console, you must call it as an administrator, because we need to set a new entry in %PATH% of Windows.");
                        Console.WriteLine("");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
                    }
                    finally
                    {
                        Console.WriteLine("Succesfully running as admin, closing this instance.");
                    }
                }
                if(closeApp) Environment.Exit(0);
            }
        }

        static void SetEnvPath(string newpath)
        {
            using (var envKey = Registry.LocalMachine.OpenSubKey(
    @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment",
    true))
            {
                Contract.Assert(envKey != null, @"registry key is missing!");
                envKey.SetValue("PATH", newpath);
                SendNotifyMessage((IntPtr)HWND_BROADCAST, WM_SETTINGCHANGE,
                    (UIntPtr)0, "Environment");
            }
        }

        static void ForcePathReset(string[] paths, string dll)
        {
            List<string> tempPathList = paths.Where(x => !x.Contains("Lerp2API")).ToList();
            tempPathList.Add(dll);
            string newPATH = string.Join(";", tempPathList.ToArray()) + ";";
            SetEnvPath(newPATH);
        }

        static void Work(string[] args)
        {
            /*l2dStream = new PipeServer();
            //PublishServerID(l2dStream.ServerId);

            l2dStream.Start();

            ConsoleSender.InitStream(l2dStream.ServerId);

            l2dStream.ClientConnectedEvent += L2dStream_ClientConnected;
            l2dStream.ClientDisconnectedEvent += L2dStream_ClientDisconnected;
            l2dStream.MessageReceivedEvent += L2dStream_MessageReceived;*/

            l2dServer = new Lerp2API.Communication.Sockets.SocketServer();

            l2dServer.ComeAlive();
            l2dServer.StartListening();

            l2dServer.ServerCallback = new AsyncCallback(AcceptCallback);

            listenPath = Path.Combine(projectPath, sPath[0]); //string.IsNullOrWhiteSpace(lFile) ? "debug.log" : lFile; //Tengo que probar a renombrar el archivo a ver que pasa...
            listenFile = Path.Combine(listenPath, sPath[1]);
            /*l2dWatcher = new FileSystemWatcher(listenPath, listenFile); //executionPath
            l2dWatcher.Changed += L2dWatcher_Changed;
            l2dWatcher.Deleted += L2dWatcher_Deleted;
            l2dWatcher.EnableRaisingEvents = true;*/
            QueryWork();
        }

        /// <summary>
        /// Asynchronously accepts an incoming connection attempt and creates
        /// a new Socket to handle remote host communication.
        /// </summary>     
        /// <param name="ar">the status of an asynchronous operation
        /// </param> 
        public static void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = null;

            // A new Socket to handle remote host communication
            Socket handler = null;
            try
            {
                // Receiving byte array
                byte[] buffer = new byte[1024];
                // Get Listening Socket object
                listener = (Socket)ar.AsyncState;
                // Create a new socket
                handler = listener.EndAccept(ar);

                // Using the Nagle algorithm
                handler.NoDelay = false;

                // Creates one object array for passing data
                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                // Begins to asynchronously receive data
                handler.BeginReceive(
                    buffer,        // An array of type Byt for received data
                    0,             // The zero-based position in the buffer 
                    buffer.Length, // The number of bytes to receive
                    SocketFlags.None,// Specifies send and receive behaviors
                    new AsyncCallback(ReceiveCallback),//An AsyncCallback delegate
                    obj            // Specifies infomation for receive operation
                    );

                // Begins an asynchronous operation to accept an attempt
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(aCallback, listener);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Asynchronously receive data from a connected Socket.
        /// </summary>
        /// <param name="ar">
        /// the status of an asynchronous operation
        /// </param> 
        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Fetch a user-defined object that contains information
                object[] obj = new object[2];
                obj = (object[])ar.AsyncState;

                // Received byte array
                byte[] buffer = (byte[])obj[0];

                // A Socket to handle remote host communication.
                Socket handler = (Socket)obj[1];

                // Received message
                string content = string.Empty;

                // The number of bytes received.
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    content += Encoding.Unicode.GetString(buffer, 0,
                        bytesRead);
                    // If message contains "<Client Quit>", finish receiving
                    if (content.IndexOf("<Client Quit>") > -1)
                    {
                        // Convert byte array to string
                        string str =
                            content.Substring(0, content.LastIndexOf("<Client Quit>"));
                        Console.WriteLine(
                            "Read {0} bytes from client.\n Data: {1}",
                            str.Length * 2, str);

                        // Prepare the reply message
                        byte[] byteData =
                            Encoding.Unicode.GetBytes(str);

                        // Sends data asynchronously to a connected Socket
                        handler.BeginSend(byteData, 0, byteData.Length, 0,
                            new AsyncCallback(SendCallback), handler);
                    }
                    else
                    {
                        // Continues to asynchronously receive data
                        byte[] buffernew = new byte[1024];
                        obj[0] = buffernew;
                        obj[1] = handler;
                        handler.BeginReceive(buffernew, 0, buffernew.Length,
                            SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Sends data asynchronously to a connected Socket.
        /// </summary>
        /// <param name="ar">
        /// The status of an asynchronous operation
        /// </param> 
        public static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // A Socket which has sent the data to remote host
                Socket handler = (Socket)ar.AsyncState;

                // The number of bytes sent to the Socket
                int bytesSend = handler.EndSend(ar);
                Console.WriteLine(
                    "Sent {0} bytes to Client", bytesSend);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.ToString());
            }
        }

        /*static void L2dStream_ClientConnected(object sender, ClientConnectedEventArgs e)
        {

        }

        static void L2dStream_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {

        }

        static void L2dStream_MessageReceived(object sender, MessageReceivedEventArgs e)
        {

        }*/

        static void L2dWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            ++calls;
        }

        static void L2dWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            ExitEvent();
        }

        static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        //Obsolete?
        private static void PublishServerID(string ID)
        {
            File.WriteAllText(Path.Combine(projectPath, "Lerp2API/Console/ServerID.GUID"), ID);
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
                    Lerp2API.ConsoleMessage msg = JsonUtility.FromJson<Lerp2API.ConsoleMessage>(line);
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

        private static string ForceGetParam(string[] args, string name)
        {
            foreach (string arg in args)
                if (arg.IndexOf("=") > -1 && arg.Substring(1, arg.IndexOf("=")) == name)
                    return arg.Split('=')[1];
            return "";
        }

        private static bool ForceExistParam(string[] args, string name)
        {
            return !string.IsNullOrWhiteSpace(ForceGetParam(args, name));
        }

        //Obsolete
        private static string UnpackNl(string str)
        {
            return str.Replace("\\n", Environment.NewLine);
        }

        //Obsolete
        private static string PackNl(string str)
        {
            return str.Replace(Environment.NewLine, "\\n");
        }

        private static ConsoleColor GetColor(LogType type)
        {
            switch (type)
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
            ICSharpCode.SharpZipLib.Zip.FastZip fastZip = new ICSharpCode.SharpZipLib.Zip.FastZip();

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
            foreach (string a in args)
                if (a.StartsWith("-"))
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