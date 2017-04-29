using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lerp2API;
using ICSharpCode.SharpZipLib.Zip;
using Lerp2API.Communication.Sockets;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Lerp2API.SafeECalls;
using System.Runtime.InteropServices;
//using ClientServerUsingNamedPipes.Server;
//using ClientServerUsingNamedPipes.Interfaces;

namespace Lerp2Console
{
    class Program
    {
        private static SocketClient lerpedSocketConsoleClient;
        //private static SocketClient signalClient;
        private static string projectPath = "", listenPath = "", listenFile = "";
        private static Parameter[] parameters;
        private const int msWait = 500;
        private static bool stacktrace = true, editor = false;
        private static string[] sPath = LerpedCore.defaultLogFilePath.Split('/');
        private static bool isClosing;

        /*
         * 
         * TODO:
         
            - Command system, inherited from lerp2api
             
        */

        //... -path=C:/.../ -file=example.txt
        static void Main(string[] args) //Esto siempre será una cadena única
        { //Check when Unity button closes up...
            Console.Title = "Lerp2Dev Console";

            //Console.WriteLine(new StackTrace(true).GetFrames().Last().GetFileName());
            LerpedCore.safeECallEnabled = true;

            parameters = Parameter.GetParams(args[0]);

            projectPath = GetParam("projectPath");
            stacktrace = !ExistParam("nostacktrace");
            editor = ExistParam("editor");

            Work(args[0].UnsafeArguments());

            Console.Read();
        }

        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate.

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        static void Work(string[] args)
        {
            /* = new SocketServer();

            l2dServer.ComeAlive();
            l2dServer.StartListening();

            l2dServer.ServerCallback = new AsyncCallback(AcceptCallback);*/

            lerpedSocketConsoleClient = new SocketClient(WriteReceived());

            LerpedCore.consoleClient = lerpedSocketConsoleClient;

            lerpedSocketConsoleClient.DoConnection();

            //Console.WriteLine(projectPath);

            listenPath = Path.Combine(projectPath, sPath[0]); //string.IsNullOrWhiteSpace(lFile) ? "debug.log" : lFile; //Tengo que probar a renombrar el archivo a ver que pasa...
            listenFile = Path.Combine(listenPath, sPath[1]);

            //Console.WriteLine(listenFile);
        }

        private static Action WriteReceived()
        {
            return () =>
            {
                string str = "";
                if (lerpedSocketConsoleClient.ReceiveMessage(out str))
                {
                    SocketMessage sm = JsonUtility.FromJson<SocketMessage>(str);
                    //Console.WriteLine("My ID: {0}, Id received: {1}\nMessage: {2}", lerpedSocketConsoleClient.Id, sm.id, sm.msg);
                    Console.WriteLine("{0}: {1}", DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss"), sm.msg);
                }
                else
                    ExitEvent(false);
            };
        }

        private static void ExitEvent(bool ic)
        {
            //We create a compressed version of the log before we close...
            CreateCompressedLog();

            lerpedSocketConsoleClient.Dispose();

            //And finally, we end up everything...
            if(!ic) //If we aren't closing the console, we need to close it.
                Environment.Exit(0);
        }

        private static void CreateCompressedLog()
        {
            FastZip fastZip = new FastZip();

            string now = DateTime.Now.ToString("yyyy-MM-dd");
            int count = new DirectoryInfo(listenPath).GetFiles(string.Format("{0}*.{1}", now, sPath[1].Split('.')[1])).Length;

            fastZip.CreateZip(string.Format("{0}{1}.gz", now, (count > 0 ? "-" + (count - 1).ToString() : "")), listenPath, false, @"\.log$");
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

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            // Put your own handler here

            switch (ctrlType)
            {
                case CtrlTypes.CTRL_CLOSE_EVENT:
                    isClosing = true;
                    Console.WriteLine("Program being closed!");
                    ExitEvent(isClosing);
                    break;
            }

            return true;
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
        public static Parameter[] GetParams(string arg)
        {
            string[] decargs = arg.UnsafeArguments().Select(x => x.Replace("'", "")).ToArray();
            List<Parameter> prms = new List<Parameter>();
            foreach (string a in decargs)
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