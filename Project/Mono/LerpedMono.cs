using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2API.Mono
{
    /// <summary>
    /// Class LerpedMono.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class LerpedMono : MonoBehaviour
    {
        /// <summary>
        /// Me
        /// </summary>
        public static MonoBehaviour me;

        /// <summary>
        /// Sets the instance.
        /// </summary>
        /// <param name="ins">The ins.</param>
        public static void SetInstance(LerpedMono ins)
        {
            me = ins;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public static T GetInstance<T>() where T : MonoBehaviour
        {
            if (me == null)
                Debug.LogWarningFormat("'{0}' instance is null.", typeof(T).FullName);
            return (T)me;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            //if (!EditorApplication.isPlayingOrWillChangePlaymode)
            me = this;
        }
    }
}