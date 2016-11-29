using System.Collections;
using UnityEngine;

public class FallingAvoider : MonoBehaviour
{
    public static FallingAvoider m_current;

    //private Vector3 lastPos;
    public bool m_message;
    public bool m_underZero;

    [HideInInspector]
    public bool teleported = true;

    private void Awake()
    {
        m_current = this;
    }

    // Use this for initialization
    private void Start()
    {
        //StartCoroutine("FindGround", true);
    }

    // Update is called once per frame
    private void Update()
    {
        if (teleported)
        {
            StartCoroutine("FindGround", true);
            teleported = false;
        }
        if (m_underZero && transform.position.y < 0)
        {
            transform.position = transform.position + Vector3.up * 1000;
            teleported = true;
        }
        //lastPos = transform.position;
    }

    public IEnumerator FindGround(bool debug = false)
    {
        if (debug)
            Debug.Log("Trying to find ground!");
        RaycastHit hit;
        bool hitted = Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, Mathf.Infinity);
        while (!hitted)
            yield return null;
        if (hitted)
            transform.position = hit.point + Vector3.up * transform.localScale.y;
        if (debug)
            Debug.Log("Ground founded and teleported to it!");
    }


}