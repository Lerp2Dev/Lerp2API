using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2API
{
    public class LerpedHook : MonoBehaviour
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
