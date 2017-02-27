using Lerp2API;
using System.IO;
//using Lerp2API.Mono;
using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2Raw
{
    public class LerpedHook : MonoBehaviour
    {
        public bool m_runConsoleAtPlayEvent = true;
        public string m_defLogPath = "";
        private void Awake() //This must be onenable, only for hook log
        {
            LerpedCore.lerpedCore = LerpedCore.AutoHookCore();
            Debug.HookLog(!string.IsNullOrEmpty(m_defLogPath) ? m_defLogPath : Path.Combine(Application.dataPath, LerpedCore.defaultLogFilePath));
            if (m_runConsoleAtPlayEvent) ConsoleListener.StartConsole();
        }

        private void OnDisable()
        {
            UnityEngine.Debug.Log("Unhooking log!");
            Debug.UnhookLog(); //Aquí tengo que guardar todo y mandarle la solicitud a la consola
        }
    }
}
