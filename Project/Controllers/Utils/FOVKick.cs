using System;
using System.Collections;
using UnityEngine;

namespace Lerp2Assets.Utility
{
    /// <summary>
    /// Class FOVKick.
    /// </summary>
    [Serializable]
    public class FOVKick
    {
        //[NonSerialized]
        /// <summary>
        /// The camera
        /// </summary>
        public Camera Camera;           // optional camera setup, if null the main camera will be used

        /// <summary>
        /// The original fov
        /// </summary>
        [HideInInspector]
        public float originalFov;     // the original fov

        /// <summary>
        /// The fov increase
        /// </summary>
        public float FOVIncrease = 3f;                  // the amount the field of view increases when going into a run

        /// <summary>
        /// The time to increase
        /// </summary>
        public float TimeToIncrease = 1f;               // the amount of time the field of view will increase over

        /// <summary>
        /// The time to decrease
        /// </summary>
        public float TimeToDecrease = 1f;               // the amount of time the field of view will take to return to its original size

        //[NonSerialized]
        /// <summary>
        /// The increase curve
        /// </summary>
        public AnimationCurve IncreaseCurve;

        /// <summary>
        /// Setups the specified camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        public void Setup(Camera camera)
        {
            CheckStatus(camera);
            Camera = camera;
            originalFov = camera.fieldOfView;
        }

        private void CheckStatus(Camera camera)
        {
            if (camera == null)
                throw new Exception("FOVKick camera is null, please supply the camera to the constructor");
            if (IncreaseCurve == null)
                throw new Exception("FOVKick Increase curve is null, please define the curve for the field of view kicks");
        }

        /// <summary>
        /// Changes the camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        public void ChangeCamera(Camera camera)
        {
            Camera = camera;
        }

        /// <summary>
        /// Fovs the kick up.
        /// </summary>
        /// <returns>IEnumerator.</returns>
        public IEnumerator FOVKickUp()
        {
            float t = Mathf.Abs((Camera.fieldOfView - originalFov) / FOVIncrease);
            while (t < TimeToIncrease)
            {
                Camera.fieldOfView = originalFov + (IncreaseCurve.Evaluate(t / TimeToIncrease) * FOVIncrease);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Fovs the kick down.
        /// </summary>
        /// <returns>IEnumerator.</returns>
        public IEnumerator FOVKickDown()
        {
            float t = Mathf.Abs((Camera.fieldOfView - originalFov) / FOVIncrease);
            while (t > 0)
            {
                Camera.fieldOfView = originalFov + (IncreaseCurve.Evaluate(t / TimeToDecrease) * FOVIncrease);
                t -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            //make sure that fov returns to the original size
            Camera.fieldOfView = originalFov;
        }
    }
}