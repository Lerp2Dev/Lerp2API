using UnityEngine;
using System.Collections;

public class LinkObject {
  public LinkObject prev = null;
  public LinkObject next = null;
  public GameObject data;
  
  public LinkObject(GameObject link) {
    data = link;
  }
}

public class GameObjectQueue {
  
  public GameObject objectPrefab;

  private LinkObject list = null;
  private LinkObject last = null;
  
  private int limit = 0;
  private int count = 0;
  private int objectCount = 0;
  
  public GameObjectQueue(GameObject prefab, int spawnLimit) {
    objectPrefab = prefab;
    limit        = spawnLimit;
  }
  
  public int DebugCount() {
    return count;
  }
  
  public int DebugObjectCount() {
    return objectCount;
  }
  
  public void Prime(int amount) {
    for (int i = 0; i < amount; i++) {
      GameObject obj = Object.Instantiate(objectPrefab) as GameObject;
      obj.GetComponent<QueueBehaviour>().DestroyToQueue();
    }
  }
  
  public void Push(GameObject link) {
    count++;
    if (list == null) {
      list = new LinkObject(link);
      last = list;
    } else {
      LinkObject obj = new LinkObject(link);
      
      list.prev = obj;
      obj.next  = list;
      list      = obj;
    }
  }
  
  public GameObject Pop() {
    if (limit > 0 && objectCount > limit)
      return null;

    if (list == null) {
      objectCount++;
      GameObject obj = Object.Instantiate(objectPrefab) as GameObject;
      return obj;
    } else {
      count--;
      
      // set object to last in list
      LinkObject obj = last;
      // pull out the data
      GameObject link = obj.data;
      // reset the list to the previous object
      if (obj.prev != null) {
        last = obj.prev;
        last.next = null;
      } else {
        list.next = null;
        list.prev = null;
        list.data = null;
        list = last = null;
      }
      
      // reset object
      link.GetComponent<QueueBehaviour>().Reset();
      
      // return GameObject
      return link;
    }
  }

}
