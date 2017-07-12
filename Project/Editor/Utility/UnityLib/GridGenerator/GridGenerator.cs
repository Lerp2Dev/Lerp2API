using UnityEngine;

/// <summary>
/// Class GridGenerator.
/// </summary>
public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToPlace;

    [SerializeField]
    private uint sizeX = 2;

    [SerializeField]
    private uint sizeY = 2;

    [SerializeField]
    private Vector2 offset = Vector2.one;

    /// <summary>
    /// Gets the prefab to place.
    /// </summary>
    /// <value>The prefab to place.</value>
    public GameObject PrefabToPlace { get { return prefabToPlace; } }

    /// <summary>
    /// Gets the offset.
    /// </summary>
    /// <value>The offset.</value>
    public Vector2 Offset { get { return offset; } }

    /// <summary>
    /// Gets the size x.
    /// </summary>
    /// <value>The size x.</value>
    public uint SizeX { get { return sizeX; } }

    /// <summary>
    /// Gets the size y.
    /// </summary>
    /// <value>The size y.</value>
    public uint SizeY { get { return sizeY; } }
}