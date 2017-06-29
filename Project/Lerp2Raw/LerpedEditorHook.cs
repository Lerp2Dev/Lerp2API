using Lerp2API;
using Lerp2API.Mono;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

namespace Lerp2Raw
{
#if UNITY_EDITOR

    /// <summary>
    /// Class LerpedEditorHook.
    /// </summary>
    /// <seealso cref="Lerp2API.Mono.LerpedMono" />
    [ExecuteInEditMode]
#endif
    public class LerpedEditorHook : LerpedMono
    {
#if UNITY_EDITOR

        /// <summary>
        /// Me
        /// </summary>
        public new static LerpedEditorHook me;

        /// <summary>
        /// The type pool
        /// </summary>
        public static Queue<Type> typePool = new Queue<Type>();

        [InitializeOnLoadMethod]
        private static void OnLoadMethods()
        {
            ChangeAPILevelComp();
            LerpedCore.HookThis(typeof(LerpedEditorHook));
            LerpedCore.HookThis(typeof(LerpedHook));
        }

        private static void ChangeAPILevelComp()
        {
            if (PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Standalone | BuildTargetGroup.WebGL) == ApiCompatibilityLevel.NET_2_0_Subset)
                PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone | BuildTargetGroup.WebGL, ApiCompatibilityLevel.NET_2_0);
        }

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        public void Awake()
        {
            me = this;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (typePool.Count > 0)
                {
                    Type type = typePool.Dequeue();
                    gameObject.AddComponent(type);
                }
            }
        }

        /// <summary>
        /// Adds the element.
        /// </summary>
        /// <param name="name">The name.</param>
        public void AddElement(string name)
        {
            Type type = Type.GetType(name);
            if (type == null)
                Debug.LogErrorFormat("The type '{0}' wan't found.", name);
            else
            {
                if (!type.IsSubclassOf(typeof(MonoBehaviour)))
                    Debug.LogErrorFormat("The type '{0}' isn't derived from MonoBehaviour.");
                else
                {
                    typePool.Enqueue(type);
                }
            }
        }

        private void OnDestroy()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                LerpedCore.SetBool(LerpedCore.UnityBoot, false);
        }

#endif
    }
}