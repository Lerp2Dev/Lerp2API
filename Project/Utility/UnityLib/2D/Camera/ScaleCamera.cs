using UnityEngine;

// pixel perfect camera helpers, from old unity 2D tutorial videos

/// <summary>
/// Class ScaleCamera.
/// </summary>
[ExecuteInEditMode]
public class ScaleCamera : MonoBehaviour
{
    /// <summary>
    /// The target width
    /// </summary>
    public int targetWidth = 640;

    /// <summary>
    /// The pixels to units
    /// </summary>
    public float pixelsToUnits = 100;

    private void Start()
    {
        int height = Mathf.RoundToInt(targetWidth / (float)Screen.width * Screen.height);
        GetComponent<Camera>().orthographicSize = height / pixelsToUnits / 2;
    }
}