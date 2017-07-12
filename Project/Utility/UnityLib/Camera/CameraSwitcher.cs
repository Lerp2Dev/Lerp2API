// Camera switcher, https://forum.unity3d.com/threads/how-can-i-switch-between-multiple-cameras-using-one-key-click.472009/
// usage: Assign cameras into the array, press C to switch into next camera

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityLibrary
{
    /// <summary>
    /// Class CameraSwitcher.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class CameraSwitcher : MonoBehaviour
    {
        /// <summary>
        /// The cameras
        /// </summary>
        public Camera[] cameras;

        private int currentCamera = 0;

        private void Awake()
        {
            if (cameras == null || cameras.Length == 0)
            {
                Debug.LogError("No cameras assigned..", gameObject);
                this.enabled = false;
            }

            EnableOnlyFirstCamera();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                // disable current
                cameras[currentCamera].enabled = false;

                // increment index and wrap after finished array
                currentCamera = (currentCamera + 1) % cameras.Length;

                // enable next
                cameras[currentCamera].enabled = true;
            }
        }

        private void EnableOnlyFirstCamera()
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                // only returns true when i is 0
                cameras[i].enabled = (i == 0);
            }
        }
    }
}