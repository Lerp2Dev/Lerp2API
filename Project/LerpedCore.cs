using Lerp2API.Communication.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;
using Random = System.Random;
using Logger = Lerp2API.SafeECalls.Logger;

namespace Lerp2API
{
    public class LerpedCore : MonoBehaviour
    {
        private static Dictionary<string, object> _storedInfo;
        private static string storePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "L2API.sav");
        public const string UnityBoot = "UNITY_STARTED_UP", enabledDebug = "ENABLE_DEBUG", loggerPath = "LOG_PATH", consoleSymLinkPath = "CONSOLE_SYMLINK_PATH",
                            defaultLogFilePath = "Logs/Console.log";
        public static GameObject lerpedCore;
        public static MonoBehaviour lerpedHook;
        public static bool socketDebug,
                           safeECallEnabled,
                           cancelSocketClient,
                           cancelSocketServer;
        public static SocketClient consoleClient;
        private static Thread curThread;
        private static bool _isEditor, _isPlaying;
        public static Logger logger = new Logger(Path.Combine(Path.GetDirectoryName(Path.Combine(Application.dataPath, defaultLogFilePath)), "api-Logger.log"));

        public static string SystemTime
        {
            get
            {
                return DateTime.Now.ToString("h:mm:ss.ffffff");
            }
        }

        public static float UnityTick
        {
            get
            {
                return Time.fixedDeltaTime;
            }
        }

        //Aki ai un vuj
        //Clever way to compare if we are in Editor or Playing, using the main thread only.
        //we only have to update the bool when we are, on the main thread. Normally, this state is not updated so many times in a minute. So, we don't have any problem.
        internal static bool IsEditor()
        {
            if (ReferenceEquals(Thread.CurrentThread, curThread))
                _isEditor = Application.isEditor;
            return _isEditor;
        }

        internal static bool IsPlaying()
        {
            if (ReferenceEquals(Thread.CurrentThread, curThread))
                _isPlaying = Application.isEditor;
            return _isPlaying;
        }
        //End clever part

        public static bool CheckUnityVersion(int mainVer, int subVer)
        {
            char[] separator = new char[] { '.' };
            int[] numArray = Application.unityVersion.Split(separator).Select(x => GetVersionValue(x)).ToArray();
            return ((mainVer < numArray[0]) || ((mainVer == numArray[0]) && (subVer <= numArray[1])));
        }

        private static int GetVersionValue(string strNumber)
        {
            int result = 0;
            int.TryParse(strNumber, out result);
            return result;
        }

        public static bool GetBool(string key)
        {
            return storedInfo.ContainsKey(key) && ((bool)storedInfo[key]);
        }

        public static void SetBool(string key, bool value)
        {
            if (!storedInfo.ContainsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        public static string GetString(string key)
        {
            if (storedInfo.ContainsKey(key))
                return (string)storedInfo[key];
            else
                return "";
        }

        public static void SetString(string key, string value)
        {
            if (!storedInfo.ContainsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        public static int GetInt(string key)
        {
            if (storedInfo.ContainsKey(key))
                return (int)storedInfo[key];
            else
                return 0;
        }

        public static void SetInt(string key, int value)
        {
            if (!storedInfo.ContainsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        public static long GetLong(string key)
        {
            if (storedInfo.ContainsKey(key))
                return (long)storedInfo[key];
            else
                return 0;
        }

        public static void SetLong(string key, long value)
        {
            if (!storedInfo.ContainsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        private static Dictionary<string, object> storedInfo
        {
            get
            {
                if (_storedInfo == null)
                    if (File.Exists(storePath))
                        _storedInfo = JSONHelpers.DeserializeFromFile<Dictionary<string, object>>(storePath);
                    else
                        _storedInfo = new Dictionary<string, object>();
                return _storedInfo;
            }
            set
            {
                _storedInfo = value;
            }
        }

        public static GameObject AutoHookCore()
        {
            GameObject core = GameObject.Find("Lerp2Core");
            if (core == null)
                core = new GameObject("Lerp2Core");
            return core;
        }

        public static void HookThis(Type type, int waitSecs = 1)
        {
            int i = DateTime.Now.Second + waitSecs; //Wait desired seconds for lerpedCore to be assigned 
            if (lerpedCore == null) lerpedCore = AutoHookCore();
            //while (lerpedCore == null && DateTime.Now.Second < i) { }
            if (lerpedCore.GetComponent(type) == null)
                lerpedCore.AddComponent(type);
        }

        public static void ChangeRequiredASM(string ASMName, string ASMFolder)
        {
            string destFile = Path.Combine(Application.streamingAssetsPath, "RequiredASM/" + ASMName + ".dll"),
                   originFile = ASMFolder + "/" + ASMName + ".dll",
                   destFolder = Path.Combine(Application.streamingAssetsPath, "RequiredASM");
            if (!File.Exists(destFile))
            {
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);
                if (File.Exists(originFile))
                    File.Copy(originFile, destFile);
            }
        }
    }

    public class ConsoleServer
    {
        private static SocketServer lerpedServer;
        private static Process consoleProc;

        public static void BeReady(string path)
        {
            if (!LerpedCore.cancelSocketServer)
            {
                lerpedServer = new SocketServer(new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts), IPAddress.Loopback, SocketServer.lerpedPort, SocketType.Stream, ProtocolType.Tcp, LerpedCore.socketDebug);
                lerpedServer.ComeAlive();

                //This is also included, because, if we don't start the Socket server with shouldn't relwase any client
                ConsoleSender.CreateInstance(path);
            }
        }

        public static void StartConsole(string path)
        { //Tendr�a q comentar esta parte
            string console = Path.Combine(Application.dataPath, "Lerp2API/Console/Lerp2Console.exe"),
                   curLoc = Assembly.GetExecutingAssembly().Location;
            if (!File.Exists(console))
            {
                Debug.LogErrorFormat("Console app couldn't be found in its default path ({0}).", console);
                return;
            }
            if (Application.isEditor)
                LerpedCore.ChangeRequiredASM("UnityEngine", Path.Combine(Path.GetDirectoryName(Application.dataPath), "Library/UnityAssemblies"));
            string[] mklinkspaths = new string[2] { Path.Combine(Path.GetDirectoryName(console), "Lerp2API.dll"), Path.Combine(Path.GetDirectoryName(console), "UnityEngine.dll") };
            bool domklink = mklinkspaths.All(x => !File.Exists(x)), 
                 redomklink = LerpedCore.GetString(LerpedCore.consoleSymLinkPath) != Path.GetDirectoryName(curLoc);
            string mklinks = (!File.Exists(mklinkspaths[0]) ? string.Format(@"mklink ""{0}"" ""{1}""", mklinkspaths[0], curLoc) : "")
                + (domklink ? " & " : "")
                + (!File.Exists(mklinkspaths[1]) ? string.Format(@"mklink ""{0}"" ""{1}""", mklinkspaths[1], Path.Combine(Application.streamingAssetsPath, "RequiredASM/UnityEngine.dll")) : "");
            if (domklink || redomklink)
            {
                LerpedCore.SetString(LerpedCore.consoleSymLinkPath, Path.GetDirectoryName(curLoc));
                using (consoleProc = new Process())
                { //This is a little bit problematic, because it can cause a crash due to a recompilation on the UnityEngine.dll (because of the smybolic link)
                    consoleProc.StartInfo.FileName = "cmd.exe";
                    consoleProc.StartInfo.Arguments = string.Format(@"/c " + mklinks);
                    consoleProc.StartInfo.UseShellExecute = true;
                    consoleProc.StartInfo.Verb = "runas";
                    consoleProc.Start();
                    consoleProc.WaitForExit();
                }
            }
            using (consoleProc = new Process())
            {
                consoleProc.StartInfo.FileName = console;
                consoleProc.StartInfo.Arguments = string.Format(@"-projectPath='{0}'{1}", Application.dataPath, Application.isEditor ? " -editor" : "").SafeArguments();
                consoleProc.Start();
                //UnityEngine.Debug.Log("Starting Console app!");
                BeReady(path);
                /*BeReady(() => {
                    UnityEngine.Debug.Log("Ready to connect!");
                    ConsoleSender.CreateInstance(path);
                });*/
            }
        }

        public static void CloseConsole()
        {
            //consoleProc.CloseMainWindow();
            //consoleProc.Close();

            //Ya que como esto no funciona, tendr� que enviarle un comando en pla, string <close_console>, y de alguna forma hacer q cuando le llegue este al servidor
            //ejecute un metodo, de la clase SocketClient (esto a atraves de s�lo el Socket, ojo), y este metodo tendr� un callback que habr� yo puesto antes, o bien,
            //podr�a hacer un callback para q cuandos e cierre se llame a esa funci�n...

            //Con este comando cierro todos los clientes...
            ConsoleSender.instance.lerpedClient.WriteLine("<close_clients>");
            ConsoleSender.instance.lerpedClient.Dispose(); 
            
            //Despu�s de esto, el servidor se autocierra
            
            //lerpedServer.CloseServer(); //Y si envio un mensaje al servidor con <stop> por si tiene que hacer algo m�s?
        }
    }

    public class ConsoleSender
    {
        //private static List<string> paths = new List<string>();
        //public static PipeClient l2dStream;
        public static ConsoleSender instance;
        public SocketClient lerpedClient;
        //private ConsoleLogger logger;
        public ConsoleSender(string path)
        {
            if (!LerpedCore.cancelSocketClient)
            {
                lerpedClient = new SocketClient(GetUnitySocketActions());
                lerpedClient.DoConnection();

                string dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (!File.Exists(path))
                    File.Create(path);

                //logger = new ConsoleLogger(path);
            }
        }
        private Action GetUnitySocketActions()
        { //Esto no sirve de na
            return () => {
                //byte[] bytes = new byte[1024];
                //UnityEngine.Debug.LogWarning(lerpedClient.ReceiveMessage(bytes));
            };
        }
        public static void CreateInstance(string path)
        {
            instance = new ConsoleSender(path);
        }
        public void SendMessage(LogType lt, string ls, string st)
        {
            //if (!paths.Contains(path))
            //    paths.Add(path);
            //File.AppendAllText(path, Environment.NewLine + JsonUtility.ToJson(new ConsoleMessage(lt, ls, st)));
            //if(l2dStream == null)
            //    l2dStream = new PipeClient(GetServerID());

            //l2dStream.SendMessage(string.Format("{0}\n{1}", ls, st));

            if (lerpedClient == null)
            {
                lerpedClient = new SocketClient();
                lerpedClient.DoConnection();
            }

            lerpedClient.WriteLine(ls);
            LerpedCore.logger.AppendLine(ls, lt.ToLoggerType());

            //Tengo que quitar el path, tengo que ver lo de los colores...
        }
        /*public static void Quit()
        {
            //Tengo q comprimir el log... Todo esto cuando tenga una carpeta q se llame logs y este codigo este dentro de la api
            //Al tener la api ya no me va a hacer falta tener q estar diciendole q se ejecute todos los eventos, ni los archivos definidos en otros scripts ni nada
            //Ni tampoco estar diciendole a esta clase donde esta el ejecutable pq debe ir siempre acompa�ado de la api al tenerlo como referencia dentro de si
            //Y todo estar mas compactado tanto las clases como los metodos...
            foreach (string p in paths)
                if (File.Exists(p))
                    File.Delete(p);
        }*/

        //Obsolete?
        /*private static string GetServerID()
        {
            string path = Path.Combine(Application.dataPath, "Lerp2API/Console/ServerID.GUID");
            if (File.Exists(path))
                return File.ReadAllText(path);
            else
            {
                Debug.LogErrorFormat("File '{0}' doesn't exists. Server ID couldn't be retrieved!", path);
                return "";
            }
        }*/
    }

    public class ConsoleMessage
    {
        public LogType logType;
        public string logString, stackTrace;
        public ConsoleMessage(LogType lt, string ls, string st)
        {
            logType = lt;
            logString = ls;
            stackTrace = st;
        }
    }

    /*public class ConsoleLogger
    {
        public string path = "";
        public ConsoleLogger(string path)
        {
            this.path = path;
        }
        public void SendLog(string msg)
        {
            File.AppendAllText(path, msg);
        }

        public void ClearLog()
        {
            File.WriteAllText(path, "");
        }
    }*/

    public enum TaskState { NotStarted, Running, Paused, Stopped }
    public class CronTask
    {
        internal static Dictionary<ulong, CronTask> curTasks = new Dictionary<ulong, CronTask>();

        public Action myAction;
        public float delay = 1;
        public float times = Mathf.Infinity;
        internal ulong id;
        private bool createdByConstructor;
        private TaskState curState;

        public static bool debugTasks;

        private bool CheckValid()
        {
            if (createdByConstructor)
                Debug.LogError("Cannot use an instance that has been created by using constructor, please, create a new instance by calling CreateInstance");
            return createdByConstructor;
        }

        public CronTask()
        {
            Debug.LogWarning("Create a new instance by using CreateInstace instead! Returning null object.");
        }

        private CronTask(bool valid)
        {
            //if (!CoroutineDatabase.database.ContainsKey()) //Basicamente queria a�adir las coroutinas desde aqui con un metodo, a partir de los metodos IEnumerator, todo esto para poder llamarla desde un comando... Logicamente solo si son publicas
                foreach (var method in typeof(CronTask).GetMethods())
                {
                    var parameters = method.GetParameters();
                    var parameterDescriptions = string.Join
                        (", ", method.GetParameters()
                                     .Select(x => x.ParameterType + " " + x.Name)
                                     .ToArray());

                    Console.WriteLine("{0} {1} ({2})",
                                      method.ReturnType,
                                      method.Name,
                                      parameterDescriptions);
                }
        }

        public static CronTask CreateInstance(Action a, float delay, float times = Mathf.Infinity, bool startInmediatly = false)
        {
            if (a == null)
            {
                Debug.LogError("Cannot create a Task with a null Action!");
                return null;
            }
            CronTask ins = new CronTask(true);
            ulong id = new Random().NextUInt64(ulong.MaxValue); //Tener cuidado con no a�adir una id que ya exista
            //runningTasks<T1>.Add(id, startInmediatly ? TaskState.Running : TaskState.NotStarted);

            ins.id = id;
            ins.myAction = a;
            ins.delay = delay;
            ins.times = times;

            if (startInmediatly)
                ins.RunDelayed();

            curTasks.Add(id, ins);

            return ins;
        }

        public void RunDelayed()
        {
            if (MultiRun(string.Format("Task with ID {0} is already running!", id)) && debugTasks)
                Debug.LogFormat("Task with ID {0} started succesfully at {1}!", id, LerpedCore.SystemTime);
        }

        public void Resume()
        {
            if (MultiRun(string.Format("Task with ID {0} isn't paused!", id), TaskState.Paused, TaskState.Running) && debugTasks)
                Debug.LogFormat("Task with ID {0} paused succesfully at {1}!", id, LerpedCore.SystemTime);
        }

        public void Stop()
        {
            if (MultiRun(string.Format("Task with ID {0} isn't stated to be stopped!", id), TaskState.Running, TaskState.Stopped) && debugTasks)
                Debug.LogFormat("Task with ID {0} stopped succesfully at {1}!", id, LerpedCore.SystemTime);
        }

        private bool MultiRun(string errorMsg = "", TaskState ifis = TaskState.NotStarted, TaskState setas = TaskState.Running)
        {
            if (curState == ifis)
            {
                curState = setas;
                if (LerpedCore.safeECallEnabled)
                    RunSafe(LerpedCore.consoleClient);
                else
                    RunWild();
                return true;
            }
            else
                Debug.LogError(errorMsg);
            return true;
        }

        private void RunSafe(SocketClient client)
        {
            /*new StackTrace(true).GetFrames().ForEach((frame) => {
                Console.WriteLine("{0} {1}", frame.GetFileName(), frame.GetFileLineNumber());
            });*/
            if (client != null)
                client.WriteLine("/runcoroutine InternalCoroutine");
        }

        private void RunWild()
        {
                if (LerpedCore.lerpedCore != null)
                {
                    if (debugTasks)
                        UnityEngine.Debug.Log("Running this task in Unity!");
                    LerpedCore.lerpedHook.StartCoroutine(InternalCoroutine());
                }
                else
                    Debug.LogError("Cannot run tasks in the Editor!");
        }

        private IEnumerator InternalCoroutine()
        {
            ulong i = 0; //Puede ser que llegue un momento en el que pete?
            while (i < times && IsValidState(curState))
            {
                yield return (curState != TaskState.Stopped);
                myAction();
                yield return new WaitForSeconds(delay);
            }
            //Aqui tenemos que borrar esa ID de los 2 diccionarios
            if (curState == TaskState.Stopped)
                curTasks.Remove(id);
        }

        private static bool IsValidState(TaskState state)
        {
            return state != TaskState.NotStarted && state != TaskState.Stopped;
        }

        public static bool ExistsTask(ulong id)
        {
            return curTasks.ContainsKey(id);
        }

        public static CronTask GetTask(ulong id)
        {
            if (!ExistsTask(id))
            {
                Debug.LogErrorFormat("There isn't any task with ID {0}", id);
                return null;
            }
            return curTasks[id];
        }
    }

    public class CoroutineDatabase
    {
        public static Dictionary<string, CoroutineEntity> database = new Dictionary<string, CoroutineEntity>();
    }

    public class CoroutineEntity
    {
        public string name;
        public IEnumerator body;

        public CoroutineEntity(string n, IEnumerator b)
        {
            name = n;
            body = b;
        }
    }

    /*    public class CronTask<T1> : LerpedCore
        {
            internal static Dictionary<ulong, object> curTasks = new Dictionary<ulong, object>();

            public Action<T1> myAction;
            public T1 obj1;
            public float delay = 1;
            public float times = Mathf.Infinity;
            internal ulong id;
            private bool createdByConstructor;
            private TaskState curState;

            public static bool debugTasks;

            private bool CheckValid()
            {
                if (createdByConstructor)
                    Debug.LogError("Cannot use an instance that has been created by using constructor, please, create a new instance by calling CreateInstance");
                return createdByConstructor;
            }

            public CronTask()
            {
                Debug.LogWarning("Create a new instance by using CreateInstace instead! Returning null object.");
            }

            private CronTask(bool valid)
            {

            }

            public static CronTask<T1> CreateInstance(Action<T1> a, T1 obj, float delay, float times = Mathf.Infinity, bool startInmediatly = false)
            {
                if (a == null)
                {
                    Debug.LogError("Cannot create a Task with a null Action!");
                    return null;
                }
                CronTask<T1> ins = new CronTask<T1>(true);
                ulong id = new Random().NextUInt64(ulong.MaxValue); //Tener cuidado con no a�adir una id que ya exista
                //runningTasks<T1>.Add(id, startInmediatly ? TaskState.Running : TaskState.NotStarted);

                curTasks.Add(id, a);
                ins.id = id;
                ins.myAction = a;
                ins.delay = delay;
                ins.times = times;
                ins.obj1 = obj;

                if (startInmediatly)
                    ins.RunDelayed();

                return ins;
            }

            public void RunDelayed()
            {
                if (MultiRun(string.Format("Task with ID {0} is already running!", id)) && debugTasks)
                    Debug.LogFormat("Task with ID {0} started succesfully at {1}!", id, SystemTime);
            }

            public void Resume()
            {
                if (MultiRun(string.Format("Task with ID {0} isn't paused!", id), TaskState.Paused, TaskState.Running) && debugTasks)
                    Debug.LogFormat("Task with ID {0} paused succesfully at {1}!", id, SystemTime);
            }

            public void Stop()
            {
                if (MultiRun(string.Format("Task with ID {0} isn't stated to be stopped!", id), TaskState.Running, TaskState.Stopped) && debugTasks)
                    Debug.LogFormat("Task with ID {0} stopped succesfully at {1}!", id, SystemTime);
            }

            private bool MultiRun(string errorMsg = "", TaskState ifis = TaskState.NotStarted, TaskState setas = TaskState.Running)
            {
                if(curState == ifis)
                {
                    curState = setas;
                    Run();
                    return true;
                }
                else
                    Debug.LogError(errorMsg);
                return true;
            }

            private void Run()
            {
                if (lerpedCore != null)
                    ((MonoBehaviour)lerpedCore.GetComponent(Type.GetType("LerpedHook"))).StartCoroutine(InternalCoroutine());
                else
                    Debug.LogError("Cannot run tasks in the Editor!");
            }

            private IEnumerator InternalCoroutine()
            {
                ulong i = 0; //Puede ser que llegue un momento en el que pete?
                while (i < times && IsValidState(curState))
                {
                    yield return (curState != TaskState.Stopped);
                    myAction(obj1);
                    yield return new WaitForSeconds(delay);
                }
                //Aqui tenemos que borrar esa ID de los 2 diccionarios
                if (curState == TaskState.Stopped)
                    curTasks.Remove(id);
            }

            private static bool IsValidState(TaskState state)
            {
                return state != TaskState.NotStarted && state != TaskState.Stopped;
            }

            public static bool ExistsTask(ulong id)
            {
                return curTasks.ContainsKey(id);
            }

            public static CronTask<T> GetTask<T>(ulong id)
            {
                if(!ExistsTask(id))
                {
                    Debug.LogErrorFormat("There isn't any task with ID {0}", id);
                    return null;
                }
                return (CronTask<T>)curTasks[id];
            }
        }*/
}