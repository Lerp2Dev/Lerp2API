using UnityEngine;
using System.Collections;

// Draws crosshair to mouseposition with GL.Lines, takes line length in pixels
// Usage: Attach this script to Main Camera, Assign material (for example some particle shader)

/// <summary>
/// Class DrawCrossHair.
/// </summary>
public class DrawCrossHair : MonoBehaviour
{
    /// <summary>
    /// The mat
    /// </summary>
    public Material mat;

    /// <summary>
    /// The line length
    /// </summary>
    public float lineLen = 5f; // in pixels

    private Vector3 mousePos;
    private float lineLenHorizontal;
    private float lineLenVertical;

    private void Awake()
    {
        // if you want to adjust lineLen at runtime, would need to calculate these again
        lineLenHorizontal = lineLen / Screen.width;
        lineLenVertical = lineLen / Screen.height;
    }

    private void Update()
    {
        mousePos = Input.mousePosition;
        mousePos.x /= Screen.width;
        mousePos.y /= Screen.height;
    }

    private void OnPostRender()
    {
        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(Color.white);
        GL.Vertex(new Vector3(mousePos.x, mousePos.y - lineLenVertical, 0));
        GL.Vertex(new Vector3(mousePos.x, mousePos.y + lineLenVertical, 0));
        GL.Vertex(new Vector3(mousePos.x - lineLenHorizontal, mousePos.y, 0));
        GL.Vertex(new Vector3(mousePos.x + lineLenHorizontal, mousePos.y, 0));
        GL.End();
        GL.PopMatrix();
    }
}