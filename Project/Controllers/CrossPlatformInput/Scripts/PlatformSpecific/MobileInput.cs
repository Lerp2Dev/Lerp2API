using Lerp2Assets.CrossPlatformInput.CrossPlatformInput;
using UnityEngine;

namespace Lerp2Assets.CrossPlatformInput.PlatformSpecific
{
    /// <summary>
    /// Class MobileInput.
    /// </summary>
    /// <seealso cref="Lerp2Assets.CrossPlatformInput.CrossPlatformInput.VirtualInput" />
    public class MobileInput : VirtualInput
    {
        private void AddButton(string name)
        {
            // we have not registered this button yet so add it, happens in the constructor
            CrossPlatformInputManager.RegisterVirtualButton(new CrossPlatformInputManager.VirtualButton(name));
        }

        private void AddAxes(string name)
        {
            // we have not registered this button yet so add it, happens in the constructor
            CrossPlatformInputManager.RegisterVirtualAxis(new CrossPlatformInputManager.VirtualAxis(name));
        }

        /// <summary>
        /// Gets the axis.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="raw">if set to <c>true</c> [raw].</param>
        /// <returns>System.Single.</returns>
        public override float GetAxis(string name, bool raw)
        {
            if (!m_VirtualAxes.ContainsKey(name))
            {
                AddAxes(name);
            }
            return m_VirtualAxes[name].GetValue;
        }

        /// <summary>
        /// Sets the button down.
        /// </summary>
        /// <param name="name">The name.</param>
        public override void SetButtonDown(string name)
        {
            if (!m_VirtualButtons.ContainsKey(name))
            {
                AddButton(name);
            }
            m_VirtualButtons[name].Pressed();
        }

        /// <summary>
        /// Sets the button up.
        /// </summary>
        /// <param name="name">The name.</param>
        public override void SetButtonUp(string name)
        {
            if (!m_VirtualButtons.ContainsKey(name))
            {
                AddButton(name);
            }
            m_VirtualButtons[name].Released();
        }

        /// <summary>
        /// Sets the axis positive.
        /// </summary>
        /// <param name="name">The name.</param>
        public override void SetAxisPositive(string name)
        {
            if (!m_VirtualAxes.ContainsKey(name))
            {
                AddAxes(name);
            }
            m_VirtualAxes[name].Update(1f);
        }

        /// <summary>
        /// Sets the axis negative.
        /// </summary>
        /// <param name="name">The name.</param>
        public override void SetAxisNegative(string name)
        {
            if (!m_VirtualAxes.ContainsKey(name))
            {
                AddAxes(name);
            }
            m_VirtualAxes[name].Update(-1f);
        }

        /// <summary>
        /// Sets the axis zero.
        /// </summary>
        /// <param name="name">The name.</param>
        public override void SetAxisZero(string name)
        {
            if (!m_VirtualAxes.ContainsKey(name))
            {
                AddAxes(name);
            }
            m_VirtualAxes[name].Update(0f);
        }

        /// <summary>
        /// Sets the axis.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public override void SetAxis(string name, float value)
        {
            if (!m_VirtualAxes.ContainsKey(name))
            {
                AddAxes(name);
            }
            m_VirtualAxes[name].Update(value);
        }

        /// <summary>
        /// Gets the button down.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool GetButtonDown(string name)
        {
            if (m_VirtualButtons.ContainsKey(name))
            {
                return m_VirtualButtons[name].GetButtonDown;
            }

            AddButton(name);
            return m_VirtualButtons[name].GetButtonDown;
        }

        /// <summary>
        /// Gets the button up.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool GetButtonUp(string name)
        {
            if (m_VirtualButtons.ContainsKey(name))
            {
                return m_VirtualButtons[name].GetButtonUp;
            }

            AddButton(name);
            return m_VirtualButtons[name].GetButtonUp;
        }

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool GetButton(string name)
        {
            if (m_VirtualButtons.ContainsKey(name))
            {
                return m_VirtualButtons[name].GetButton;
            }

            AddButton(name);
            return m_VirtualButtons[name].GetButton;
        }

        /// <summary>
        /// Mouses the position.
        /// </summary>
        /// <returns>Vector3.</returns>
        public override Vector3 MousePosition()
        {
            return virtualMousePosition;
        }
    }
}