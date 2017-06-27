// /* ------------------
//       ${Name}
//       (c)3Radical 2012
//           by Mike Talbot
//     ------------------- */
//
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class RoomManager.
/// </summary>
public class RoomManager
{
    /// <summary>
    /// The saving room
    /// </summary>
    public static bool savingRoom;
    /// <summary>
    /// The loading room
    /// </summary>
    public static bool loadingRoom;
    /// <summary>
    /// The rooms
    /// </summary>
    public static Dictionary<string, string> rooms = new Dictionary<string, string>();

    /// <summary>
    /// Saves the current room.
    /// </summary>
    public static void SaveCurrentRoom()
    {
        savingRoom = true;
        rooms[SceneManager.GetActiveScene().name] = LevelSerializer.SerializeLevel();
        savingRoom = false;
    }

    /// <summary>
    /// Loads the room.
    /// </summary>
    /// <param name="name">The name.</param>
    public static void LoadRoom(string name)
    {
        LoadRoom(name, true);
    }

    /// <summary>
    /// Loads the room.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="showGUI">if set to <c>true</c> [show GUI].</param>
    public static void LoadRoom(string name, bool showGUI)
    {
        if (Room.Current)
        {
            Room.Current.Save();
        }
        if (rooms.ContainsKey(name))
        {
            loadingRoom = true;
            var loader = LevelSerializer.LoadSavedLevel(rooms[name]);
            loader.showGUI = showGUI;
            loader.whenCompleted = (obj, list) =>
            {
                foreach (var gameObject in list)
                {
                    gameObject.SendMessage("OnRoomWasLoaded", SendMessageOptions.DontRequireReceiver);
                }
            };
        }
        else {
            var go = new GameObject("RoomLoader");
            go.AddComponent<RoomLoader>();
            SceneManager.LoadScene(name, LoadSceneMode.Single);
        }
    }
}