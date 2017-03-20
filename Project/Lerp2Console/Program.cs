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

        /*
         * 
         * TODO:
         
            - Command system, inherited from lerp2api
             
        */

        //... -path=C:/.../ -file=example.txt
        static void Main(string[] args)
        { //Check when Unity button closes up...
            Console.Title = "Lerp2Dev Console";

            //Console.WriteLine(new StackTrace(true).GetFrames().Last().GetFileName());
            LerpedCore.safeECallEnabled = true;

            parameters = Parameter.GetParams(args);

            projectPath = GetParam("projectPath");
            stacktrace = !ExistParam("nostacktrace");
            editor = ExistParam("editor");

            Work(args[0].UnsafeArguments());

            Console.Read();
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

            listenPath = Path.Combine(projectPath, sPath[0]); //string.IsNullOrWhiteSpace(lFile) ? "debug.log" : lFile; //Tengo que probar a renombrar el archivo a ver que pasa...
            listenFile = Path.Combine(listenPath, sPath[1]);
        }

        private static Action WriteReceived()
        {
            return () => {
                byte[] bytes = new byte[1024];
                Console.WriteLine(lerpedSocketConsoleClient.ReceiveMessage(bytes));
            };
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

        private static string GetParam(string name)
        {
            Parameter p = parameters.FirstOrDefault(x => x.name == name);
            return p != null ? p.value : "";
        }

        private static bool ExistParam(string name)
        {
            return parameters.FirstOrDefault(x => x.name == name) != null;
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