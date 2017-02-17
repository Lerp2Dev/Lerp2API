using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2API.Mono
{
    public class LerpedMono : MonoBehaviour
    {
        public static MonoBehaviour me;
        public static void SetInstance(LerpedMono ins)
        {
            me = ins;
        }
        public static T GetInstance<T>() where T : MonoBehaviour
        {
            if (me == null)
                Debug.LogWarningFormat("'{0}' instance is null.", typeof(T).FullName);
            return (T)me;
        }
        public void Start()
        {
            //if (!EditorApplication.isPlayingOrWillChangePlaymode)
            me = this;
        }
    }
}
