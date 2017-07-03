using UnityEngine;
using System.Collections;

namespace TeamUtility.IO
{
    /// <summary>
    /// Struct ScanSettings
    /// </summary>
    public struct ScanSettings
    {
        /// <summary>
        /// The scan flags
        /// </summary>
        public ScanFlags scanFlags;

        /// <summary>
        /// The joystick
        /// </summary>
        public int? joystick;

        /// <summary>
        /// The timeout
        /// </summary>
        public float timeout;

        /// <summary>
        /// The cancel scan button
        /// </summary>
        public string cancelScanButton;

        /// <summary>
        /// The user data
        /// </summary>
        public object userData;
    }
}