using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class BoxOutline.
/// </summary>
public class BoxOutline : ModifiedShadow
{
    private const int maxHalfSampleCount = 20;

    [SerializeField]
    [Range(1, maxHalfSampleCount)]
    private int m_halfSampleCountX = 1;

    [SerializeField]
    [Range(1, maxHalfSampleCount)]
    private int m_halfSampleCountY = 1;

    /// <summary>
    /// Gets or sets the half sample count x.
    /// </summary>
    /// <value>The half sample count x.</value>
    public int halfSampleCountX
    {
        get
        {
            return m_halfSampleCountX;
        }

        set
        {
            m_halfSampleCountX = Mathf.Clamp(value, 1, maxHalfSampleCount);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    /// <summary>
    /// Gets or sets the half sample count y.
    /// </summary>
    /// <value>The half sample count y.</value>
    public int halfSampleCountY
    {
        get
        {
            return m_halfSampleCountY;
        }

        set
        {
            m_halfSampleCountY = Mathf.Clamp(value, 1, maxHalfSampleCount);
            if (graphic != null)
                graphic.SetVerticesDirty();
        }
    }

    /// <summary>
    /// Modifies the vertices.
    /// </summary>
    /// <param name="verts">The verts.</param>
    public override void ModifyVertices(List<UIVertex> verts)
    {
        if (!IsActive())
            return;

        var neededCapacity = verts.Count * (m_halfSampleCountX * 2 + 1) * (m_halfSampleCountY * 2 + 1);
        if (verts.Capacity < neededCapacity)
            verts.Capacity = neededCapacity;

        var original = verts.Count;
        var count = 0;
        var dx = effectDistance.x / m_halfSampleCountX;
        var dy = effectDistance.y / m_halfSampleCountY;
        for (int x = -m_halfSampleCountX; x <= m_halfSampleCountX; x++)
        {
            for (int y = -m_halfSampleCountY; y <= m_halfSampleCountY; y++)
            {
                if (!(x == 0 && y == 0))
                {
                    var next = count + original;
                    ApplyShadow(verts, effectColor, count, next, dx * x, dy * y);
                    count = next;
                }
            }
        }
    }
}