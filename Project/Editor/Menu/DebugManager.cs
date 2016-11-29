using UnityEditor;
using UnityEngine;
using Lerp2API;
using EditorMenu = UnityEditor.Menu;

namespace Lerp2APIEditor.Menu
{
    public class DebugManager
    {
        private const string menuName = "Lerp2Dev Team Tools/Enable or Disable Debug...";
        private const string section = "ENABLE_DEBUG";

        private static bool active
        {
            get
            {
                return LerpedCore.GetBool(section);
            }
        }

        [InitializeOnLoadMethod]
        static void Init()
        {
            EditorApplication.update += UpdateState;
        }

        private static void UpdateState()
        {
            EditorApplication.update -= UpdateState;
            EditorMenu.SetChecked(menuName, active);
        }

        [MenuItem(menuName)]
        public static void __changeDebug()
        {
            LerpedCore.SetBool(section, !active);
            EditorMenu.SetChecked(menuName, active);
            EditorPrefs.SetBool(menuName, active);
            if (active)
                Debug.Log("Debug is enabled!");
            else
                Debug.Log("Debug is disabled!");
        }
    }
}