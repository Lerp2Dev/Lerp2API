// /* ------------------
//       ${Name} 
//       (c)3Radical 2012
//           by Mike Talbot 
//     ------------------- */
// 
using UnityEngine;

[AddComponentMenu("Storage/Rooms/Room")]
[DontStore]
public class Room : MonoBehaviour {
    public static Room Current;

    private void Awake() {
        Current = this;
    }

    public void Save() {
        RoomManager.SaveCurrentRoom();
    }
}