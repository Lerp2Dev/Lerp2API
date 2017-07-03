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
using System.Collections;

namespace TeamUtility.IO
{
    /// <summary>
    /// Class AxisMapping.
    /// </summary>
    [System.Serializable]
    public class AxisMapping
    {
        private string _name;
        private KeyCode _key;
        private int _joystickAxis;
        private MappingWizard.ScanType _scanType;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get { return _name; } }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>The key.</value>
        public KeyCode Key { get { return _key; } }

        /// <summary>
        /// Gets the joystick axis.
        /// </summary>
        /// <value>The joystick axis.</value>
        public int JoystickAxis { get { return _joystickAxis; } }

        /// <summary>
        /// Gets the type of the scan.
        /// </summary>
        /// <value>The type of the scan.</value>
        public MappingWizard.ScanType ScanType { get { return _scanType; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisMapping"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        public AxisMapping(string name, KeyCode key)
        {
            _name = name;
            _key = key;
            _joystickAxis = -1;
            _scanType = MappingWizard.ScanType.Button;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AxisMapping"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="joystickAxis">The joystick axis.</param>
        public AxisMapping(string name, int joystickAxis)
        {
            _name = name;
            _joystickAxis = joystickAxis;
            _key = KeyCode.None;
            _scanType = MappingWizard.ScanType.Axis;
        }
    }
}