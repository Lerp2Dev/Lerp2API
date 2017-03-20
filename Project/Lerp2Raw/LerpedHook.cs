using Lerp2API;
using System.IO;
using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2Raw
{
    public class LerpedHook : MonoBehaviour
    {
        public static LerpedHook me;
        public bool m_runConsoleAtPlayEvent = true, 
                    m_socketDebug = true,
                    m_cronTasksDebug = true,
                    m_disableClientSocket,
                    m_disableServerSocket;
        public string m_defLogPath = "";
        private void Awake() //This must be onenable, only for hook log
        {
            me = this;
            LerpedCore.lerpedHook = this;
            LerpedCore.socketDebug = m_socketDebug;
            CronTask.debugTasks = m_cronTasksDebug;
            LerpedCore.cancelSocketClient = m_disableClientSocket;
            LerpedCore.cancelSocketServer = m_disableServerSocket;
            LerpedCore.lerpedCore = LerpedCore.AutoHookCore();
            Debug.HookLog();
            if (m_runConsoleAtPlayEvent) ConsoleServer.StartConsole(!string.IsNullOrEmpty(m_defLogPath) ? m_defLogPath : Path.Combine(Application.dataPath, LerpedCore.defaultLogFilePath));
        }

        private void OnDisable()
        {
            //UnityEngine.Debug.Log("Unhooking log!");
            Debug.UnhookLog(); //Aquí tengo que guardar todo y mandarle la solicitud a la consola
        }
    }
}
