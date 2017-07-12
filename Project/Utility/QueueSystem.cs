using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class QueueLinks {
  public string prefabType;
  public GameObject prefab;
  public int spawnLimit   = 0;
  public int primerAmount = 0;
}

public class QueueSystem : MonoBehaviour {

  public QueueLinks[] queueLinks;
  private Dictionary<string, GameObjectQueue> queueDict = new Dictionary<string, GameObjectQueue>();
  
  void Awake() {
    foreach (QueueLinks p in queueLinks) {
      queueDict.Add(p.prefabType, new GameObjectQueue(p.prefab, p.spawnLimit));
    }
  }
  
  void Start() {
    foreach (QueueLinks p in queueLinks) {
      queueDict[p.prefabType].Prime(p.primerAmount);
    }
  }
  
  public int DebugCount(string type) {
    return queueDict[type].DebugCount();
  }
  
  public int DebugObjectCount(string type) {
    return queueDict[type].DebugObjectCount();
  }
  
  public void Push(string type, GameObject obj) {
    queueDict[type].Push(obj);
  }
  
  public GameObject Pop(string obj) {
    return queueDict[obj].Pop();
  }

}
