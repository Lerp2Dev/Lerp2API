using Serialization;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class EmptyObjectIdentifier.
/// </summary>
[DontStore]
[AddComponentMenu("Storage/Empty Object Identifier")]
[ExecuteInEditMode]
public class EmptyObjectIdentifier : StoreInformation
{
    /// <summary>
    /// Awakes this instance.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        if (!gameObject.GetComponent<StoreMaterials>())
            gameObject.AddComponent<StoreMaterials>();
    }

    /// <summary>
    /// Flags all.
    /// </summary>
    /// <param name="gameObject">The game object.</param>
    public static void FlagAll(GameObject gameObject)
    {
        foreach (var c in gameObject.GetComponentsInChildren<Transform>().Where(c => !c.GetComponent<UniqueIdentifier>()))
            c.gameObject.AddComponent<EmptyObjectIdentifier>();
    }
}