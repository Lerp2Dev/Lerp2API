// /* ------------------
//       ${Name} 
//       (c)3Radical 2012
//           by Mike Talbot 
//     ------------------- */
// 
using UnityEngine;

[AddComponentMenu("Storage/Rooms/Examples/Player Spawn Point")]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(StoreInformation))]
public class PlayerSpawnPoint : MonoBehaviour {
    public static PlayerSpawnPoint currentSpawnPoint;

    public bool current {
        get {
            return currentSpawnPoint == this;
        }
        set {
            if (value) {
                currentSpawnPoint = this;
            }
            else if (currentSpawnPoint == this) {
                currentSpawnPoint = null;
            }
        }
    }

    private void Awake() {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == PlayerLocator.PlayerGameObject) {
            current = true;
        }
    }

    private void OnRoomWasLoaded() {
        if (current) {
            PlayerLocator.Current.transform.position = transform.position;
            PlayerLocator.Current.transform.rotation = transform.rotation;
        }
    }
}