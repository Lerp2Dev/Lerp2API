using Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = Lerp2API.DebugHandler.Debug;
using Lerp2API;

//Do not add this script to your own classes! This is created internally
[AddComponentMenu("Storage/Internal/Level Loader (Internal use only, do not add this to your objects!)")]
public class LevelLoader : MonoBehaviour
{
    #region Delegates

    public delegate void CreateObjectDelegate(GameObject prefab, ref bool cancel);

    public delegate void SerializedComponentDelegate(GameObject gameObject, string componentName, ref bool cancel);

    public delegate void SerializedObjectDelegate(GameObject gameObject, ref bool cancel);

    #endregion Delegates

    public static LevelLoader Current;
    private static Texture2D _pixel;
    public GameObject rootObject;
    private readonly Dictionary<string, int> _indexDictionary = new Dictionary<string, int>();
    public LevelSerializer.LevelData Data;
    public bool DontDelete = false;
    public GameObject Last;
    private float _alpha = 1;
    private bool _loading = true;
    public bool showGUI = true;
    public float timeScaleAfterLoading = 1;
    public Action<GameObject, List<GameObject>> whenCompleted = delegate { };

    public static event CreateObjectDelegate CreateGameObject = delegate { };

    public static event SerializedObjectDelegate OnDestroyObject = delegate { };

    public static event SerializedObjectDelegate LoadData = delegate { };

    public static event SerializedComponentDelegate LoadComponent = delegate { };

    public static event Action<Component> LoadedComponent = delegate { };

    public static Color guiColor = Color.white;

    private void Awake()
    {
        timeScaleAfterLoading = Time.timeScale;
        Current = this;
        if (_pixel == null)
        {
            _pixel = new Texture2D(1, 1);
        }
        SceneManager.sceneLoaded -= SceneWasLoaded;
        SceneManager.sceneLoaded += SceneWasLoaded;
    }

    private bool wasLoaded;

    private void SceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
        if (wasLoaded)
        {
            return;
        }
        wasLoaded = true;
        timeScaleAfterLoading = Time.timeScale;
        StartCoroutine(Load());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= SceneWasLoaded;
    }

    private void OnGUI()
    {
        if (!showGUI)
        {
            return;
        }
        if (!_loading && Event.current.type == EventType.repaint)
        {
            _alpha = Mathf.Clamp01(_alpha - 0.02f);
        }
        else if (Math.Abs(_alpha - 0) < float.Epsilon)
        {
            Destroy(gameObject);
        }
        if (!(Math.Abs(_alpha - 0) > float.Epsilon))
        {
            return;
        }
        guiColor.a = _alpha;
        _pixel.SetPixel(0, 0, guiColor);
        _pixel.Apply();
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _pixel, ScaleMode.StretchToFill);
    }

    private static void SetActive(GameObject go, bool activate)
    {
        go.SetActive(activate);
    }

    public IEnumerator Load()
    {
        yield return StartCoroutine(Load(2));
    }

    private static int loadingCount = 0;

    public IEnumerator Load(int numberOfFrames, float timeScale = 0)
    {
        if(Data == null)
            Debug.LogWarning("There was a problem loading Object Data!");

        ++loadingCount;
        //var oldFixedTime = Time.fixedDeltaTime;
        //if (Application.isPlaying)
        //    Time.fixedDeltaTime = 9;
        //Need to wait while the base level is prepared, it takes 2 frames
        while (numberOfFrames-- > 0)
            yield return new WaitForEndOfFrame();

        if (LevelSerializer.ShouldCollect && timeScale == 0)
            GC.Collect();

        LevelSerializer.RaiseProgress("Initializing", 0);
        if (Data != null && Data.rootObject != null)
            Debug.Log(Data.StoredObjectNames.Any(sn => sn.Name == Data.rootObject) ? "Located " + Data.rootObject : "Not found " + Data.rootObject);
        //Check if we should be deleting missing items
        if (!DontDelete)
            //First step is to remove any items that should not exist according to the saved scene
            if(Data != null)
            foreach (var go in
                    UniqueIdentifier.AllIdentifiers.Where(n => Data.StoredObjectNames.All(sn => sn.Name != n.Id)).ToList())
                try
                {
                    var cancel = false;
                    OnDestroyObject(go.gameObject, ref cancel);
                    if (!cancel)
                    {
                        Destroy(go.gameObject);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Problem destroying object " + go.name + " " + e.ToString());
                }

        var flaggedObjects = new List<UniqueIdentifier>();
        if(Data != null)
            flaggedObjects.AddRange(Data.StoredObjectNames.Select(c => UniqueIdentifier.GetByName(c.Name)).Where(c => c != null).Select(c => c.GetComponent<UniqueIdentifier>()));

        LevelSerializer.RaiseProgress("Initializing", 0.25f);

        var position = new Vector3(0, 2000, 2000);
        //Next we need to instantiate any items that are needed by the stored scene
        if(Data != null)
        foreach (var sto in
            Data.StoredObjectNames.Where(c => UniqueIdentifier.GetByName(c.Name) == null))
        {
            try
            {
                if (sto.createEmptyObject || sto.ClassId == null || !LevelSerializer.AllPrefabs.ContainsKey(sto.ClassId))
                {
                    sto.GameObject = new GameObject("CreatedObject");
                    sto.GameObject.transform.position = position;
                    var emptyObjectMarker = sto.GameObject.AddComponent<EmptyObjectIdentifier>();
                    sto.GameObject.AddComponent<StoreMesh>();
                    emptyObjectMarker.IsDeserializing = true;
                    emptyObjectMarker.Id = sto.Name;
                    if (emptyObjectMarker.Id == Data.rootObject)
                        Debug.Log("Set the root object on an empty");
                    flaggedObjects.Add(emptyObjectMarker);
                }
                else {
                    var pf = LevelSerializer.AllPrefabs[sto.ClassId];
                    var cancel = false;
                    CreateGameObject(pf, ref cancel);
                    if (cancel)
                    {
                        Debug.LogWarning("Cancelled");
                        continue;
                    }
                    var uis = pf.GetComponentsInChildren<UniqueIdentifier>();
                    foreach (var ui in uis)
                        ui.IsDeserializing = true;
                    sto.GameObject = Instantiate(pf, position, Quaternion.identity) as GameObject;
                    sto.GameObject.GetComponent<UniqueIdentifier>().Id = sto.Name;
                    if (sto.GameObject.GetComponent<UniqueIdentifier>().Id == Data.rootObject)
                        Debug.Log("Set the root object on a prefab");
                    foreach (var ui in uis)
                        ui.IsDeserializing = false;
                    flaggedObjects.AddRange(sto.GameObject.GetComponentsInChildren<UniqueIdentifier>());
                }

                position += Vector3.right * 50;
                sto.GameObject.GetComponent<UniqueIdentifier>().Id = sto.Name;
                sto.GameObject.name = sto.GameObjectName;
                if (sto.ChildIds.Count > 0)
                {
                    var list = sto.GameObject.GetComponentsInChildren<UniqueIdentifier>().ToList();
                    for (var i = 0; i < list.Count && i < sto.ChildIds.Count; i++)
                    {
                        list[i].Id = sto.ChildIds[i];
                    }
                }
                if (sto.Children.Count > 0)
                {
                    var list = LevelSerializer.GetComponentsInChildrenWithClause(sto.GameObject);
                    _indexDictionary.Clear();
                    foreach (var c in list)
                    {
                        if (!sto.Children.ContainsKey(c.ClassId))
                        {
                            continue;
                        }
                        if (!_indexDictionary.ContainsKey(c.ClassId))
                        {
                            _indexDictionary[c.ClassId] = 0;
                        }
                        c.Id = sto.Children[c.ClassId][_indexDictionary[c.ClassId]];
                        _indexDictionary[c.ClassId] = _indexDictionary[c.ClassId] + 1;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Debug.LogWarning("Problem creating an object " + sto.GameObjectName + " with classID " + sto.ClassId + " " + e);
            }
        }
        var loadedGameObjects = new HashSet<GameObject>();

        LevelSerializer.RaiseProgress("Initializing", 0.75f);

        if(Data != null)
        foreach (var so in Data.StoredObjectNames)
        {
            var go = UniqueIdentifier.GetByName(so.Name);
            if (go != null)
            {
                loadedGameObjects.Add(go);
                if (so.Components != null && so.Components.Count > 0)
                {
                    var all = go.GetComponents<Component>().Where(c => !typeof(UniqueIdentifier).IsAssignableFrom(c.GetType())).ToList();
                    foreach (var comp in all)
                        if (!so.Components.ContainsKey(comp.GetType().FullName))
                            if (Application.isPlaying)
                                Destroy(comp);
                            else
                                DestroyImmediate(comp);
                }
                SetActive(go, so.Active);
                if (so.setExtraData)
                {
                    go.layer = so.layer;
                    go.tag = so.tag;
                }
            }
            else
                Debug.LogError("Could not find " + so.GameObjectName + " " + so.Name);
        }

        LevelSerializer.RaiseProgress("Initializing", 0.85f);

        if (Data != null && rootObject != null)
            if (UniqueIdentifier.GetByName(Data.rootObject) == null)
                Debug.Log("No root object has been configured");

        foreach (var go in Data.StoredObjectNames.Where(c => !string.IsNullOrEmpty(c.ParentName)))
        {
            var parent = UniqueIdentifier.GetByName(go.ParentName);
            var item = UniqueIdentifier.GetByName(go.Name);
            if (item != null && parent != null)
            {
                item.SetParent(parent);
            }
        }

        LevelSerializer.RaiseProgress("Initializing", 1f);

        var currentProgress = 0;
        UnitySerializer.FinalProcess process;

        if(Data != null)
        using (new UnitySerializer.SerializationSplitScope())
        {
            using (new UnitySerializer.SerializationScope())
            {
                //Now we restore the data for the items
                foreach (var item in
                    Data.StoredItems.GroupBy(i => i.Name,
                                             (name, cps) => new
                                             {
                                                 Name = name,
                                                 Components = cps.Where(cp => cp.Name == name).GroupBy(cp => cp.Type,
                                                                                                                          (type, components) => new
                                                                                                                          {
                                                                                                                              Type = type,
                                                                                                                              List = components.ToList()
                                                                                                                          }).ToList()
                                             }))
                {
                    Debug.LogFormat("\n*****************\n{0}\n********START**********\n", item.Name);
                    var go = UniqueIdentifier.GetByName(item.Name);
                    if (go == null)
                    {
                        Debug.LogWarning(item.Name + " was null");
                        continue;
                    }

                    foreach (var cp in item.Components)
                    {
                        try
                        {
                            LevelSerializer.RaiseProgress("Loading", (float)++currentProgress / (float)Data.StoredItems.Count);
                            var type = UnitySerializer.GetTypeEx(cp.Type);
                            if (type == null)
                            {
                                continue;
                            }
                            Last = go;
                            var cancel = false;
                            LoadData(go, ref cancel);
                            LoadComponent(go, type.Name, ref cancel);
                            if (cancel)
                            {
                                continue;
                            }

                            Debug.LogFormat("<{0}>\n", type.FullName);

                            var list = go.GetComponents(type).Where(c => c.GetType() == type).ToList();
                            //Make sure the lists are the same length
                            while (list.Count > cp.List.Count)
                            {
                                DestroyImmediate(list.Last());
                                list.Remove(list.Last());
                            }
                            if (type == typeof(NavMeshAgent))
                            {
                                var cp1 = cp;
                                var item1 = item;
                                Action perform = () =>
                                {
                                    var comp = cp1;
                                    var tp = type;
                                    var tname = item1.Name;
                                    UnitySerializer.AddFinalAction(() =>
                                    {
                                        var g = UniqueIdentifier.GetByName(tname);
                                        var nlist = g.GetComponents(tp).Where(c => c.GetType() == tp).ToList();
                                        while (nlist.Count < comp.List.Count)
                                        {
                                            try
                                            {
                                                nlist.Add(g.AddComponent(tp));
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        list = list.Where(l => l != null).ToList();
                                        //Now deserialize the items back in
                                        for (var i = 0; i < nlist.Count; i++)
                                        {
                                            if (LevelSerializer.CustomSerializers.ContainsKey(tp))
                                            {
                                                LevelSerializer.CustomSerializers[tp].Deserialize((byte[])comp.List[i].Data, nlist[i]);
                                            }
                                            else {
                                                UnitySerializer.DeserializeInto(comp.List[i].Data, nlist[i]);
                                            }
                                            LoadedComponent(nlist[i]);
                                        }
                                    });
                                };
                                perform();
                            }
                            else {
                                while (list.Count < cp.List.Count)
                                {
                                    try
                                    {
                                        Debug.Log("Adding component of type " + type.ToString());
                                        // Try to resolve [RequireComponent] attributes by iteratively adding the components that
                                        // don't have any requirements first
                                        var requirements = new List<Type>() { type };
                                        bool updated = false;
                                        do
                                        {
                                            int before = requirements.Count();
                                            var r = requirements.SelectMany(it => it.GetCustomAttributes(typeof(RequireComponent), true).
                                                Select(itm => new[] { ((RequireComponent)itm).m_Type0, ((RequireComponent)itm).m_Type1, ((RequireComponent)itm).m_Type2 }).SelectMany(itm => itm).Where(itm => itm != null).Distinct()).ToList();

                                            requirements.AddRange(r.Where(a => !requirements.Contains(a)));
                                            updated = before != requirements.Count();
                                        } while (updated);
                                        requirements.RemoveAt(0);

                                        foreach (var req in requirements)
                                        {
                                            go.AddComponent(UnitySerializer.GetTypeEx(item.Components.FirstOrDefault(a => req.IsAssignableFrom(UnitySerializer.GetTypeEx(a.Type))).Type));
                                        }

                                        list.Add(go.AddComponent(type));
                                    }
                                    catch { }
                                }
                                list = list.Where(l => l != null).ToList();
                                //Now deserialize the items back in
                                for (var i = 0; i < list.Count; i++)
                                {
                                    Debug.Log(string.Format("Deserializing {0} for {1}", type.Name, go.GetFullName()));
                                    if (LevelSerializer.CustomSerializers.ContainsKey(type))
                                    {
                                        LevelSerializer.CustomSerializers[type].Deserialize(cp.List[i].Data, list[i]);
                                    }
                                    else {
                                        UnitySerializer.DeserializeInto(cp.List[i].Data, list[i]);
                                    }
                                    LoadedComponent(list[i]);
                                }
                            }
                            Debug.LogFormat("</{0}>", type.FullName);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("Problem deserializing " + cp.Type + " for " + go.name + " " + e.ToString());
                        }
                    }
                    Debug.LogFormat("\n*****************\n{0}\n********END**********\n\n", item.Name);
                }
                process = UnitySerializer.TakeOwnershipOfFinalization();
            }

            UnitySerializer.RunDeferredActions(process);

            if (LevelSerializer.ShouldCollect && timeScale == 0)
            {
                Resources.UnloadUnusedAssets();
                GC.Collect();
            }

            PluginsHelper.Add(Last);

            UnitySerializer.InformDeserializedObjects(process);

            //Tell the world that the level has been loaded
            if (Data != null && Data.rootObject != null)
                rootObject = UniqueIdentifier.GetByName(Data.rootObject);
            else
                rootObject = null;

            if (rootObject == null && Data.rootObject != null)
            {
                Debug.LogError("Could not find the root object");
                Debug.Log(Data.rootObject + " not found " + (!Data.StoredObjectNames.Any(n => n.Name == Data.rootObject) ? "not in the stored names" : "was in the stored names"));
            }

            //Flag that we aren't deserializing
            foreach (var obj in flaggedObjects)
            {
                obj.IsDeserializing = false;
                obj.SendMessage("OnDeserialized", SendMessageOptions.DontRequireReceiver);
            }

            LevelSerializer.IsDeserializing = false;
            _loading = false;
            RoomManager.loadingRoom = false;
            whenCompleted(rootObject, loadedGameObjects.ToList());

            if (Application.isEditor)
            {
                Last.hideFlags = HideFlags.HideAndDontSave;
                Last.GetComponent<MeshRenderer>().enabled = false;
            }

            //Get rid of the current object that is holding this level loader, it was
            //created solely for the purpose of running this script
            if (Application.isPlaying)
                Destroy(gameObject, 0.1f);
            else
                DestroyImmediate(gameObject);
        }
    }
}