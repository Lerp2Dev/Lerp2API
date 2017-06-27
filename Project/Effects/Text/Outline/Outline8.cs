using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class Outline8.
/// </summary>
public class Outline8 : ModifiedShadow
{
    /// <summary>
    /// Modifies the vertices.
    /// </summary>
    /// <param name="verts">The verts.</param>
    public override void ModifyVertices(List<UIVertex> verts)
    {
        if (!IsActive())
            return;

        var neededCapacity = verts.Count * 9;
        if (verts.Capacity < neededCapacity)
            verts.Capacity = neededCapacity;

        var original = verts.Count;
        var count = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (!(x == 0 && y == 0))
                {
                    var next = count + original;
                    ApplyShadow(verts, effectColor, count, next, effectDistance.x * x, effectDistance.y * y);
                    count = next;
                }
            }
        }
    }
}