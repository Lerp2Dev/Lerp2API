using UnityEngine;
using System.Collections;

// Usage:
// - assign texture (that has [x] read/write enabled in inspector
// - assign Quad mesh (prefab) to planePrefab. You can assign Unlit/Color material to the quad prefab first.

/// <summary>
/// Class TexturePixels2Quads.
/// </summary>
public class TexturePixels2Quads : MonoBehaviour
{
    /// <summary>
    /// The tex
    /// </summary>
    public Texture2D tex;

    /// <summary>
    /// The plane prefab
    /// </summary>
    public Renderer planePrefab;

    private void Start()
    {
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                var c = tex.GetPixel(x, y);
                var pos = new Vector3(x, y, 0);
                var plane = Instantiate(planePrefab, pos, Quaternion.identity) as Renderer;
                plane.material.color = c;
            }
        }
    }
}