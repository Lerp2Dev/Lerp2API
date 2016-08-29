using UnityEngine;

public class cVoronoiMap
{
    /// The noise used for generating Voronoi seeds
    protected cNoise m_Noise1;

    protected cNoise m_Noise2;
    protected cNoise m_Noise3;

    /** Size of the Voronoi cells (avg X/Y distance between the seeds). Expected to be at least 2. */
    protected int m_CellSize;

    /** The amount that the cell seeds may be offset from the grid.
	Expected to be at least 1 and less than m_CellSize. */
    protected int m_JitterSize;

    /** The constant amount that the cell seeds of every odd row will be offset from the grid.
	This allows us to have non-rectangular grids.
	Expected to be between -m_CellSize and +m_CellSize. */
    protected int m_OddRowOffset;

    /** The X coordinate of the currently cached cell neighborhood */
    protected int m_CurrentCellX;

    /** The Z coordinate of the currently cached cell neighborhood */
    protected int m_CurrentCellZ;

    /** The seeds of cells around m_CurrentCellX, m_CurrentCellZ, X-coords */
    protected int[,] m_SeedX = new int[5, 5];

    /** The seeds of cells around m_CurrentCellX, m_CurrentCellZ, X-coords */
    protected int[,] m_SeedZ = new int[5, 5];

    public cVoronoiMap(int a_Seed, int a_CellSize) : this(a_Seed, a_CellSize, 128)
    {
    }

    public cVoronoiMap(int a_Seed) : this(a_Seed, 128, 128)
    {
    }

    public cVoronoiMap(int a_Seed, int a_CellSize, int a_JitterSize)
    {
        m_Noise1 = new cNoise(a_Seed + 1);
        m_Noise2 = new cNoise(a_Seed + 2);
        m_Noise3 = new cNoise(a_Seed + 3);
        m_CellSize = Mathf.Max(a_CellSize, 2);
        m_JitterSize = Mathf.Clamp(a_JitterSize, 1, a_CellSize);
        m_OddRowOffset = 0;
        m_CurrentCellX = 9999999;
        m_CurrentCellZ = 9999999;
    }

    /** Sets both the cell size and jitter size used for generating the Voronoi seeds. */

    public void SetCellSize(int a_CellSize)
    {
        a_CellSize = Mathf.Max(a_CellSize, 2); // Cell size must be at least 2
        m_CellSize = a_CellSize;

        // For compatibility with previous version, which didn't have the jitter, we set jitter here as well.
        m_JitterSize = a_CellSize;
    }

    /** Sets the jitter size. Clamps it to current cell size. */

    public void SetJitterSize(int a_JitterSize)
    {
        m_JitterSize = Mathf.Clamp(a_JitterSize, 1, m_CellSize);
    }

    /** Sets the offset that is added to each odd row of cells.
	This offset makes the voronoi cells align to a non-grid.
	Clamps the value to [-m_CellSize, +m_CellSize]. */

    public void SetOddRowOffset(int a_OddRowOffset)
    {
        m_OddRowOffset = Mathf.Clamp(a_OddRowOffset, -m_CellSize, m_CellSize);
    }

    /** Returns the value in the cell into which the specified point lies. */

    public int GetValueAt(int a_X, int a_Y)
    {
        int SeedX = 0, SeedY = 0, MinDist2 = 0;
        return GetValueAt(a_X, a_Y, ref SeedX, ref SeedY, ref MinDist2);
    }

    /** Returns the value in the cell into which the specified point lies,
	and the distance to the nearest Voronoi seed. */

    public int GetValueAt(int a_X, int a_Y, ref int a_MinDist)
    {
        int SeedX = 0, SeedY = 0, MinDist2 = 0, res = GetValueAt(a_X, a_Y, ref SeedX, ref SeedY, ref MinDist2);
        a_MinDist = (a_X - SeedX) * (a_X - SeedX) + (a_Y - SeedY) * (a_Y - SeedY);
        return res;
    }

    /** Returns the value in the cell into which the specified point lies,
	and the distances to the 2 nearest Voronoi seeds. Uses a cache. */

    public int GetValueAt(int a_X, int a_Y, ref int a_NearestSeedX, ref int a_NearestSeedY, ref int a_MinDist2)
    {
        int CellX = a_X / m_CellSize,
            CellY = a_Y / m_CellSize;

        UpdateCell(CellX, CellY);

        // Get 5x5 neighboring cell seeds, compare distance to each. Return the value in the minumim-distance cell
        int NearestSeedX = 0,
            NearestSeedY = 0,
            MinDist = m_CellSize * m_CellSize * 16, // There has to be a cell closer than this
            MinDist2 = MinDist,
            res = 0; // Will be overriden
        for (int x = 0; x < 5; ++x)
            for (int y = 0; y < 5; ++y)
            {
                int SeedX = m_SeedX[x, y],
                    SeedY = m_SeedZ[x, y];

                int Dist = (SeedX - a_X) * (SeedX - a_X) + (SeedY - a_Y) * (SeedY - a_Y);
                if (Dist < MinDist)
                {
                    NearestSeedX = SeedX;
                    NearestSeedY = SeedY;
                    MinDist2 = MinDist;
                    MinDist = Dist;
                    res = m_Noise3.IntNoise2DInt(x + CellX - 2, y + CellY - 2);
                }
                else if (Dist < MinDist2)
                    MinDist2 = Dist;
            }
        a_NearestSeedX = NearestSeedX;
        a_NearestSeedY = NearestSeedY;
        a_MinDist2 = MinDist2;
        return res;
    }

    /** Finds the nearest and second nearest seeds, returns their coords. */

    public void FindNearestSeeds(int a_X, int a_Y, ref int a_NearestSeedX, ref int a_NearestSeedY, ref int a_SecondNearestSeedX, ref int a_SecondNearestSeedY)
    {
        int CellX = a_X / m_CellSize,
            CellY = a_Y / m_CellSize;

        UpdateCell(CellX, CellY);

        // Get 5x5 neighboring cell seeds, compare distance to each. Return the value in the minumim-distance cell
        int NearestSeedX = 0,
            NearestSeedY = 0,
            SecondNearestSeedX = 0,
            SecondNearestSeedY = 0,
            MinDist = m_CellSize * m_CellSize * 16, // There has to be a cell closer than this
            MinDist2 = MinDist;
        for (int x = 0; x < 5; ++x)
            for (int y = 0; y < 5; ++y)
            {
                int SeedX = m_SeedX[x, y];
                int SeedY = m_SeedZ[x, y];

                int Dist = (SeedX - a_X) * (SeedX - a_X) + (SeedY - a_Y) * (SeedY - a_Y);
                if (Dist < MinDist)
                {
                    SecondNearestSeedX = NearestSeedX;
                    SecondNearestSeedY = NearestSeedY;
                    MinDist2 = MinDist;
                    NearestSeedX = SeedX;
                    NearestSeedY = SeedY;
                    MinDist = Dist;
                }
                else if (Dist < MinDist2)
                {
                    SecondNearestSeedX = SeedX;
                    SecondNearestSeedY = SeedY;
                    MinDist2 = Dist;
                }
            }
        a_NearestSeedX = NearestSeedX;
        a_NearestSeedY = NearestSeedY;
        a_SecondNearestSeedX = SecondNearestSeedX;
        a_SecondNearestSeedY = SecondNearestSeedY;
    }

    /** Updates the cached cell seeds to match the specified cell. Noop if cell pos already matches.
	Updates m_SeedX and m_SeedZ. */

    protected void UpdateCell(int a_CellX, int a_CellZ)
    {
        // If the specified cell is currently cached, bail out:
        if (a_CellX == m_CurrentCellX && a_CellZ == m_CurrentCellZ)
            return;

        // Update the cell cache for the new cell position:
        int NoiseBaseX = a_CellX - 2,
            NoiseBaseZ = a_CellZ - 2;
        for (int x = 0; x < 5; x++)
        {
            int BaseX = (NoiseBaseX + x) * m_CellSize;
            int OddRowOffset = ((NoiseBaseX + x) & 0x01) * m_OddRowOffset;
            for (int z = 0; z < 5; z++)
            {
                int OffsetX = (m_Noise1.IntNoise2DInt(NoiseBaseX + x, NoiseBaseZ + z) / 8) % m_JitterSize;
                int OffsetZ = (m_Noise2.IntNoise2DInt(NoiseBaseX + x, NoiseBaseZ + z) / 8) % m_JitterSize;
                m_SeedX[x, z] = BaseX + OffsetX;
                m_SeedZ[x, z] = (NoiseBaseZ + z) * m_CellSize + OddRowOffset + OffsetZ;
            }
        }
        m_CurrentCellX = a_CellX;
        m_CurrentCellZ = a_CellZ;
    }
}//218