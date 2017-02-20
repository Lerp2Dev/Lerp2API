using Lerp2API;
using Lerp2API.Mono;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2Raw
{
    public class LerpedHook : LerpedMono
    {
        private void Awake() //This must be ondisable, only for hooklog
        {
            LerpedCore.lerpedCore = LerpedCore.AutoHookCore();
            Debug.HookLog();
        }

        private void OnDisable()
        {
            Debug.UnhookLog();
        }
    }
}
