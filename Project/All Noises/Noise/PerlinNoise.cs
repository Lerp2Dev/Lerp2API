/// <summary>
/// Class PerlinNoise.
/// </summary>
public class PerlinNoise : NoiseModule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PerlinNoise"/> class.
    /// </summary>
    /// <param name="seed">The seed.</param>
    public PerlinNoise(int seed) : base(seed)
    {
    }

    /// <summary>
    /// Fractals the noise1 d.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="octNum">The oct number.</param>
    /// <param name="frq">The FRQ.</param>
    /// <param name="amp">The amp.</param>
    /// <returns>System.Single.</returns>
    public float FractalNoise1D(float x, int octNum, float frq, float amp)
    {
        float gain = 1.0f;
        float sum = 0.0f;

        for (int i = 0; i < octNum; i++)
        {
            sum += Noise1D(x * gain / frq) * amp / gain;
            gain *= 2.0f;
        }
        return sum;
    }

    /// <summary>
    /// Fractals the noise2 d.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="octNum">The oct number.</param>
    /// <param name="frq">The FRQ.</param>
    /// <param name="amp">The amp.</param>
    /// <returns>System.Single.</returns>
    public override float FractalNoise2D(float x, float y, int octNum, float frq, float amp)
    {
        float gain = 1.0f;
        float sum = 0.0f;

        for (int i = 0; i < octNum; i++)
        {
            sum += Noise2D(x * gain / frq, y * gain / frq) * amp / gain;
            gain *= 2.0f;
        }
        return sum;
    }

    /// <summary>
    /// Fractals the noise3 d.
    /// </summary>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="z">The z.</param>
    /// <param name="octNum">The oct number.</param>
    /// <param name="frq">The FRQ.</param>
    /// <param name="amp">The amp.</param>
    /// <returns>System.Single.</returns>
    public float FractalNoise3D(float x, float y, float z, int octNum, float frq, float amp)
    {
        float gain = 1.0f;
        float sum = 0.0f;

        for (int i = 0; i < octNum; i++)
        {
            sum += Noise3D(x * gain / frq, y * gain / frq, z * gain / frq) * amp / gain;
            gain *= 2.0f;
        }
        return sum;
    }
}