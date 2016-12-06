using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lerp2API.Utility
{
    internal static class GameLoopEntry
    {

        #region Events
        public static event EventHandler Update
        {
            add
            {
                _updateHook.UpdateHook += value;
            }
            remove
            {
                _updateHook.UpdateHook -= value;
            }
        }
        #endregion

        #region Fields
        private static GameObject _gameObject;
        private static UpdateEventHooks _updateHook;
        #endregion

        #region CONSTRUCTOR
        static GameLoopEntry()
        {
            _gameObject = GameObject.Find("Lerp2Dev.GameLoopEntryObject") ?? new GameObject("Lerp2Dev.GameLoopEntryObject");
            //_gameObject.hideFlags = HideFlags.HideAndDontSave;
            _updateHook = _gameObject.GetComponent<UpdateEventHooks>() ?? _gameObject.AddComponent<UpdateEventHooks>();
            Object.DontDestroyOnLoad(_gameObject);
        }
        #endregion

        #region Methods

        //Regular

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            if(Application.isPlaying)
                return _updateHook.StartCoroutine(routine);
            else
            {
                Debug.LogError("Coroutines cannot be executed is anyone is playing!");
                return null;
            }
        }

        #endregion

    }
}
