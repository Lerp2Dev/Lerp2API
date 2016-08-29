#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Lerp2API.MenuSrc
{
    [InitializeOnLoad]
    public class DebugManager
    {
        private const string menuName = "Lerp2Dev Team Tools/Enable or Disable Debug...";
        private const string section = "ENABLE_DEBUG";

        public static bool active
        {
            get
            {
                return LerpedCore.GetBool(section);
            }
        }

        static DebugManager()
        {
            Menu.SetChecked(menuName, active);
        }

        [MenuItem(menuName)]
        public static void __changeDebug()
        {
            LerpedCore.SetBool(section, !active);
            Menu.SetChecked(menuName, active);
            EditorPrefs.SetBool(menuName, active);
            if (active)
                Debug.Log("Debug is enabled!");
            else
                Debug.Log("Debug is disabled!");
        }
    }
}

#endif