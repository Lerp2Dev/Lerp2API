using System.Collections;
using UnityEngine;

public class FallingAvoider : MonoBehaviour
{
    //private Vector3 lastPos;
    public bool m_message;

    [HideInInspector]
    public bool teleported;

    // Use this for initialization
    void Start()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (teleported)
            FindGround();
        if (transform.position.y < 0)
        {
            transform.position = transform.position + Vector3.up * 1000;
            teleported = true;
        }
        //lastPos = transform.position;
	}

    public bool FindGround()
    {
        RaycastHit hit;
        bool hitted = Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, Mathf.Infinity);
        if (hitted)
        {
            transform.position = hit.point + Vector3.up * transform.localScale.y;
            teleported = false;
        }
        return hitted;
    }
}
