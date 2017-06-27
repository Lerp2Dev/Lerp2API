using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Store this component when saving data
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StoreComponent : Attribute { }

/// <summary>
/// Class DontStoreAttribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DontStoreAttribute : Attribute { }

/// <summary>
/// Class UniqueIdentifier.
/// </summary>
[ExecuteInEditMode]
[DontStore]
[AddComponentMenu("Storage/Unique Identifier")]
public class UniqueIdentifier : MonoBehaviour
{
    /// <summary>
    /// The is deserializing
    /// </summary>
    [HideInInspector]
    public bool IsDeserializing;

    /// <summary>
    /// The identifier
    /// </summary>
    public string _id = string.Empty;

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public string Id
    {
        get
        {
            if (gameObject == null)
            {
                return _id;
            }
            if (!string.IsNullOrEmpty(_id))
            {
                return _id;
            }
            return _id = SaveGameManager.GetId(gameObject);
        }
        set
        {
            _id = value;
            SaveGameManager.Instance.SetId(gameObject, value);
        }
    }

    private static List<UniqueIdentifier> allIdentifiers = new List<UniqueIdentifier>();

    /// <summary>
    /// Gets or sets all identifiers.
    /// </summary>
    /// <value>All identifiers.</value>
    public static List<UniqueIdentifier> AllIdentifiers
    {
        get
        {
            allIdentifiers = allIdentifiers.Where(a => a != null).ToList();
            return allIdentifiers;
        }
        set
        {
            allIdentifiers = value;
        }
    }

    /// <summary>
    /// The class identifier
    /// </summary>
    [HideInInspector]
    public string classId = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the class identifier.
    /// </summary>
    /// <value>The class identifier.</value>
    public string ClassId
    {
        get
        {
            return classId;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                value = Guid.NewGuid().ToString();
            }
            classId = value;
        }
    }

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    protected virtual void Awake()
    {
        foreach (var c in GetComponents<UniqueIdentifier>().Where(t => t.GetType() == typeof(UniqueIdentifier) && t != this))
        {
            DestroyImmediate(c);
        }

        SaveGameManager.Initialize(() =>
        {
            if (gameObject) { FullConfigure(); }
        });
    }

    private void OnDestroy()
    {
        if (AllIdentifiers.Count > 0)
        {
            AllIdentifiers.Remove(this);
        }
    }

    /// <summary>
    /// Fulls the configure.
    /// </summary>
    public void FullConfigure()
    {
        ConfigureId();
        foreach (var c in GetComponentsInChildren<UniqueIdentifier>(true).Where(c => c.gameObject.activeInHierarchy == false))
        {
            c.ConfigureId();
        }
    }

    private void ConfigureId()
    {
        _id = SaveGameManager.GetId(gameObject);
        AllIdentifiers.Add(this);
    }

    /// <summary>
    /// Gets the name of the by.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>GameObject.</returns>
    public static GameObject GetByName(string id)
    {
        var result = SaveGameManager.Instance.GetById(id);
        return result ?? GameObject.Find(id);
    }
}

/// <summary>
/// Class SerializationHelper.
/// </summary>
public static class SerializationHelper
{
    /// <summary>
    /// Determines whether the specified go is deserializing.
    /// </summary>
    /// <param name="go">The go.</param>
    /// <returns><c>true</c> if the specified go is deserializing; otherwise, <c>false</c>.</returns>
    public static bool IsDeserializing(this GameObject go)
    {
        var ui = go.GetComponent<UniqueIdentifier>();
        return ui != null ? ui.IsDeserializing : false;
    }
}