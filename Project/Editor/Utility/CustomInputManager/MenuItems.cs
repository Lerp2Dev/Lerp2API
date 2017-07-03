using TeamUtility.IO;
using UnityEditor;
using UnityEngine;

namespace Lerp2APIEditor.Utility.CustomInputManager
{
    /// <summary>
    /// Class MenuItems.
    /// </summary>
    public class MenuItems
    {
        [MenuItem("Lerp2Dev Team Tools/Input Manager/Create Input Adapter", false, 3)]
        private static void Create()
        {
            GameObject gameObject = new GameObject("Input Adapter");
            gameObject.AddComponent<InputAdapter>();

            Selection.activeGameObject = gameObject;
        }

        [MenuItem("Lerp2Dev Team Tools/Input Manager/Use Custom Input Module", false, 201)]
        private static void FixEventSystem()
        {
            UnityEngine.EventSystems.StandaloneInputModule[] im = Object.FindObjectsOfType<UnityEngine.EventSystems.StandaloneInputModule>();
            if (im.Length > 0)
            {
                for (int i = 0; i < im.Length; i++)
                {
                    im[i].gameObject.AddComponent<StandaloneInputModule>();
                    Object.DestroyImmediate(im[i]);
                }
                EditorUtility.DisplayDialog("Success", "All built-in standalone input modules have been replaced!", "OK");
                Debug.LogFormat("{0} built-in standalone input module(s) have been replaced", im.Length);
            }
            else
            {
                EditorUtility.DisplayDialog("Warning", "Unable to find any built-in input modules in the scene!", "OK");
            }
        }
    }
}