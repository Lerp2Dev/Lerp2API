using System.Collections.Generic;
using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2Assets.CrossPlatformInput.CrossPlatformInput
{
    /// <summary>
    /// Class VirtualInput.
    /// </summary>
    public abstract class VirtualInput
    {
        /// <summary>
        /// Gets the virtual mouse position.
        /// </summary>
        /// <value>The virtual mouse position.</value>
        public Vector3 virtualMousePosition { get; private set; }

        /// <summary>
        /// The m virtual axes
        /// </summary>
        protected Dictionary<string, CrossPlatformInputManager.VirtualAxis> m_VirtualAxes =
            new Dictionary<string, CrossPlatformInputManager.VirtualAxis>();

        // Dictionary to store the name relating to the virtual axes
        /// <summary>
        /// The m virtual buttons
        /// </summary>
        protected Dictionary<string, CrossPlatformInputManager.VirtualButton> m_VirtualButtons =
            new Dictionary<string, CrossPlatformInputManager.VirtualButton>();

        /// <summary>
        /// The m always use virtual
        /// </summary>
        protected List<string> m_AlwaysUseVirtual = new List<string>();

        // list of the axis and button names that have been flagged to always use a virtual axis or button

        /// <summary>
        /// Axises the exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool AxisExists(string name)
        {
            return m_VirtualAxes.ContainsKey(name);
        }

        /// <summary>
        /// Buttons the exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ButtonExists(string name)
        {
            return m_VirtualButtons.ContainsKey(name);
        }

        /// <summary>
        /// Registers the virtual axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public void RegisterVirtualAxis(CrossPlatformInputManager.VirtualAxis axis)
        {
            // check if we already have an axis with that name and log and error if we do
            if (!m_VirtualAxes.ContainsKey(axis.name))
            {
                // add any new axes
                m_VirtualAxes.Add(axis.name, axis);

                // if we dont want to match with the input manager setting then revert to always using virtual
                if (!axis.matchWithInputManager)
                    m_AlwaysUseVirtual.Add(axis.name);
            }
            else
                Debug.LogError("There is already a virtual axis named " + axis.name + " registered.");
        }

        /// <summary>
        /// Registers the virtual button.
        /// </summary>
        /// <param name="button">The button.</param>
        public void RegisterVirtualButton(CrossPlatformInputManager.VirtualButton button)
        {
            // check if already have a buttin with that name and log an error if we do
            if (!m_VirtualButtons.ContainsKey(button.name))
            {
                // add any new buttons
                m_VirtualButtons.Add(button.name, button);

                // if we dont want to match to the input manager then always use a virtual axis
                if (!button.matchWithInputManager)
                {
                    m_AlwaysUseVirtual.Add(button.name);
                }
            }
            else
                Debug.LogError("There is already a virtual button named " + button.name + " registered.");
        }

        /// <summary>
        /// Uns the register virtual axis.
        /// </summary>
        /// <param name="name">The name.</param>
        public void UnRegisterVirtualAxis(string name)
        {
            // if we have an axis with that name then remove it from our dictionary of registered axes
            if (m_VirtualAxes.ContainsKey(name))
            {
                m_VirtualAxes.Remove(name);
            }
        }

        /// <summary>
        /// Uns the register virtual button.
        /// </summary>
        /// <param name="name">The name.</param>
        public void UnRegisterVirtualButton(string name)
        {
            // if we have a button with this name then remove it from our dictionary of registered buttons
            if (m_VirtualButtons.ContainsKey(name))
            {
                m_VirtualButtons.Remove(name);
            }
        }

        // returns a reference to a named virtual axis if it exists otherwise null
        /// <summary>
        /// Virtuals the axis reference.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>CrossPlatformInputManager.VirtualAxis.</returns>
        public CrossPlatformInputManager.VirtualAxis VirtualAxisReference(string name)
        {
            return m_VirtualAxes.ContainsKey(name) ? m_VirtualAxes[name] : null;
        }

        /// <summary>
        /// Sets the virtual mouse position x.
        /// </summary>
        /// <param name="f">The f.</param>
        public void SetVirtualMousePositionX(float f)
        {
            virtualMousePosition = new Vector3(f, virtualMousePosition.y, virtualMousePosition.z);
        }

        /// <summary>
        /// Sets the virtual mouse position y.
        /// </summary>
        /// <param name="f">The f.</param>
        public void SetVirtualMousePositionY(float f)
        {
            virtualMousePosition = new Vector3(virtualMousePosition.x, f, virtualMousePosition.z);
        }

        /// <summary>
        /// Sets the virtual mouse position z.
        /// </summary>
        /// <param name="f">The f.</param>
        public void SetVirtualMousePositionZ(float f)
        {
            virtualMousePosition = new Vector3(virtualMousePosition.x, virtualMousePosition.y, f);
        }

        /// <summary>
        /// Gets the axis.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="raw">if set to <c>true</c> [raw].</param>
        /// <returns>System.Single.</returns>
        public abstract float GetAxis(string name, bool raw);

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public abstract bool GetButton(string name);

        /// <summary>
        /// Gets the button down.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public abstract bool GetButtonDown(string name);

        /// <summary>
        /// Gets the button up.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public abstract bool GetButtonUp(string name);

        /// <summary>
        /// Sets the button down.
        /// </summary>
        /// <param name="name">The name.</param>
        public abstract void SetButtonDown(string name);

        /// <summary>
        /// Sets the button up.
        /// </summary>
        /// <param name="name">The name.</param>
        public abstract void SetButtonUp(string name);

        /// <summary>
        /// Sets the axis positive.
        /// </summary>
        /// <param name="name">The name.</param>
        public abstract void SetAxisPositive(string name);

        /// <summary>
        /// Sets the axis negative.
        /// </summary>
        /// <param name="name">The name.</param>
        public abstract void SetAxisNegative(string name);

        /// <summary>
        /// Sets the axis zero.
        /// </summary>
        /// <param name="name">The name.</param>
        public abstract void SetAxisZero(string name);

        /// <summary>
        /// Sets the axis.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public abstract void SetAxis(string name, float value);

        /// <summary>
        /// Mouses the position.
        /// </summary>
        /// <returns>Vector3.</returns>
        public abstract Vector3 MousePosition();
    }
}