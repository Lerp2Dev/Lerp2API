// /* ------------------
//       ${Name} 
//       (c)3Radical 2012
//           by Mike Talbot 
//     ------------------- */
// 
using UnityEngine;

[AddComponentMenu("Storage/Rooms/Examples/Player Locator")]
public class PlayerLocator : MonoBehaviour {
    public static PlayerLocator Current;
    public static GameObject PlayerGameObject;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        Current = this;
        PlayerGameObject = gameObject;
    }
}