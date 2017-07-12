using UnityEngine;
using System.Collections;

/// <summary>
/// Class WowCamera.
/// </summary>
public class WowCamera : MonoBehaviour
{
    /// <summary>
    /// The target
    /// </summary>
    public Transform target;

    /// <summary>
    /// The target height
    /// </summary>
    public float targetHeight = 1.7f;

    /// <summary>
    /// The distance
    /// </summary>
    public float distance = 5.0f;

    /// <summary>
    /// The offset from wall
    /// </summary>
    public float offsetFromWall = 0.1f;

    /// <summary>
    /// The maximum distance
    /// </summary>
    public float maxDistance = 20;

    /// <summary>
    /// The minimum distance
    /// </summary>
    public float minDistance = .6f;

    /// <summary>
    /// The x speed
    /// </summary>
    public float xSpeed = 200.0f;

    /// <summary>
    /// The y speed
    /// </summary>
    public float ySpeed = 200.0f;

    /// <summary>
    /// The target speed
    /// </summary>
    public float targetSpeed = 5.0f;

    /// <summary>
    /// The y minimum limit
    /// </summary>
    public int yMinLimit = -80;

    /// <summary>
    /// The y maximum limit
    /// </summary>
    public int yMaxLimit = 80;

    /// <summary>
    /// The zoom rate
    /// </summary>
    public int zoomRate = 40;

    /// <summary>
    /// The rotation dampening
    /// </summary>
    public float rotationDampening = 3.0f;

    /// <summary>
    /// The zoom dampening
    /// </summary>
    public float zoomDampening = 5.0f;

    /// <summary>
    /// The collision layers
    /// </summary>
    public LayerMask collisionLayers = -1;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private float correctedDistance;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        xDeg = angles.x;
        yDeg = angles.y;

        currentDistance = distance;
        desiredDistance = distance;
        correctedDistance = distance;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>() != null)
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    private void Update()
    {
        //Move the Player with left & right button press together
        if (Input.GetMouseButton(1) && Input.GetMouseButton(0))
        {
            float targetRotationAngle = target.eulerAngles.y;
            float currentRotationAngle = transform.eulerAngles.y;
            xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
            target.transform.Rotate(0, Input.GetAxis("Mouse X") * xSpeed * 0.02f, 0);
            xDeg += Input.GetAxis("Mouse X") * targetSpeed * 0.02f;
            target.transform.Translate(Vector3.forward * targetSpeed * Time.deltaTime);
        }
    }

    /**
     * Camera logic on LateUpdate to only update after all character movement logic has been handled.
     */

    private void LateUpdate()
    {
        Vector3 vTargetOffset;

        // Don't do anything if target is not defined
        if (!target)
            return;

        // If either mouse buttons are down, let the mouse govern camera position
        if (Input.GetMouseButton(0))
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }
        //Reset the camera angle and Rotate the Target Around the World!
        else if (Input.GetMouseButton(1))
        {
            float targetRotationAngle = target.eulerAngles.y;
            float currentRotationAngle = transform.eulerAngles.y;
            xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
            target.transform.Rotate(0, Input.GetAxis("Mouse X") * xSpeed * 0.02f, 0);
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
        }

        // otherwise, ease behind the target if any of the directional keys are pressed
        else if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            float targetRotationAngle = target.eulerAngles.y;
            float currentRotationAngle = transform.eulerAngles.y;
            xDeg = Mathf.LerpAngle(currentRotationAngle, targetRotationAngle, rotationDampening * Time.deltaTime);
        }

        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

        // set camera rotation
        Quaternion rotation = Quaternion.Euler(yDeg, xDeg, 0);

        // calculate the desired distance
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        correctedDistance = desiredDistance;

        // calculate desired camera position
        vTargetOffset = new Vector3(0, -targetHeight, 0);
        Vector3 position = target.position - (rotation * Vector3.forward * desiredDistance + vTargetOffset);

        // check for collision using the true target's desired registration point as set by user using height
        RaycastHit collisionHit;
        Vector3 trueTargetPosition = new Vector3(target.position.x, target.position.y + targetHeight, target.position.z);

        // if there was a collision, correct the camera position and calculate the corrected distance
        bool isCorrected = false;
        if (Physics.Linecast(trueTargetPosition, position, out collisionHit, collisionLayers.value))
        {
            // calculate the distance from the original estimated position to the collision location,
            // subtracting out a safety "offset" distance from the object we hit.  The offset will help
            // keep the camera from being right on top of the surface we hit, which usually shows up as
            // the surface geometry getting partially clipped by the camera's front clipping plane.
            correctedDistance = Vector3.Distance(trueTargetPosition, collisionHit.point) - offsetFromWall;
            isCorrected = true;
        }

        // For smoothing, lerp distance only if either distance wasn't corrected, or correctedDistance is more than currentDistance
        currentDistance = !isCorrected || correctedDistance > currentDistance ? Mathf.Lerp(currentDistance, correctedDistance, Time.deltaTime * zoomDampening) : correctedDistance;

        // keep within legal limits
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        // recalculate position based on the new currentDistance
        position = target.position - (rotation * Vector3.forward * currentDistance + vTargetOffset);

        transform.rotation = rotation;
        transform.position = position;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}