// /* ------------------
//       ${Name} 
//       (c)3Radical 2012
//           by Mike Talbot 
//     ------------------- */
// 
using UnityEngine;

[AddComponentMenu("Components/Storage/Preserve Object When Loading")]
public class PreserveObjectWhenLoading : MonoBehaviour {
    private void Awake() {
        LevelLoader.OnDestroyObject += HandleLevelLoaderOnDestroyObject;
    }

    private void HandleLevelLoaderOnDestroyObject(GameObject toBeDestroyed, ref bool cancel) {

        cancel = true;
    }
    private void OnDestroy() {
        LevelLoader.OnDestroyObject -= HandleLevelLoaderOnDestroyObject;
    }
}