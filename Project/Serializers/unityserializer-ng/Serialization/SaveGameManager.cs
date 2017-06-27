// /* ------------------
//
//       (c) whydoidoit.com 2012
//           by Mike Talbot
//     ------------------- */
//
using Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

/// <summary>
/// Class SaveGameManager.
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("Storage/Save Game Manager")]
public class SaveGameManager : MonoBehaviour
{
    /// <summary>
    /// The required objects
    /// </summary>
    public UnityEngine.Object[] requiredObjects;

    private static SaveGameManager instance;

    /// <summary>
    /// Gets or sets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static SaveGameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectsOfType(typeof(GameObject))
                    .Cast<GameObject>()
                    .Where(g => g.GetComponent<SaveGameManager>() != null)
                    .Select(g => g.GetComponent<SaveGameManager>())
                    .FirstOrDefault();

                if (instance == null) // && Application.isPlaying
                {
                    var saveGameManager = new GameObject("Save Game Manager");
                    instance = saveGameManager.AddComponent<SaveGameManager>();
                    Debug.LogWarning("Creating a save game manager dynamically, consider adding one to the scene");
                }
            }

            return instance;
        }
        set
        {
            instance = value;
        }
    }

    /// <summary>
    /// The has run
    /// </summary>
    public static bool hasRun;

    /// <summary>
    /// Loadeds this instance.
    /// </summary>
    public static void Loaded()
    {
        _cached = null;
    }

    /// <summary>
    /// Class StoredEntry.
    /// </summary>
    [Serializable]
    public class StoredEntry
    {
        /// <summary>
        /// The game object
        /// </summary>
        [NonSerialized]
        public GameObject gameObject;

        /// <summary>
        /// The identifier
        /// </summary>
        public string Id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// The reference
    /// </summary>
    [HideInInspector]
    public StoredReferences Reference;

    private static StoredReferences _cached;

    //private static Dictionary<string, StoredEntry> _cached = new Dictionary<string, StoredEntry>();
    private static List<Action> _initActions = new List<Action>();

    /// <summary>
    /// Gets the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>GameObject.</returns>
    public GameObject GetById(string id)
    {
        var se = Instance.Reference[id];
        return se != null ? se.gameObject : null;
    }

    /// <summary>
    /// Sets the identifier.
    /// </summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="id">The identifier.</param>
    public void SetId(GameObject gameObject, string id)
    {
        var rr = Instance.Reference[gameObject] ?? Instance.Reference[id];
        if (rr != null)
        {
            Instance.Reference.Remove(rr.gameObject);
            rr.Id = id;
            rr.gameObject = gameObject;
        }
        else {
            rr = new StoredEntry { gameObject = gameObject, Id = id };
        }
        Instance.Reference[rr.Id] = rr;
    }

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    /// <param name="gameObject">The game object.</param>
    /// <returns>System.String.</returns>
    public static string GetId(GameObject gameObject)
    {
        if (Instance == null || gameObject == null)
            return string.Empty;
        var entry = Instance.Reference[gameObject];
        if (entry != null)
            return entry.Id;
        // TODO: Keep an eye on this. Application.isLoadingLevel was deprecated without any equivalent replacement. It still works though, so I won't touch it for now.
#pragma warning disable
        if (Application.isLoadingLevel && !Application.isPlaying)
            return null;
#pragma warning restore
        entry = new StoredEntry { gameObject = gameObject };
        Instance.Reference[entry.Id] = entry;
        return entry.Id;
    }

    private bool hasWoken;

    /// <summary>
    /// Initializes the specified a.
    /// </summary>
    /// <param name="a">a.</param>
    public static void Initialize(Action a)
    {
        if (Instance != null && Instance.hasWoken)
        {
            a();
        }
        else {
            _initActions.Add(a);
        }
    }

    private Dictionary<Type, Index<string, List<UnityEngine.Object>>> assetReferences = new Dictionary<Type, Index<string, List<UnityEngine.Object>>>();

    /// <summary>
    /// Gets the asset identifier.
    /// </summary>
    /// <param name="referencedObject">The referenced object.</param>
    /// <returns>AssetReference.</returns>
    public AssetReference GetAssetId(UnityEngine.Object referencedObject)
    {
        if (referencedObject == null) return new AssetReference { index = -1 };
        Index<string, List<UnityEngine.Object>> nameLookup = null;
        var type = referencedObject.GetType();
        if (!assetReferences.TryGetValue(type, out nameLookup))
        {
            assetReferences[type] = nameLookup = new Index<string, List<UnityEngine.Object>>();
            var objectsOfType = Resources.FindObjectsOfTypeAll(type).Except(FindObjectsOfType(type));
            foreach (var reference in objectsOfType)
            {
                nameLookup[reference.name].Add(reference);
            }
        }
        List<UnityEngine.Object> references = null;
        if (!nameLookup.TryGetValue(referencedObject.name, out references))
        {
            return new AssetReference { index = -1 };
        }
        return new AssetReference { index = references.IndexOf(referencedObject), name = referencedObject.name, type = type.FullName };
    }

    /// <summary>
    /// Class AssetReference.
    /// </summary>
    public class AssetReference
    {
        /// <summary>
        /// The name
        /// </summary>
        public string name;
        /// <summary>
        /// The type
        /// </summary>
        public string type;
        /// <summary>
        /// The index
        /// </summary>
        public int index;
    }

    /// <summary>
    /// Gets the asset.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>System.Object.</returns>
    public object GetAsset(AssetReference id)
    {
        if (id.index == -1)
        {
            return null;
        }
        try
        {
            var type = UnitySerializer.GetTypeEx(id.type);
            Index<string, List<UnityEngine.Object>> nameLookup;
            if (!assetReferences.TryGetValue(type, out nameLookup))
            {
                assetReferences[type] = nameLookup = new Index<string, List<UnityEngine.Object>>();
                var objectsOfType = Resources.FindObjectsOfTypeAll(type).Except(UnityEngine.Object.FindObjectsOfType(type));
                foreach (var reference in objectsOfType)
                {
                    nameLookup[reference.name].Add(reference);
                }
            }
            List<UnityEngine.Object> references;
            if (!nameLookup.TryGetValue(id.name, out references))
            {
                return null;
            }
            if (id.index >= references.Count)
            {
                return null;
            }
            return references[id.index];
        }
        catch
        {
            return null;
        }
    }

    private void OnDestroy()
    {
        DestroyImmediate(Reference);
    }

    private void GetAllInactiveGameObjects()
    {
        var items = Reference.AllReferences.Select(g => g.transform);
        RecurseAddInactive(items);
    }

    private void RecurseAddInactive(IEnumerable<Transform> items)
    {
        foreach (var child in items)
        {
            if (child.GetComponent<UniqueIdentifier>() != null)
            {
                if (!child.gameObject.activeInHierarchy)
                {
                    GetId(child.gameObject);
                }
            }
            RecurseAddInactive(child.Cast<Transform>());
        }
    }

    private void Awake()
    {
        Loom.Initialize();
        if (Reference == null)
        {
            Reference = ScriptableObject.CreateInstance<StoredReferences>();
        }
        if (Application.isEditor)
        {
            GetAllInactiveGameObjects();
        }
        if (Instance != null && Instance != this)
            Destroy(Instance.gameObject);
        Instance = this;
        hasWoken = true;
        if (Application.isPlaying && !hasRun)
        {
            _cached = Reference;
            hasRun = true;
        }
        else if (!Application.isPlaying)
        {
            hasRun = false;
            if (_cached != null && _cached.Count > 0)
                Reference = _cached.Alive();
        }
        if (_initActions.Count > 0)
        {
            foreach (var a in _initActions)
            {
                a();
            }
            _initActions.Clear();
        }
    }
}