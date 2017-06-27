// /* ------------------
//       ${Name}
//       (c)3Radical 2012
//           by Mike Talbot
//     ------------------- */
//
using UnityEngine;

/// <summary>
/// Class PlayerLocator.
/// </summary>
[AddComponentMenu("Storage/Rooms/Examples/Player Locator")]
public class PlayerLocator : MonoBehaviour
{
    /// <summary>
    /// The current
    /// </summary>
    public static PlayerLocator Current;
    /// <summary>
    /// The player game object
    /// </summary>
    public static GameObject PlayerGameObject;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Current = this;
        PlayerGameObject = gameObject;
    }
}