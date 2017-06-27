using UnityEngine;

namespace Lerp2API.Hepers.Unity_Extensions
{
    /// <summary>
    /// Class UnityColorHelpers.
    /// </summary>
    public static class ColorHelpers
    {
        /// <summary>
        /// Inverts the color.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>Color.</returns>
        public static Color InvertColor(this Color c)
        {
            return new Color(1f - c.r, 1f - c.g, 1f - c.b);
        }
    }
}