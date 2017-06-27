using Lerp2Assets.CrossPlatformInput.CrossPlatformInput;
using Lerp2Assets.CrossPlatformInput.PlatformSpecific;
using System;
using UnityEngine;

namespace Lerp2Assets.CrossPlatformInput
{
    /// <summary>
    /// Class CrossPlatformInputManager.
    /// </summary>
    public static partial class CrossPlatformInputManager
    {
        private static VirtualInput activeInput;

        private static VirtualInput s_TouchInput;
        private static VirtualInput s_HardwareInput;

        static CrossPlatformInputManager()
        {
            s_TouchInput = new MobileInput();
            s_HardwareInput = new StandaloneInput();
#if MOBILE_INPUT
            activeInput = s_TouchInput;
#else
            activeInput = s_HardwareInput;
#endif
        }

        /// <summary>
        /// Switches the active input method.
        /// </summary>
        /// <param name="activeInputMethod">The active input method.</param>
        public static void SwitchActiveInputMethod(ActiveInputMethod activeInputMethod)
        {
            switch (activeInputMethod)
            {
                case ActiveInputMethod.Hardware:
                    activeInput = s_HardwareInput;
                    break;

                case ActiveInputMethod.Touch:
                    activeInput = s_TouchInput;
                    break;
            }
        }

        /// <summary>
        /// Axises the exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool AxisExists(string name)
        {
            return activeInput.AxisExists(name);
        }

        /// <summary>
        /// Buttons the exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ButtonExists(string name)
        {
            return activeInput.ButtonExists(name);
        }

        /// <summary>
        /// Registers the virtual axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        public static void RegisterVirtualAxis(VirtualAxis axis)
        {
            activeInput.RegisterVirtualAxis(axis);
        }

        /// <summary>
        /// Registers the virtual button.
        /// </summary>
        /// <param name="button">The button.</param>
        public static void RegisterVirtualButton(VirtualButton button)
        {
            activeInput.RegisterVirtualButton(button);
        }

        /// <summary>
        /// Uns the register virtual axis.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">name</exception>
        public static void UnRegisterVirtualAxis(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            activeInput.UnRegisterVirtualAxis(name);
        }

        /// <summary>
        /// Uns the register virtual button.
        /// </summary>
        /// <param name="name">The name.</param>
        public static void UnRegisterVirtualButton(string name)
        {
            activeInput.UnRegisterVirtualButton(name);
        }

        // returns a reference to a named virtual axis if it exists otherwise null
        /// <summary>
        /// Virtuals the axis reference.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>VirtualAxis.</returns>
        public static VirtualAxis VirtualAxisReference(string name)
        {
            return activeInput.VirtualAxisReference(name);
        }

        // returns the platform appropriate axis for the given name
        /// <summary>
        /// Gets the axis.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Single.</returns>
        public static float GetAxis(string name)
        {
            return GetAxis(name, false);
        }

        /// <summary>
        /// Gets the axis raw.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.Single.</returns>
        public static float GetAxisRaw(string name)
        {
            return GetAxis(name, true);
        }

        // private function handles both types of axis (raw and not raw)
        private static float GetAxis(string name, bool raw)
        {
            return activeInput.GetAxis(name, raw);
        }

        // -- Button handling --
        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetButton(string name)
        {
            return activeInput.GetButton(name);
        }

        /// <summary>
        /// Gets the button down.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetButtonDown(string name)
        {
            return activeInput.GetButtonDown(name);
        }

        /// <summary>
        /// Gets the button up.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetButtonUp(string name)
        {
            return activeInput.GetButtonUp(name);
        }

        /// <summary>
        /// Sets the button down.
        /// </summary>
        /// <param name="name">The name.</param>
        public static void SetButtonDown(string name)
        {
            activeInput.SetButtonDown(name);
        }

        /// <summary>
        /// Sets the button up.
        /// </summary>
        /// <param name="name">The name.</param>
        public static void SetButtonUp(string name)
        {
            activeInput.SetButtonUp(name);
        }

        /// <summary>
        /// Sets the axis positive.
        /// </summary>
        /// <param name="name">The name.</param>
        public static void SetAxisPositive(string name)
        {
            activeInput.SetAxisPositive(name);
        }

        /// <summary>
        /// Sets the axis negative.
        /// </summary>
        /// <param name="name">The name.</param>
        public static void SetAxisNegative(string name)
        {
            activeInput.SetAxisNegative(name);
        }

        /// <summary>
        /// Sets the axis zero.
        /// </summary>
        /// <param name="name">The name.</param>
        public static void SetAxisZero(string name)
        {
            activeInput.SetAxisZero(name);
        }

        /// <summary>
        /// Sets the axis.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void SetAxis(string name, float value)
        {
            activeInput.SetAxis(name, value);
        }

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <value>The mouse position.</value>
        public static Vector3 mousePosition
        {
            get { return activeInput.MousePosition(); }
        }

        /// <summary>
        /// Sets the virtual mouse position x.
        /// </summary>
        /// <param name="f">The f.</param>
        public static void SetVirtualMousePositionX(float f)
        {
            activeInput.SetVirtualMousePositionX(f);
        }

        /// <summary>
        /// Sets the virtual mouse position y.
        /// </summary>
        /// <param name="f">The f.</param>
        public static void SetVirtualMousePositionY(float f)
        {
            activeInput.SetVirtualMousePositionY(f);
        }

        /// <summary>
        /// Sets the virtual mouse position z.
        /// </summary>
        /// <param name="f">The f.</param>
        public static void SetVirtualMousePositionZ(float f)
        {
            activeInput.SetVirtualMousePositionZ(f);
        }

        // virtual axis and button classes - applies to mobile input
        // Can be mapped to touch joysticks, tilt, gyro, etc, depending on desired implementation.
        // Could also be implemented by other input devices - kinect, electronic sensors, etc
        /// <summary>
        /// Class VirtualAxis.
        /// </summary>
        public class VirtualAxis
        {
            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>The name.</value>
            public string name { get; private set; }

            private float m_Value;

            /// <summary>
            /// Gets a value indicating whether [match with input manager].
            /// </summary>
            /// <value><c>true</c> if [match with input manager]; otherwise, <c>false</c>.</value>
            public bool matchWithInputManager { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="VirtualAxis"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            public VirtualAxis(string name)
                : this(name, true)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="VirtualAxis"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="matchToInputSettings">if set to <c>true</c> [match to input settings].</param>
            public VirtualAxis(string name, bool matchToInputSettings)
            {
                this.name = name;
                matchWithInputManager = matchToInputSettings;
            }

            // removes an axes from the cross platform input system
            /// <summary>
            /// Removes this instance.
            /// </summary>
            public void Remove()
            {
                UnRegisterVirtualAxis(name);
            }

            // a controller gameobject (eg. a virtual thumbstick) should update this class
            /// <summary>
            /// Updates the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            public void Update(float value)
            {
                m_Value = value;
            }

            /// <summary>
            /// Gets the get value.
            /// </summary>
            /// <value>The get value.</value>
            public float GetValue
            {
                get { return m_Value; }
            }

            /// <summary>
            /// Gets the get value raw.
            /// </summary>
            /// <value>The get value raw.</value>
            public float GetValueRaw
            {
                get { return m_Value; }
            }
        }

        // a controller gameobject (eg. a virtual GUI button) should call the
        // 'pressed' function of this class. Other objects can then read the
        // Get/Down/Up state of this button.
        /// <summary>
        /// Class VirtualButton.
        /// </summary>
        public class VirtualButton
        {
            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>The name.</value>
            public string name { get; private set; }

            /// <summary>
            /// Gets a value indicating whether [match with input manager].
            /// </summary>
            /// <value><c>true</c> if [match with input manager]; otherwise, <c>false</c>.</value>
            public bool matchWithInputManager { get; private set; }

            private int m_LastPressedFrame = -5;
            private int m_ReleasedFrame = -5;
            private bool m_Pressed;

            /// <summary>
            /// Initializes a new instance of the <see cref="VirtualButton"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            public VirtualButton(string name)
                : this(name, true)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="VirtualButton"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="matchToInputSettings">if set to <c>true</c> [match to input settings].</param>
            public VirtualButton(string name, bool matchToInputSettings)
            {
                this.name = name;
                matchWithInputManager = matchToInputSettings;
            }

            // A controller gameobject should call this function when the button is pressed down
            /// <summary>
            /// Presseds this instance.
            /// </summary>
            public void Pressed()
            {
                if (m_Pressed)
                {
                    return;
                }
                m_Pressed = true;
                m_LastPressedFrame = Time.frameCount;
            }

            // A controller gameobject should call this function when the button is released
            /// <summary>
            /// Releaseds this instance.
            /// </summary>
            public void Released()
            {
                m_Pressed = false;
                m_ReleasedFrame = Time.frameCount;
            }

            // the controller gameobject should call Remove when the button is destroyed or disabled
            /// <summary>
            /// Removes this instance.
            /// </summary>
            public void Remove()
            {
                UnRegisterVirtualButton(name);
            }

            // these are the states of the button which can be read via the cross platform input system
            /// <summary>
            /// Gets a value indicating whether [get button].
            /// </summary>
            /// <value><c>true</c> if [get button]; otherwise, <c>false</c>.</value>
            public bool GetButton
            {
                get { return m_Pressed; }
            }

            /// <summary>
            /// Gets a value indicating whether [get button down].
            /// </summary>
            /// <value><c>true</c> if [get button down]; otherwise, <c>false</c>.</value>
            public bool GetButtonDown
            {
                get
                {
                    return m_LastPressedFrame - Time.frameCount == -1;
                }
            }

            /// <summary>
            /// Gets a value indicating whether [get button up].
            /// </summary>
            /// <value><c>true</c> if [get button up]; otherwise, <c>false</c>.</value>
            public bool GetButtonUp
            {
                get
                {
                    return (m_ReleasedFrame == Time.frameCount - 1);
                }
            }
        }
    }
}