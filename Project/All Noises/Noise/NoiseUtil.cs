/// <summary>
/// Class NoiseUtil.
/// </summary>
public class NoiseUtil
{
    /// <summary>
    /// The octaves maximum
    /// </summary>
    public static int OctavesMaximum = 16;

    /// <summary>
    /// Fades the specified t.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <returns>System.Single.</returns>
    public static float FADE(float t)
    {
        return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
    }

    /// <summary>
    /// Lerps the specified t.
    /// </summary>
    /// <param name="t">The t.</param>
    /// <param name="a">a.</param>
    /// <param name="b">The b.</param>
    /// <returns>System.Single.</returns>
    public static float LERP(float t, float a, float b)
    {
        return (a) + (t) * ((b) - (a));
    }

    /// <summary>
    /// Gras the d1.
    /// </summary>
    /// <param name="hash">The hash.</param>
    /// <param name="x">The x.</param>
    /// <returns>System.Single.</returns>
    public static float GRAD1(int hash, float x)
    {
        //This method uses the mod operator which is slower
        //than bitwise operations but is included out of interest
        //		int h = hash % 16;
        //		float grad = 1.0f + (h % 8);
        //		if((h%8) < 4) grad = -grad;
        //		return ( grad * x );

        int h = hash & 15;
        float grad = 1.0f + (h & 7);
        if ((h & 8) != 0) grad = -grad;
        return (grad * x);
    }

    /// <summary>
    /// Gras the d2.
    /// </summary>
    /// <param name="hash">The hash.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <returns>System.Single.</returns>
    public static float GRAD2(int hash, float x, float y)
    {
        //This method uses the mod operator which is slower
        //than bitwise operations but is included out of interest
        //		int h = hash % 16;
        //    	float u = h<4 ? x : y;
        //    	float v = h<4 ? y : x;
        //		int hn = h%2;
        //		int hm = (h/2)%2;
        //    	return ((hn != 0) ? -u : u) + ((hm != 0) ? -2.0f*v : 2.0f*v);

        int h = hash & 7;
        float u = h < 4 ? x : y;
        float v = h < 4 ? y : x;
        return (((h & 1) != 0) ? -u : u) + (((h & 2) != 0) ? -2.0f * v : 2.0f * v);
    }

    /// <summary>
    /// Gras the d3.
    /// </summary>
    /// <param name="hash">The hash.</param>
    /// <param name="x">The x.</param>
    /// <param name="y">The y.</param>
    /// <param name="z">The z.</param>
    /// <returns>System.Single.</returns>
    public static float GRAD3(int hash, float x, float y, float z)
    {
        //This method uses the mod operator which is slower
        //than bitwise operations but is included out of interest
        //    	int h = hash % 16;
        //    	float u = (h<8) ? x : y;
        //    	float v = (h<4) ? y : (h==12||h==14) ? x : z;
        //		int hn = h%2;
        //		int hm = (h/2)%2;
        //    	return ((hn != 0) ? -u : u) + ((hm != 0) ? -v : v);

        int h = hash & 15;
        float u = h < 8 ? x : y;
        float v = (h < 4) ? y : (h == 12 || h == 14) ? x : z;
        return (((h & 1) != 0) ? -u : u) + (((h & 2) != 0) ? -v : v);
    }
}