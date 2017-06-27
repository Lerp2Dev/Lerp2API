using UnityEngine;

/// <summary>
/// Class RidgedNoise.
/// </summary>
public class RidgedNoise : NoiseModule
{
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
        float signal = 0.0f;
        float value = 0.0f;
        float weight = 1.0f;
        float offset = 1.0f;

        for (int i = 0; i < octNum; i++)
        {
            signal = Noise2D(x * gain / frq, y * gain / frq) / gain;
            signal = Mathf.Abs(signal);
            signal = offset - signal;
            signal *= signal;
            signal *= weight;
            weight = signal * gain;
            if (weight > 1.0) { weight = 1.0f; }
            if (weight < 0.0) { weight = 0.0f; }

            value += (signal * m_weights[i]);
            x *= m_lacunarity;
            y *= m_lacunarity;
        }
        return ((value * 1.25f) - 1.0f) * amp;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RidgedNoise"/> class.
    /// </summary>
    /// <param name="seed">The seed.</param>
    public RidgedNoise(int seed) : base(seed)
    {
        UpdateWeights();
    }

    private float[] m_weights = new float[NoiseUtil.OctavesMaximum];
    private float m_lacunarity = 2.0f;

    private void UpdateWeights()
    {
        float f = 1.0f;
        for (int i = 0; i < NoiseUtil.OctavesMaximum; i++)
        {
            m_weights[i] = Mathf.Pow(f, -1.0f);
            f *= m_lacunarity;
        }
    }
}