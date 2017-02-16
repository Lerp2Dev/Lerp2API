using Lerp2API.Mono;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2Raw
{
    public abstract class LerpedHook : LerpedMono
    {
        private void Awake() //This must be ondisable, only for hooklog
        {
            Debug.HookLog();
        }

        private void OnDisable()
        {
            Debug.UnhookLog();
        }
    }
}
