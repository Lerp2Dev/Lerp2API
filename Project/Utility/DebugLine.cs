/* Author: Spencer Evans
 *
 * Description:
 * DebugLine.cs provides 2 useful functions for debugging : DrawLine and DrawRay. These functions are almost identical to those provided in the Unity 3D
 * engine Debug class (http://docs.unity3d.com/Documentation/ScriptReference/Debug.html), except in the following ways:
 * 1. The lines are drawn in the Game window, not the Scene editor window.
 * 2. A "width" option is provided - allowing the user to choose the width in pixels of the line drawn.
 * 3. The "depthTest" parameter in the Debug.DrawLine() and Debug.DrawRay() functions is unavailable; all lines are obscured by objects closer to the camera.
 *
 * Usage:
 * 1. Create a new folder titled "Plugins", and move it to your project's "/Assets" folder (if you haven't already).
 * 2. Create and place "DebugLine.cs" inside the "/Plugins" folder.
 * 3. Call the DebugLine.DrawLine() or DebugLine.DrawRay() functions just as you would with the Debug class.
 *
 * Notes:
 * 1. You must have a Camera in your scene tagged "MainCamera" for these debug functions to work properly.
 * 2. You do NOT need Unity Pro for this to work.
 */

using UnityEngine;

/// <summary>
/// Class DebugLine.
/// </summary>
public class DebugLine : MonoBehaviour
{
    //used to make calls from both Update and FixedUpdate function properly
    /// <summary>
    /// The destroy time
    /// </summary>
    public float destroy_time = float.MaxValue;

    /// <summary>
    /// The fixed destroy time
    /// </summary>
    public float fixed_destroy_time = float.MaxValue;

    //draw ray functions - http://docs.unity3d.com/Documentation/ScriptReference/Debug.DrawRay.html
    /// <summary>
    /// Draws the ray.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="dir">The dir.</param>
    public static void DrawRay(Vector3 start, Vector3 dir)
    {
        DrawLine(start, start + dir, Color.white);
    }

    /// <summary>
    /// Draws the ray.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="dir">The dir.</param>
    /// <param name="color">The color.</param>
    /// <param name="duration">The duration.</param>
    /// <param name="width">The width.</param>
    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration = 0, float width = 1)
    {
        DrawLine(start, start + dir, color, duration, width);
    }

    //draw line functions - http://docs.unity3d.com/Documentation/ScriptReference/Debug.DrawLine.html
    /// <summary>
    /// Draws the line.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    public static void DrawLine(Vector3 start, Vector3 end)
    {
        DrawLine(start, end, Color.white);
    }

    /// <summary>
    /// Draws the line.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    /// <param name="color">The color.</param>
    /// <param name="duration">The duration.</param>
    /// <param name="width">The width.</param>
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0, float width = 1)
    {
        //early out if there is no Camera.main to calculate the width
        if (!Camera.main)
            return;

        GameObject line = new GameObject("debug_line");

        //set the params of the line renderer - http://docs.unity3d.com/Documentation/ScriptReference/LineRenderer.html
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive")); //Outlined/Silhouetted Diffuse //new Material(Shader.Find("Mobile/Particles/Additive"));
        lineRenderer.material.SetColor("_OutlineColor", new Color(color.r, color.g, color.b, 0.5f));
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = CalcPixelHeightAtDist((start - Camera.main.transform.position).magnitude) * width;
        lineRenderer.endWidth = CalcPixelHeightAtDist((end - Camera.main.transform.position).magnitude) * width;
        /*lineRenderer.SetColors(color, color); //I have to arreglar esto
        lineRenderer.SetWidth(CalcPixelHeightAtDist((start - Camera.main.transform.position).magnitude) * width,
                               CalcPixelHeightAtDist((end - Camera.main.transform.position).magnitude) * width);*/

        //add the MonoBehaviour instance
        DebugLine debugLine = line.AddComponent<DebugLine>();

        //set the time to expire - default is 0 (will only draw for one update cycle)
        //use Update or FixedUpdate depending on the time of the invoking call
        if (Time.deltaTime == Time.fixedDeltaTime)
            debugLine.fixed_destroy_time = Time.fixedTime + duration;
        else debugLine.destroy_time = Time.time + duration;
    }

    //utility function to calculate the world height of a single pixel given the following info:
    // - Camera.main.fov
    // - Camera.main.pixelHeight
    // - distance to the camera
    /// <summary>
    /// Calculates the pixel height at dist.
    /// </summary>
    /// <param name="dist">The dist.</param>
    /// <returns>System.Single.</returns>
    public static float CalcPixelHeightAtDist(float dist)
    {
        //early out if there is no Camera.main to calculate the width
        if (!Camera.main)
            return 0;

        //http://docs.unity3d.com/Documentation/Manual/FrustumSizeAtDistance.html
        float frustumHeight = 2 * dist * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);

        return frustumHeight / Camera.main.pixelHeight;
    }

    //check to see if should be destroyed yet
    /// <summary>
    /// Updates this instance.
    /// </summary>
    public void Update()
    {
        if (Time.time > destroy_time)
            Destroy(transform.gameObject);
    }

    /// <summary>
    /// Fixeds the update.
    /// </summary>
    public void FixedUpdate()
    {
        if (Time.fixedTime > fixed_destroy_time)
            Destroy(transform.gameObject);
    }
}