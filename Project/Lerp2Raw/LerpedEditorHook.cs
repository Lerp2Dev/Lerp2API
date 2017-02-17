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

    [ExecuteInEditMode]
    public class LerpedEditorHook : LerpedMono
    {
#if UNITY_EDITOR
        public static Queue<Type> typePool = new Queue<Type>();

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
