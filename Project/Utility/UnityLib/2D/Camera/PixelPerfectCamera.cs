using UnityEngine;

// pixel perfect camera helpers, from old unity 2D tutorial videos

/// <summary>
/// Class PixelPerfectCamera.
/// </summary>
[ExecuteInEditMode]
public class PixelPerfectCamera : MonoBehaviour
{
    /// <summary>
    /// The pixels to units
    /// </summary>
    public float pixelsToUnits = 100;

    private void Start()
    {
        GetComponent<Camera>().orthographicSize = Screen.height / pixelsToUnits / 2;
    }
}