using UnityEngine;
using System.Collections;

// Inherits from custom class CoreBehaviour which inherits from Unity's MonoBehaviour
// and implements small optimizations such as transform caching, etc.
/*public class QueueBehaviour : CoreBehaviour
{
    // the string signifying this object type for queue tracking
    public string prefabType;

    public virtual void Reset()
    {
        // optional reset handler
        transform.SendMessage("OnReset", SendMessageOptions.DontRequireReceiver);

        // various bits of code to manage turning on colliders, audio, animations, etc
        // or simply enable the object
        //...
    }

    public virtual void DestroyToQueue()
    {
        // optional destroy handler
        transform.SendMessage("OnDestroyToQueue", SendMessageOptions.DontRequireReceiver);

        // various bits of code to manage turning off colliders, audio, animations, etc
        // or simply disable the object
        //...

        queueSystem.Push(prefabType, gameObject);
    }
}*/