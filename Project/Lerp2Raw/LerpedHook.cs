using Lerp2API;
using Lerp2API.Utility;
using System.IO;
using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2Raw
{
    /// <summary>
    /// Class LerpedHook.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class LerpedHook : MonoBehaviour
    {
        /// <summary>
        /// Me
        /// </summary>
        public static LerpedHook me;

        /// <summary>
        /// The m run console at play event
        /// </summary>
        public bool m_runConsoleAtPlayEvent = false, //There is a bug...
                                                     /// <summary>
                                                     /// The m socket debug
                                                     /// </summary>
                    m_socketDebug = true,
                    /// <summary>
                    /// The m cron tasks debug
                    /// </summary>
                    m_cronTasksDebug = true,
                    /// <summary>
                    /// The m disable client socket
                    /// </summary>
                    m_disableClientSocket,
                    /// <summary>
                    /// The m disable server socket
                    /// </summary>
                    m_disableServerSocket,
                    m_disableFileSystemWatcher;

        /// <summary>
        /// The m definition log path
        /// </summary>
        public string m_defLogPath = "";

        private void Awake() //This must be onenable, only for hook log
        {
            me = this;
            LerpedCore.lerpedHook = this;
            LerpedCore.socketDebug = m_socketDebug;
            CronTask.debugTasks = m_cronTasksDebug;
            LerpedCore.cancelSocketClient = m_disableClientSocket;
            LerpedCore.cancelSocketServer = m_disableServerSocket;
            LerpedCore.disableFileSystemWatcher = m_disableFileSystemWatcher;
            LerpedCore.lerpedCore = LerpedCore.AutoHookCore();
            Debug.HookLog();
            if (m_runConsoleAtPlayEvent) ConsoleServer.StartConsole(!string.IsNullOrEmpty(m_defLogPath) ? m_defLogPath : Path.Combine(Application.dataPath, LerpedCore.defaultLogFilePath));
        }

        private void OnEnable()
        {
            //LerpedInputs.LoadAxis();
        }

        private void OnDisable()
        {
            //LerpedInputs.SaveAxis();

            //UnityEngine.Debug.Log("Unhooking log!");
            Debug.UnhookLog();

            //Aquí tengo que guardar todo y mandarle la solicitud a la consola
            if (m_runConsoleAtPlayEvent)
                ConsoleServer.CloseConsole();
        }
    }
}