using System.Collections;
using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2API.Controllers
{
    public class FallingAvoider : MonoBehaviour
    {
        //private Vector3 lastPos;
        public bool m_message;

        [HideInInspector]
        public bool teleported;

        // Use this for initialization
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (teleported)
                StartCoroutine("FindGround");
            if (transform.position.y < 0)
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
            {
                transform.position = hit.point + Vector3.up * transform.localScale.y;
                teleported = false;
            }
            if (debug)
                Debug.Log("Ground founded and teleported to it!");
        }
    }
}