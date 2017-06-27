// /* ------------------
//       ${Name}
//       (c)3Radical 2012
//           by Mike Talbot
//     ------------------- */
//
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class StoredReferences.
/// </summary>
[Serializable]
public class StoredReferences : ScriptableObject
{
    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        ById.Clear();
        ByObject.Clear();
        entries.Clear();
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>The count.</value>
    public int Count
    {
        get
        {
            FixEntries();
            return entries.Count;
        }
    }

    /// <summary>
    /// Gets all references.
    /// </summary>
    /// <value>All references.</value>
    public GameObject[] AllReferences
    {
        get
        {
            FixEntries();
            return entries.Select(g => g.gameObject).ToArray();
        }
    }

    private static List<SaveGameManager.StoredEntry> betweenSceneReferences = new List<SaveGameManager.StoredEntry>();

    private void OnDisable()
    {
        if (Application.isPlaying)
        {
            betweenSceneReferences = entries.Where(g => g.gameObject != null).ToList();
        }
    }

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            entries = entries.Concat(betweenSceneReferences.Where(g => g.gameObject != null)).Where(g => g.gameObject != null).ToList();
            FixEntries();
            ById.Clear();
            ByObject.Clear();
            betweenSceneReferences = entries.Where(g => g.gameObject != null).ToList();
        }
    }

    /// <summary>
    /// The entries
    /// </summary>
    public List<SaveGameManager.StoredEntry> entries = new List<SaveGameManager.StoredEntry>();
    private Dictionary<string, int> ById = new Dictionary<string, int>();
    private Dictionary<GameObject, int> ByObject = new Dictionary<GameObject, int>();

    /// <summary>
    /// Gets or sets the <see cref="SaveGameManager.StoredEntry"/> with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>SaveGameManager.StoredEntry.</returns>
    public SaveGameManager.StoredEntry this[string id]
    {
        get
        {
            EnsureDictionaries();
            if (ById.ContainsKey(id))
            {
                var se = entries[ById[id]];
                if (se.gameObject == null)
                {
                    ById.Remove(id);
                    return null;
                }
                return se;
            }
            return null;
        }
        set
        {
            var index = 0;
            if (!ById.TryGetValue(id, out index))
            {
                index = entries.Count;
                ById[id] = index;
                entries.Add(value);
            }
            entries[ById[id]] = value;
            ByObject[value.gameObject] = index;
        }
    }

    private void EnsureDictionaries()
    {
        if (ById.Count == 0 && entries.Count > 0)
        {
            FixEntries();
            var i = 0;
            foreach (var e in entries)
            {
                ById[e.Id] = i;
                ByObject[e.gameObject] = i++;
            }
        }
    }

    /// <summary>
    /// Gets or sets the <see cref="SaveGameManager.StoredEntry"/> with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>SaveGameManager.StoredEntry.</returns>
    public SaveGameManager.StoredEntry this[GameObject id]
    {
        get
        {
            EnsureDictionaries();
            if (ByObject.ContainsKey(id))
            {
                return entries[ByObject[id]];
            }
            return null;
        }

        set
        {
            var index = 0;
            if (!ByObject.TryGetValue(id, out index))
            {
                index = entries.Count;
                ByObject[id] = index;
                entries.Add(value);
            }
            entries[ByObject[id]] = value;
            ById[value.Id] = index;
        }
    }

    /// <summary>
    /// Removes the specified go.
    /// </summary>
    /// <param name="go">The go.</param>
    public void Remove(GameObject go)
    {
        var data = this[go];
        if (data != null)
        {
            ById.Remove(data.Id);
            ByObject.Remove(data.gameObject);
        }
    }

    /// <summary>
    /// Alives this instance.
    /// </summary>
    /// <returns>StoredReferences.</returns>
    public StoredReferences Alive()
    {
        var ret = CreateInstance<StoredReferences>();
        foreach (var e in entries)
        {
            if (e.gameObject != null)
            {
                ret[e.Id] = e;
            }
        }
        return ret;
    }

    private void FixEntries()
    {
        entries = entries.Where(g => g != null && g.gameObject != null && g.gameObject.GetComponent<UniqueIdentifier>() != null).ToList();
    }
}