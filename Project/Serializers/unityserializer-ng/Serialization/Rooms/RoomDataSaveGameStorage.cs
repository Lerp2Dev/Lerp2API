// /* ------------------
//       ${Name}
//       (c)3Radical 2012
//           by Mike Talbot
//     ------------------- */
//
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class RoomDataSaveGameStorage.
/// </summary>
[AddComponentMenu("Storage/Rooms/Room Data Save Game Storage")]
public class RoomDataSaveGameStorage : DontStoreObjectInRoom
{
    /// <summary>
    /// Gets or sets the room data.
    /// </summary>
    /// <value>The room data.</value>
    public Dictionary<string, string> roomData
    {
        get
        {
            return RoomManager.rooms;
        }
        set
        {
            RoomManager.rooms = value;
        }
    }
}