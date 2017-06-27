using Lerp2API.Communication.Sockets;
using Lerp2API.Hepers.Debug_Utils;
using Lerp2API.Hepers.JSON_Extensions;
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
using Debug = Lerp2API._Debug.Debug;
using Logger = Lerp2API.SafeECalls.Logger;
using Random = System.Random;

namespace Lerp2API
{
    /// <summary>
    /// Class LerpedCore.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class LerpedCore : MonoBehaviour
    {
        private static Dictionary<string, object> _storedInfo;
        private static string storePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "L2API.sav");

        /// <summary>
        /// The unity boot
        /// </summary>
        public const string UnityBoot = "UNITY_STARTED_UP",
                            enabledDebug = "ENABLE_DEBUG",
                            loggerPath = "LOG_PATH",
                            consoleSymLinkPath = "CONSOLE_SYMLINK_PATH",
                            disabledMissingAssets = "DISABLE_MISSING_ASSETS",
                            disableFileSysWatcher = "DISABLE_FSW",
                            disableTagCheck = "DISABLE_TAG_CHECK",
                            defaultLogFilePath = "Logs/Console.log";

        /// <summary>
        /// The lerped core
        /// </summary>
        public static GameObject lerpedCore;

        /// <summary>
        /// The lerped hook
        /// </summary>
        public static MonoBehaviour lerpedHook;

        /// <summary>
        /// The socket debug
        /// </summary>
        public static bool socketDebug,
                           /// <summary>
                           /// The safe e call enabled
                           /// </summary>
                           safeECallEnabled,
                           /// <summary>
                           /// The cancel socket client
                           /// </summary>
                           cancelSocketClient,
                           /// <summary>
                           /// The cancel socket server
                           /// </summary>
                           cancelSocketServer;

        /// <summary>
        /// The console client
        /// </summary>
        public static SocketClient consoleClient;

        private static Thread curThread;

        //private static bool _isEditor, _isPlaying;
        /// <summary>
        /// The logger
        /// </summary>
        public static Logger logger = new Logger(Path.Combine(Path.GetDirectoryName(Path.Combine(Application.dataPath, defaultLogFilePath)), "api-Logger.log"));

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mssing assets disabled.
        /// </summary>
        /// <value><c>true</c> if this instance is mssing assets disabled; otherwise, <c>false</c>.</value>
        public static bool isMssingAssetsDisabled
        {
            get
            {
                if (ExistsKey(disabledMissingAssets))
                    return GetBool(disabledMissingAssets);
                else
                    return true;
            }
            set
            {
                SetBool(disabledMissingAssets, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is FSW disabled.
        /// </summary>
        /// <value><c>true</c> if this instance is FSW disabled; otherwise, <c>false</c>.</value>
        public static bool isFSWDisabled
        {
            get
            {
                if (ExistsKey(disableFileSysWatcher))
                    return GetBool(disableFileSysWatcher);
                else
                    return true;
            }
            set
            {
                SetBool(disableFileSysWatcher, value);
            }
        }

        /// <summary>
        /// Gets the system time.
        /// </summary>
        /// <value>The system time.</value>
        public static string SystemTime
        {
            get
            {
                return DateTime.Now.ToString("h:mm:ss.ffffff");
            }
        }

        /// <summary>
        /// Gets the unity tick.
        /// </summary>
        /// <value>The unity tick.</value>
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
        { //Esto lo hice así por temas de seguridad ahora por alguna razón, no lo puedo hacer así. O al menos desde donde lo llamo, me dice que no es el thread actual. Y no debería ser así.
            /*if (ReferenceEquals(Thread.CurrentThread, curThread))
                _isEditor = Application.isEditor;
            return _isEditor;*/
            return Application.isEditor;
        } //Esto lo hago así porque cuando llamaba al Log desde ciertos sitios daba problema.

        internal static bool IsPlaying()
        {
            /*if (ReferenceEquals(Thread.CurrentThread, curThread))
                _isPlaying = Application.isEditor;
            return _isPlaying;*/
            return Application.isPlaying;
        }

        //End clever part

        /// <summary>
        /// Checks the unity version.
        /// </summary>
        /// <param name="mainVer">The main ver.</param>
        /// <param name="subVer">The sub ver.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// Gets the bool.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetBool(string key)
        {
            return ExistsKey(key) && ((bool)storedInfo[key]);
        }

        /// <summary>
        /// Sets the bool.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetBool(string key, bool value)
        {
            if (!ExistsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public static string GetString(string key)
        {
            if (ExistsKey(key))
                return (string)storedInfo[key];
            else
                return "";
        }

        /// <summary>
        /// Sets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void SetString(string key, string value)
        {
            if (!ExistsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Int32.</returns>
        public static int GetInt(string key)
        {
            if (ExistsKey(key))
                return (int)storedInfo[key];
            else
                return 0;
        }

        /// <summary>
        /// Sets the int.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void SetInt(string key, int value)
        {
            if (!ExistsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        /// <summary>
        /// Gets the long.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.Int64.</returns>
        public static long GetLong(string key)
        {
            if (ExistsKey(key))
                return (long)storedInfo[key];
            else
                return 0;
        }

        /// <summary>
        /// Sets the long.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void SetLong(string key, long value)
        {
            if (!ExistsKey(key))
                storedInfo.Add(key, value);
            else
                storedInfo[key] = value;
            JSONHelpers.SerializeToFile(storePath, storedInfo, true);
        }

        /// <summary>
        /// Existses the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ExistsKey(string key)
        {
            return storedInfo.ContainsKey(key);
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

        /// <summary>
        /// Automatics the hook core.
        /// </summary>
        /// <returns>GameObject.</returns>
        public static GameObject AutoHookCore()
        {
            GameObject core = GameObject.Find("Lerp2Core");
            if (core == null)
                core = new GameObject("Lerp2Core");
            return core;
        }

        /// <summary>
        /// Hooks the this.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="waitSecs">The wait secs.</param>
        public static void HookThis(Type type, int waitSecs = 1)
        {
            int i = DateTime.Now.Second + waitSecs; //Wait desired seconds for lerpedCore to be assigned
            if (lerpedCore == null) lerpedCore = AutoHookCore();
            //while (lerpedCore == null && DateTime.Now.Second < i) { }
            if (lerpedCore.GetComponent(type) == null)
                lerpedCore.AddComponent(type);
        }

        /// <summary>
        /// Changes the required asm.
        /// </summary>
        /// <param name="ASMName">Name of the asm.</param>
        /// <param name="ASMFolder">The asm folder.</param>
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

    /// <summary>
    /// Class ConsoleServer.
    /// </summary>
    public class ConsoleServer
    {
        private static SocketServer lerpedServer;
        private static Process consoleProc;

        /// <summary>
        /// Bes the ready.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void BeReady(string path)
        {
            if (!LerpedCore.cancelSocketServer)
            {
                lerpedServer = new SocketServer(new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts), IPAddress.Loopback, SocketServer.lerpedPort, SocketType.Stream, ProtocolType.Tcp, LerpedCore.socketDebug, "server-Logger.log");
                lerpedServer.ComeAlive();

                //This is also included, because, if we don't start the Socket server with shouldn't relwase any client
                ConsoleSender.CreateInstance(path);
            }
        }

        /// <summary>
        /// Starts the console.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void StartConsole(string path)
        { //Tendría q comentar esta parte
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

        /// <summary>
        /// Closes the console.
        /// </summary>
        public static void CloseConsole()
        {
            //consoleProc.CloseMainWindow();
            //consoleProc.Close();

            //Ya que como esto no funciona, tendré que enviarle un comando en pla, string <close_console>, y de alguna forma hacer q cuando le llegue este al servidor
            //ejecute un metodo, de la clase SocketClient (esto a atraves de sólo el Socket, ojo), y este metodo tendrá un callback que habré yo puesto antes, o bien,
            //podría hacer un callback para q cuandos e cierre se llame a esa función...

            //Con este comando cierro todos los clientes...
            ConsoleSender.instance.lerpedClient.WriteLine("<close_clients>");
            ConsoleSender.instance.lerpedClient.Dispose();

            //Después de esto, el servidor se autocierra

            //lerpedServer.CloseServer(); //Y si envio un mensaje al servidor con <stop> por si tiene que hacer algo más?
        }
    }

    /// <summary>
    /// Class ConsoleSender.
    /// </summary>
    public class ConsoleSender
    {
        //private static List<string> paths = new List<string>();
        //public static PipeClient l2dStream;
        /// <summary>
        /// The instance
        /// </summary>
        public static ConsoleSender instance;

        /// <summary>
        /// The lerped client
        /// </summary>
        public SocketClient lerpedClient;

        //private ConsoleLogger logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleSender"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public ConsoleSender(string path)
        {
            if (!LerpedCore.cancelSocketClient)
            {
                lerpedClient = new SocketClient(GetUnitySocketActions(), "unityClient-Logger.log");
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
            return () =>
            {
                //byte[] bytes = new byte[1024];
                //UnityEngine.Debug.LogWarning(lerpedClient.ReceiveMessage(bytes));
            };
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void CreateInstance(string path)
        {
            instance = new ConsoleSender(path);
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="lt">The lt.</param>
        /// <param name="ls">The ls.</param>
        /// <param name="st">The st.</param>
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
                lerpedClient = new SocketClient("unityCient-Logger.log");
                lerpedClient.DoConnection();
            }

            lerpedClient.WriteLine(ls);
            LerpedCore.logger.AppendLine(ls.DetailedMessage(lt.ToLoggerType()));

            //Tengo que quitar el path, tengo que ver lo de los colores...
        }

        /*public static void Quit()
        {
            //Tengo q comprimir el log... Todo esto cuando tenga una carpeta q se llame logs y este codigo este dentro de la api
            //Al tener la api ya no me va a hacer falta tener q estar diciendole q se ejecute todos los eventos, ni los archivos definidos en otros scripts ni nada
            //Ni tampoco estar diciendole a esta clase donde esta el ejecutable pq debe ir siempre acompañado de la api al tenerlo como referencia dentro de si
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

    /// <summary>
    /// Class ConsoleMessage.
    /// </summary>
    public class ConsoleMessage
    {
        /// <summary>
        /// The log type
        /// </summary>
        public LogType logType;

        /// <summary>
        /// The log string
        /// </summary>
        public string logString, stackTrace;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleMessage"/> class.
        /// </summary>
        /// <param name="lt">The lt.</param>
        /// <param name="ls">The ls.</param>
        /// <param name="st">The st.</param>
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

    /// <summary>
    /// Class CronTask.
    /// </summary>
    public class CronTask
    {
        internal static Dictionary<ulong, CronTask> curTasks = new Dictionary<ulong, CronTask>();

        /// <summary>
        /// My action
        /// </summary>
        public Action myAction;

        /// <summary>
        /// The delay
        /// </summary>
        public float delay = 1;

        /// <summary>
        /// The times
        /// </summary>
        public float times = Mathf.Infinity;

        internal ulong id;
        private bool createdByConstructor;
        private TaskState curState;

        /// <summary>
        /// The debug tasks
        /// </summary>
        public static bool debugTasks;

        private bool CheckValid()
        {
            if (createdByConstructor)
                Debug.LogError("Cannot use an instance that has been created by using constructor, please, create a new instance by calling CreateInstance");
            return createdByConstructor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CronTask"/> class.
        /// </summary>
        public CronTask()
        {
            Debug.LogWarning("Create a new instance by using CreateInstace instead! Returning null object.");
        }

        private CronTask(bool valid)
        {
            //if (!CoroutineDatabase.database.ContainsKey()) //Basicamente queria añadir las coroutinas desde aqui con un metodo, a partir de los metodos IEnumerator, todo esto para poder llamarla desde un comando... Logicamente solo si son publicas
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

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="times">The times.</param>
        /// <param name="startInmediatly">if set to <c>true</c> [start inmediatly].</param>
        /// <returns>CronTask.</returns>
        public static CronTask CreateInstance(Action a, float delay, float times = Mathf.Infinity, bool startInmediatly = false)
        {
            if (a == null)
            {
                Debug.LogError("Cannot create a Task with a null Action!");
                return null;
            }
            CronTask ins = new CronTask(true);
            ulong id = new Random().NextUInt64(ulong.MaxValue); //Tener cuidado con no añadir una id que ya exista
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

        /// <summary>
        /// Runs the delayed.
        /// </summary>
        public void RunDelayed()
        {
            if (MultiRun(string.Format("Task with ID {0} is already running!", id)) && debugTasks)
                Debug.LogFormat("Task with ID {0} started succesfully at {1}!", id, LerpedCore.SystemTime);
        }

        /// <summary>
        /// Resumes this instance.
        /// </summary>
        public void Resume()
        {
            if (MultiRun(string.Format("Task with ID {0} isn't paused!", id), TaskState.Paused, TaskState.Running) && debugTasks)
                Debug.LogFormat("Task with ID {0} paused succesfully at {1}!", id, LerpedCore.SystemTime);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
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

        /// <summary>
        /// Existses the task.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ExistsTask(ulong id)
        {
            return curTasks.ContainsKey(id);
        }

        /// <summary>
        /// Gets the task.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>CronTask.</returns>
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

    /// <summary>
    /// Class CoroutineDatabase.
    /// </summary>
    public class CoroutineDatabase
    {
        /// <summary>
        /// The database
        /// </summary>
        public static Dictionary<string, CoroutineEntity> database = new Dictionary<string, CoroutineEntity>();
    }

    /// <summary>
    /// Class CoroutineEntity.
    /// </summary>
    public class CoroutineEntity
    {
        /// <summary>
        /// The name
        /// </summary>
        public string name;

        /// <summary>
        /// The body
        /// </summary>
        public IEnumerator body;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineEntity"/> class.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="b">The b.</param>
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
                ulong id = new Random().NextUInt64(ulong.MaxValue); //Tener cuidado con no añadir una id que ya exista
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