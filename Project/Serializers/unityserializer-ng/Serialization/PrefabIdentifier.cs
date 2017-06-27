using System.Linq;
using UnityEngine;

/// <summary>
/// Class PrefabIdentifier.
/// </summary>
[DontStore]
[AddComponentMenu("Storage/Prefab Identifier")]
[ExecuteInEditMode]
public class PrefabIdentifier : StoreInformation
{
    private bool inScenePrefab;

    /// <summary>
    /// Determines whether [is in scene].
    /// </summary>
    /// <returns><c>true</c> if [is in scene]; otherwise, <c>false</c>.</returns>
    public bool IsInScene()
    {
        return inScenePrefab;
    }

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    protected override void Awake()
    {
        inScenePrefab = true;
        base.Awake();
        foreach (var c in GetComponents<UniqueIdentifier>().Where(t => t.GetType() == typeof(UniqueIdentifier) ||
            (t.GetType() == typeof(PrefabIdentifier) && t != this) ||
            t.GetType() == typeof(StoreInformation)
            ))
        {
            DestroyImmediate(c);
        }
    }
}