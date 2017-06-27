// /* ------------------
//       ${Name}
//       (c)3Radical 2012
//           by Mike Talbot
//     ------------------- */
//
using UnityEngine;

/// <summary>
/// Class PlayerSpawnPoint.
/// </summary>
[AddComponentMenu("Storage/Rooms/Examples/Player Spawn Point")]
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(StoreInformation))]
public class PlayerSpawnPoint : MonoBehaviour
{
    /// <summary>
    /// The current spawn point
    /// </summary>
    public static PlayerSpawnPoint currentSpawnPoint;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="PlayerSpawnPoint"/> is current.
    /// </summary>
    /// <value><c>true</c> if current; otherwise, <c>false</c>.</value>
    public bool current
    {
        get
        {
            return currentSpawnPoint == this;
        }
        set
        {
            if (value)
            {
                currentSpawnPoint = this;
            }
            else if (currentSpawnPoint == this)
            {
                currentSpawnPoint = null;
            }
        }
    }

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == PlayerLocator.PlayerGameObject)
        {
            current = true;
        }
    }

    private void OnRoomWasLoaded()
    {
        if (current)
        {
            PlayerLocator.Current.transform.position = transform.position;
            PlayerLocator.Current.transform.rotation = transform.rotation;
        }
    }
}