using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Lerp2APIEditor.Utility
{
    /// <summary>
    /// Class LerpedShortcuts.
    /// </summary>
    public class LerpedShortcuts
    {
        private static bool keyDown;
        /// <summary>
        /// The key actions
        /// </summary>
        public static Dictionary<string, LerpedKeyAction> keyActions = new Dictionary<string, LerpedKeyAction>();

        [InitializeOnLoadMethod]
        static void EditorInit()
        {
            System.Reflection.FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            EditorApplication.CallbackFunction value = (EditorApplication.CallbackFunction)info.GetValue(null);
            value += CheckShortcuts;

            info.SetValue(null, value);
        }

        static void CheckShortcuts()
        {
            if (keyDown)
            {
                foreach (KeyValuePair<string, LerpedKeyAction> lka in keyActions)
                    if (lka.Value.keyCode == Event.current.keyCode)
                        lka.Value.action();
                keyDown = false;
            }
            else
                keyDown = true;
        }
    }

    /// <summary>
    /// Class LerpedKeyAction.
    /// </summary>
    public class LerpedKeyAction
    { //Next feature: Support for multiple key handling: http://answers.unity3d.com/questions/49285/how-can-i-get-a-combination-of-keys-pressed.html
        /// <summary>
        /// The key code
        /// </summary>
        public KeyCode keyCode;
        /// <summary>
        /// The action
        /// </summary>
        public Action action; //Maybe, it will needed to declare a parameter.

        /// <summary>
        /// Initializes a new instance of the <see cref="LerpedKeyAction"/> class.
        /// </summary>
        /// <param name="kc">The kc.</param>
        /// <param name="a">a.</param>
        public LerpedKeyAction(KeyCode kc, Action a)
        {
            keyCode = kc;
            action = a;
        }
    }
}
