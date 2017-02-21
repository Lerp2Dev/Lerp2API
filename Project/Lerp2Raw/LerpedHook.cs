using Lerp2API;
using Lerp2API.Mono;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2Raw
{
    public class LerpedHook : LerpedMono
    {
        public bool m_runConsoleAtPlayEvent = true;
        private void Awake() //This must be onenable, only for hook log
        {
            LerpedCore.lerpedCore = LerpedCore.AutoHookCore();
            Debug.HookLog();
            if (m_runConsoleAtPlayEvent) ConsoleListener.StartConsole();
        }

        private void OnDisable()
        {
            Debug.UnhookLog();
        }
    }
}
