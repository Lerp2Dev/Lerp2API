using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class StoreInformation.
/// </summary>
[DontStore]
[ExecuteInEditMode]
[AddComponentMenu("Storage/Store Information")]
public class StoreInformation : UniqueIdentifier
{
    /// <summary>
    /// The store all components
    /// </summary>
    public bool StoreAllComponents = true;

    /// <summary>
    /// The components
    /// </summary>
    [HideInInspector]
    public List<string> Components = new List<string>();

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        foreach (var c in GetComponents<UniqueIdentifier>().Where(t => t.GetType() == typeof(UniqueIdentifier) ||
             (t.GetType() == typeof(StoreInformation) && t != this)))
        {
            DestroyImmediate(c);
        }
    }
}