// /* ------------------
//       ${Name}
//       (c)3Radical 2012
//           by Mike Talbot
//     ------------------- */
//
using UnityEngine;

/// <summary>
/// Class Room.
/// </summary>
[AddComponentMenu("Storage/Rooms/Room")]
[DontStore]
public class Room : MonoBehaviour
{
    /// <summary>
    /// The current
    /// </summary>
    public static Room Current;

    private void Awake()
    {
        Current = this;
    }

    /// <summary>
    /// Saves this instance.
    /// </summary>
    public void Save()
    {
        RoomManager.SaveCurrentRoom();
    }
}