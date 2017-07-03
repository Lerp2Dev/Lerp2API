using UnityEngine;
using System.Collections;

namespace TeamUtility.IO
{
    /// <summary>
    /// Struct ScanResult
    /// </summary>
    public struct ScanResult
    {
        /// <summary>
        /// The scan flags
        /// </summary>
        public ScanFlags scanFlags;

        /// <summary>
        /// The key
        /// </summary>
        public KeyCode key;

        /// <summary>
        /// The joystick
        /// </summary>
        public int joystick;

        /// <summary>
        /// The joystick axis
        /// </summary>
        public int joystickAxis;

        /// <summary>
        /// The joystick axis value
        /// </summary>
        public float joystickAxisValue;

        /// <summary>
        /// The mouse axis
        /// </summary>
        public int mouseAxis;

        /// <summary>
        /// The user data
        /// </summary>
        public object userData;
    }
}