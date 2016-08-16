// /* ------------------
//       ${Name} 
//       (c)3Radical 2012
//           by Mike Talbot 
//     ------------------- */
// 
using UnityEngine;

[AddComponentMenu("Storage/Rooms/Dont Store Object In Room")]
public class DontStoreObjectInRoom : MonoBehaviour, IControlSerializationEx {
    public bool preserveThisObjectWhenLoading = true;

    private void Awake() {
        LevelLoader.OnDestroyObject += HandleLevelLoaderOnDestroyObject;
    }

    private void HandleLevelLoaderOnDestroyObject(GameObject toBeDestroyed, ref bool cancel) {
        if (toBeDestroyed == gameObject) {
            cancel = preserveThisObjectWhenLoading;
        }
    }

    private void OnDestroy() {
        LevelLoader.OnDestroyObject -= HandleLevelLoaderOnDestroyObject;
    }

    public bool ShouldSaveWholeObject() {
        return !RoomManager.savingRoom;
    }

    public bool ShouldSave() {
        return !RoomManager.savingRoom;
    }

}


