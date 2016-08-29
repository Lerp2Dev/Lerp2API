#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2API.MenuSrc
{
    [InitializeOnLoad]
    public class UnityAnalyticsVersionChecker
    {
        private const string section = "ENABLE_ANALYTICS";

        private static bool analyticsEnabled
        {
            get
            {
                return LerpedCore.GetBool(section);
            }
        }

        static UnityAnalyticsVersionChecker()
        {
            if (!analyticsEnabled)
                if (!LerpedCore.CheckUnityVersion(5, 2))
                {
                    Debug.LogError("Unity Analytics SDK doesn't support versions less than 5.2");
                    LerpedCore.SetBool(section, true);
                }
                else // Defer calling Runonce until the first editor update is called, we do this so that Application.isPlaying gets the correct value
                    EditorApplication.update += RunOnce;
        }

        private static void RunOnce()
        {
            // Only show upgrade popup when project is opened, not when the app is playing.
            if (!Application.isPlaying)
            {
                LerpedCore.SetBool(section, true);
                EditorWindow.GetWindowWithRect<UnityAnalyticsSDKUpgradeWindow>(new Rect(300, 300, 380, 130), true, "Unity Analytics SDK");
            }
            EditorApplication.update -= RunOnce;
        }
    }

    public class UnityAnalyticsSDKUpgradeWindow : EditorWindow
    {
        private GUIContent upgrade = new GUIContent("How To Upgrade", "See docs on how to upgrade.");
        private GUIContent close = new GUIContent("Close", "Close this window.");
        private const string upgradeDocLink = "https://analytics.unity3d.com/upgrade51";

        [MenuItem("Lerp2Dev Team Tools/Watch Unity Analytics Info...", false, 100)]
        private static void SeeThis()
        {
            if (!Application.isPlaying)
                GetWindowWithRect<UnityAnalyticsSDKUpgradeWindow>(new Rect(300, 300, 380, 130), true, "Unity Analytics SDK");
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.Label("Unity Analytics is now integrated directly inside the Unity Engine.");
            GUILayout.Label("This requires a simple set of changes.");

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(upgrade, GUILayout.MaxWidth(120)))
            {
                Application.OpenURL(upgradeDocLink);
                Close();
            }
            if (GUILayout.Button(close, GUILayout.MaxWidth(120)))
                Close();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}

#endif