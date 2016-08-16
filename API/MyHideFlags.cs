using UnityEngine;

[ExecuteInEditMode]
public class MyHideFlags : MonoBehaviour
{
    public static void Remove()
    {
        MyHideFlags[] list = FindObjectsOfType<MyHideFlags>();
        int l = list.Length;
        foreach (MyHideFlags mhf in list)
            if(mhf != null && mhf.gameObject != null)
                if (Application.isEditor)
                    DestroyImmediate(mhf.gameObject);
                else
                    Destroy(mhf.gameObject);
        Debug.Log(l+" ghosts deleted!");
    }

    public HideFlags myHideFlags = HideFlags.HideAndDontSave;
    public bool disabled;

    void OnEnable()
    {
        if (Application.isEditor)
        {
            gameObject.hideFlags = myHideFlags;
            if (GetComponent<MeshRenderer>() != null)
                GetComponent<MeshRenderer>().enabled = false;
            disabled = true;
        }
    }

    void OnDestroy()
    {
        if(disabled)
        {
            gameObject.hideFlags = HideFlags.None;
            if (GetComponent<MeshRenderer>() != null)
                GetComponent<MeshRenderer>().enabled = true;
        }
    }

    void Update()
    {
        if(disabled && Application.isPlaying)
        {
            gameObject.hideFlags = HideFlags.None;
            if (GetComponent<MeshRenderer>() != null)
                GetComponent<MeshRenderer>().enabled = true;
            disabled = false;
        }
    }
}
