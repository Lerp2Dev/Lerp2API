using Lerp2Assets.CrossPlatformInput.CrossPlatformInput;
using System;
using UnityEngine;

namespace Lerp2Assets.CrossPlatformInput.PlatformSpecific
{
    /// <summary>
    /// Class StandaloneInput.
    /// </summary>
    /// <seealso cref="Lerp2Assets.CrossPlatformInput.CrossPlatformInput.VirtualInput" />
    public class StandaloneInput : VirtualInput
    {
        /// <summary>
        /// Gets the axis.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="raw">if set to <c>true</c> [raw].</param>
        /// <returns>System.Single.</returns>
        public override float GetAxis(string name, bool raw)
        {
            return raw ? Input.GetAxisRaw(name) : Input.GetAxis(name);
        }

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool GetButton(string name)
        {
            return Input.GetButton(name);
        }

        /// <summary>
        /// Gets the button down.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool GetButtonDown(string name)
        {
            return Input.GetButtonDown(name);
        }

        /// <summary>
        /// Gets the button up.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool GetButtonUp(string name)
        {
            return Input.GetButtonUp(name);
        }

        /// <summary>
        /// Sets the button down.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="Exception">This is not possible to be called for standalone input. Please check your platform and code where this is called</exception>
        public override void SetButtonDown(string name)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }

        /// <summary>
        /// Sets the button up.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="Exception">This is not possible to be called for standalone input. Please check your platform and code where this is called</exception>
        public override void SetButtonUp(string name)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }

        /// <summary>
        /// Sets the axis positive.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="Exception">This is not possible to be called for standalone input. Please check your platform and code where this is called</exception>
        public override void SetAxisPositive(string name)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }

        /// <summary>
        /// Sets the axis negative.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="Exception">This is not possible to be called for standalone input. Please check your platform and code where this is called</exception>
        public override void SetAxisNegative(string name)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }

        /// <summary>
        /// Sets the axis zero.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="Exception">This is not possible to be called for standalone input. Please check your platform and code where this is called</exception>
        public override void SetAxisZero(string name)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }

        /// <summary>
        /// Sets the axis.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="Exception">This is not possible to be called for standalone input. Please check your platform and code where this is called</exception>
        public override void SetAxis(string name, float value)
        {
            throw new Exception(
                " This is not possible to be called for standalone input. Please check your platform and code where this is called");
        }

        /// <summary>
        /// Mouses the position.
        /// </summary>
        /// <returns>Vector3.</returns>
        public override Vector3 MousePosition()
        {
            return Input.mousePosition;
        }
    }
}