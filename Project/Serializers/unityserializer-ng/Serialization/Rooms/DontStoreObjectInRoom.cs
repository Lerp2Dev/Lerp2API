// /* ------------------
//       ${Name}
//       (c)3Radical 2012
//           by Mike Talbot
//     ------------------- */
//
using UnityEngine;

/// <summary>
/// Class DontStoreObjectInRoom.
/// </summary>
[AddComponentMenu("Storage/Rooms/Dont Store Object In Room")]
public class DontStoreObjectInRoom : MonoBehaviour, IControlSerializationEx
{
    /// <summary>
    /// The preserve this object when loading
    /// </summary>
    public bool preserveThisObjectWhenLoading = true;

    private void Awake()
    {
        LevelLoader.OnDestroyObject += HandleLevelLoaderOnDestroyObject;
    }

    private void HandleLevelLoaderOnDestroyObject(GameObject toBeDestroyed, ref bool cancel)
    {
        if (toBeDestroyed == gameObject)
        {
            cancel = preserveThisObjectWhenLoading;
        }
    }

    private void OnDestroy()
    {
        LevelLoader.OnDestroyObject -= HandleLevelLoaderOnDestroyObject;
    }

    /// <summary>
    /// Shoulds the save whole object.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ShouldSaveWholeObject()
    {
        return !RoomManager.savingRoom;
    }

    /// <summary>
    /// Shoulds the save.
    /// </summary>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public bool ShouldSave()
    {
        return !RoomManager.savingRoom;
    }
}