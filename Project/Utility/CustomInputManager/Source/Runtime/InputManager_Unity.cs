#region [Copyright (c) 2015 Cristian Alexandru Geambasu]

//	Distributed under the terms of an MIT-style license:
//
//	The MIT License
//
//	Copyright (c) 2015 Cristian Alexandru Geambasu
//
//	Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//	and associated documentation files (the "Software"), to deal in the Software without restriction,
//	including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
//	and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
//	subject to the following conditions:
//
//	The above copyright notice and this permission notice shall be included in all copies or substantial
//	portions of the Software.
//
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion [Copyright (c) 2015 Cristian Alexandru Geambasu]

using UnityEngine;
using System;
using System.Collections;

namespace TeamUtility.IO
{
    /// <summary>
    /// Class InputManager.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public partial class InputManager : MonoBehaviour
    {
        /// <summary>
        /// Gets the acceleration.
        /// </summary>
        /// <value>The acceleration.</value>
        public static Vector3 acceleration { get { return Input.acceleration; } }

        /// <summary>
        /// Gets the acceleration event count.
        /// </summary>
        /// <value>The acceleration event count.</value>
        public static int accelerationEventCount { get { return Input.accelerationEventCount; } }

        /// <summary>
        /// Gets the acceleration events.
        /// </summary>
        /// <value>The acceleration events.</value>
        public static AccelerationEvent[] accelerationEvents { get { return Input.accelerationEvents; } }

        /// <summary>
        /// Gets a value indicating whether [any key].
        /// </summary>
        /// <value><c>true</c> if [any key]; otherwise, <c>false</c>.</value>
        public static bool anyKey { get { return Input.anyKey; } }

        /// <summary>
        /// Gets a value indicating whether [any key down].
        /// </summary>
        /// <value><c>true</c> if [any key down]; otherwise, <c>false</c>.</value>
        public static bool anyKeyDown { get { return Input.anyKeyDown; } }

        /// <summary>
        /// Gets the compass.
        /// </summary>
        /// <value>The compass.</value>
        public static Compass compass { get { return Input.compass; } }

        /// <summary>
        /// Gets the composition string.
        /// </summary>
        /// <value>The composition string.</value>
        public static string compositionString { get { return Input.compositionString; } }

        /// <summary>
        /// Gets the device orientation.
        /// </summary>
        /// <value>The device orientation.</value>
        public static DeviceOrientation deviceOrientation { get { return Input.deviceOrientation; } }

        /// <summary>
        /// Gets the gyro.
        /// </summary>
        /// <value>The gyro.</value>
        public static Gyroscope gyro { get { return Input.gyro; } }

        /// <summary>
        /// Gets a value indicating whether [IME is selected].
        /// </summary>
        /// <value><c>true</c> if [IME is selected]; otherwise, <c>false</c>.</value>
        public static bool imeIsSelected { get { return Input.imeIsSelected; } }

        /// <summary>
        /// Gets the input string.
        /// </summary>
        /// <value>The input string.</value>
        public static string inputString { get { return Input.inputString; } }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public static LocationService location { get { return Input.location; } }

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <value>The mouse position.</value>
        public static Vector2 mousePosition { get { return Input.mousePosition; } }

        /// <summary>
        /// Gets a value indicating whether [mouse present].
        /// </summary>
        /// <value><c>true</c> if [mouse present]; otherwise, <c>false</c>.</value>
        public static bool mousePresent { get { return Input.mousePresent; } }

        /// <summary>
        /// Gets a value indicating whether [touch supported].
        /// </summary>
        /// <value><c>true</c> if [touch supported]; otherwise, <c>false</c>.</value>
        public static bool touchSupported { get { return Input.touchSupported; } }

        /// <summary>
        /// Gets the touch count.
        /// </summary>
        /// <value>The touch count.</value>
        public static int touchCount { get { return Input.touchCount; } }

        /// <summary>
        /// Gets the touches.
        /// </summary>
        /// <value>The touches.</value>
        public static Touch[] touches { get { return Input.touches; } }

        /// <summary>
        /// Gets or sets a value indicating whether [compensate sensors].
        /// </summary>
        /// <value><c>true</c> if [compensate sensors]; otherwise, <c>false</c>.</value>
        public static bool compensateSensors
        {
            get { return Input.compensateSensors; }
            set { Input.compensateSensors = value; }
        }

        /// <summary>
        /// Gets or sets the composition cursor position.
        /// </summary>
        /// <value>The composition cursor position.</value>
        public static Vector2 compositionCursorPos
        {
            get { return Input.compositionCursorPos; }
            set { Input.compositionCursorPos = value; }
        }

        /// <summary>
        /// Gets or sets the IME composition mode.
        /// </summary>
        /// <value>The IME composition mode.</value>
        public static IMECompositionMode imeCompositionMode
        {
            get { return Input.imeCompositionMode; }
            set { Input.imeCompositionMode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [multi touch enabled].
        /// </summary>
        /// <value><c>true</c> if [multi touch enabled]; otherwise, <c>false</c>.</value>
        public static bool multiTouchEnabled
        {
            get { return Input.multiTouchEnabled; }
            set { Input.multiTouchEnabled = value; }
        }

        /// <summary>
        /// Gets the acceleration event.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>AccelerationEvent.</returns>
        public static AccelerationEvent GetAccelerationEvent(int index)
        {
            return Input.GetAccelerationEvent(index);
        }

        /// <summary>
        /// Gets the axis.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="playerID">The player identifier.</param>
        /// <returns>System.Single.</returns>
        public static float GetAxis(string name, PlayerID playerID = PlayerID.One)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(playerID, name);
            if (axisConfig != null)
            {
                return axisConfig.GetAxis();
            }
            else
            {
                Debug.LogError(string.Format("An axis named \'{0}\' does not exist in the active input configuration for player {1}", name, playerID));
                return 0.0f;
            }
        }

        /// <summary>
        /// Gets the axis raw.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="playerID">The player identifier.</param>
        /// <returns>System.Single.</returns>
        public static float GetAxisRaw(string name, PlayerID playerID = PlayerID.One)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(playerID, name);
            if (axisConfig != null)
            {
                return axisConfig.GetAxisRaw();
            }
            else
            {
                Debug.LogError(string.Format("An axis named \'{0}\' does not exist in the active input configuration for player {1}", name, playerID));
                return 0.0f;
            }
        }

        /// <summary>
        /// Gets the button.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="playerID">The player identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetButton(string name, PlayerID playerID = PlayerID.One)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(playerID, name);
            if (axisConfig != null)
            {
                return axisConfig.GetButton();
            }
            else
            {
                Debug.LogError(string.Format("An button named \'{0}\' does not exist in the active input configuration for player {1}", name, playerID));
                return false;
            }
        }

        /// <summary>
        /// Gets the button down.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="playerID">The player identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetButtonDown(string name, PlayerID playerID = PlayerID.One)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(playerID, name);
            if (axisConfig != null)
            {
                return axisConfig.GetButtonDown();
            }
            else
            {
                Debug.LogError(string.Format("An button named \'{0}\' does not exist in the active input configuration for player {1}", name, playerID));
                return false;
            }
        }

        /// <summary>
        /// Gets the button up.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="playerID">The player identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetButtonUp(string name, PlayerID playerID = PlayerID.One)
        {
            AxisConfiguration axisConfig = GetAxisConfiguration(playerID, name);
            if (axisConfig != null)
            {
                return axisConfig.GetButtonUp();
            }
            else
            {
                Debug.LogError(string.Format("An button named \'{0}\' does not exist in the active input configuration for player {1}", name, playerID));
                return false;
            }
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetKey(KeyCode key)
        {
            return Input.GetKey(key);
        }

        /// <summary>
        /// Gets the key down.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetKeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }

        /// <summary>
        /// Gets the key up.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetKeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }

        /// <summary>
        /// Gets the mouse button.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetMouseButton(int index)
        {
            return Input.GetMouseButton(index);
        }

        /// <summary>
        /// Gets the mouse button down.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetMouseButtonDown(int index)
        {
            return Input.GetMouseButtonDown(index);
        }

        /// <summary>
        /// Gets the mouse button up.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetMouseButtonUp(int index)
        {
            return Input.GetMouseButtonUp(index);
        }

        /// <summary>
        /// Gets the touch.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Touch.</returns>
        public static Touch GetTouch(int index)
        {
            return Input.GetTouch(index);
        }

        /// <summary>
        /// Gets the joystick names.
        /// </summary>
        /// <returns>System.String[].</returns>
        public static string[] GetJoystickNames()
        {
            return Input.GetJoystickNames();
        }

        /// <summary>
        /// Resets the input axes.
        /// </summary>
        public static void ResetInputAxes()
        {
            Input.ResetInputAxes();
        }
    }
}