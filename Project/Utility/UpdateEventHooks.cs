using System;
using UnityEngine;

namespace Lerp2API.Utility
{
    /// <summary>
    /// Class UpdateEventHooks.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [AddComponentMenu("Lerp2Dev Team Tools/Hooks/UpdateEventHooks")]
    public class UpdateEventHooks : MonoBehaviour
    {
        /// <summary>
        /// Occurs when [update hook].
        /// </summary>
        public event EventHandler UpdateHook;
        /// <summary>
        /// Occurs when [fixed update hook].
        /// </summary>
        public event EventHandler FixedUpdateHook;
        /// <summary>
        /// Occurs when [late update hook].
        /// </summary>
        public event EventHandler LateUpdateHook;

        // Update is called once per frame
        void Update()
        {
            UpdateHook?.Invoke(this, EventArgs.Empty);
        }

        void FixedUpdate()
        {
            FixedUpdateHook?.Invoke(this, EventArgs.Empty);
        }

        void LateUpdate()
        {
            LateUpdateHook?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when [destroy].
        /// </summary>
        protected void OnDestroy()
        {
            UpdateHook = null;
            FixedUpdateHook = null;
            LateUpdateHook = null;
        }
    }
}
