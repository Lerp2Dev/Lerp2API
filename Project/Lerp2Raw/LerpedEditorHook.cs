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
    [ExecuteInEditMode]
#endif
    public class LerpedEditorHook : LerpedMono
    {
#if UNITY_EDITOR
        public static LerpedEditorHook me;
        public static Queue<Type> typePool = new Queue<Type>();

        [InitializeOnLoadMethod]
        static void OnLoadMethods()
        {
            ChangeAPILevelComp();
            LerpedCore.HookThis(typeof(LerpedEditorHook));
            LerpedCore.HookThis(typeof(LerpedHook));
        }

        private static void ChangeAPILevelComp()
        {
            if (PlayerSettings.apiCompatibilityLevel == ApiCompatibilityLevel.NET_2_0_Subset)
                PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
        }

        public void Awake()
        {
            me = this;
        }

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

        void OnDestroy()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                LerpedCore.SetBool(LerpedCore.UnityBoot, false);
        }
#endif
    }

}
