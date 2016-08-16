// /* ------------------
//       ${Name} 
//       (c)3Radical 2012
//           by Mike Talbot 
//     ------------------- */
// 
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Linq;

[AddComponentMenu("Storage/Internal/Room Loader (Internal use only, do not add this to your scene)")]
public class RoomLoader : MonoBehaviour {
    private void Awake() {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded -= SceneWasLoaded;
        SceneManager.sceneLoaded += SceneWasLoaded;
    }

    private void SceneWasLoaded(Scene scene, LoadSceneMode mode) {
        foreach (var go in FindObjectsOfType(typeof(GameObject)).Cast<GameObject>()) {
            go.SendMessage("OnRoomWasLoaded", SendMessageOptions.DontRequireReceiver);
        }
        Destroy(gameObject);
        SceneManager.sceneLoaded -= SceneWasLoaded;
    }
}