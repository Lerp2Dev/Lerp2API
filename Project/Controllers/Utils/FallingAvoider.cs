using System.Collections;
using UnityEngine;

public class FallingAvoider : MonoBehaviour
{
    public static FallingAvoider m_current;

    public bool m_message;
    public bool m_underZero = true;
    public bool m_findGroundAtStart = true;

    [HideInInspector]
    public bool teleported = true;

    private RaycastHit hit;
    private bool hitted
    {
        get
        {
            return Physics.Raycast(new Ray(transform.position, Vector3.down), out hit, Mathf.Infinity);
        }
    }

    private void Awake()
    {
        m_current = this;
    }

    // Use this for initialization
    private void Start()
    {
        if(m_findGroundAtStart && !hitted)
            StartCoroutine("FindGround", m_message);
    }

    // Update is called once per frame
    private void Update()
    {
        if (teleported)
        {
            StartCoroutine("FindGround", m_message);
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
    { //I don't know why I have to press a key to make it work...
        if (debug)
            Debug.Log("Trying to find ground!");
        while (!hitted)
            yield return null;
        if (!hit.Equals(default(RaycastHit)))
            transform.position = hit.point + Vector3.up * transform.localScale.y;
        if (debug)
            Debug.Log("Ground founded and teleported to it!");
    }


}