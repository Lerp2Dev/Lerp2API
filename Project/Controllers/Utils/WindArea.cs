using System.Collections.Generic;
using UnityEngine;

namespace Lerp2API.Controllers.Utils
{
    //Use this component with a WindZone to affect to rigidbodies also.
    /// <summary>
    /// Class WindArea.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [RequireComponent(typeof(WindZone))]
    public class WindArea : MonoBehaviour
    {
        private List<Rigidbody> rigidbodiesInWindzoneList = new List<Rigidbody>();
        private Vector3 windDirection = Vector3.right;
        private float windStrength = 5;
        private bool working;

        private void Start()
        {
            WindZone zone = GetComponent<WindZone>();
            windDirection = transform.eulerAngles.normalized;

            working = zone != null;

            if (working)
                windStrength = zone.windMain;
            else
                Debug.LogError("This component require a WindZone component.");
        }

        private void OnTriggerEnter(Collider col)
        {
            if (!working)
                return;

            Rigidbody objectRigid = col.gameObject.GetComponent<Rigidbody>();

            if (objectRigid != null)
                rigidbodiesInWindzoneList.Add(objectRigid);
        }

        private void OnTriggerExit(Collider col)
        {
            if (!working)
                return;

            Rigidbody objectRigid = col.gameObject.GetComponent<Rigidbody>();

            if (objectRigid != null)
                rigidbodiesInWindzoneList.Remove(objectRigid);
        }

        private void FixedUpdate()
        {
            if (!working)
                return;

            if (rigidbodiesInWindzoneList.Count > 0)
                foreach (Rigidbody rigid in rigidbodiesInWindzoneList)
                    rigid.AddForce(windDirection * windStrength);
        }
    }
}