using System;
using UnityEngine;

namespace Lerp2API.Utility
{
    [AddComponentMenu("Lerp2Dev Team Tools/Hooks/UpdateEventHooks")]
    public class UpdateEventHooks : MonoBehaviour
    {
        public event EventHandler UpdateHook;
        public event EventHandler FixedUpdateHook;
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

        protected void OnDestroy()
        {
            UpdateHook = null;
            FixedUpdateHook = null;
            LateUpdateHook = null;
        }
    }
}
