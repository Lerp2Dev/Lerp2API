using UnityEngine;

// Zoom forward and backward with mousewheel, Attach this script to camera

/// <summary>
/// Class ZoomWithMouse.
/// </summary>
public class ZoomWithMouse : MonoBehaviour
{
    /// <summary>
    /// The zoom speed
    /// </summary>
    public float zoomSpeed = 20;

    private void Update()
    {
        var mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        if (mouseScroll != 0)
        {
            transform.Translate(transform.forward * mouseScroll * zoomSpeed * Time.deltaTime, Space.Self);
        }
    }
}