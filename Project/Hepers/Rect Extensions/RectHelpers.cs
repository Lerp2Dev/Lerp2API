using UnityEngine;

namespace Lerp2API.Hepers.Rect_Utils
{
    /// <summary>
    /// Class RectHelpers.
    /// </summary>
    public static class RectHelpers
    {
        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="w">The w.</param>
        /// <param name="h">The h.</param>
        /// <param name="mh">The mh.</param>
        /// <param name="mv">The mv.</param>
        /// <returns>Rect.</returns>
        public static Rect GetPosition(this Position pos, int w, int h, int mh = 5, int mv = 5)
        {
            switch (pos)
            {
                case Position.UpperLeft:
                    return new Rect(mh, mv, w, h);

                case Position.UpperRight:
                    return new Rect(Screen.width - w - mh, mv, w, h);

                case Position.BottomLeft:
                    return new Rect(mh, Screen.height - h - mv, w, h);

                case Position.BottomRight:
                    return new Rect(Screen.width - w - mh, Screen.height - h - mv, w, h);

                default:
                    return new Rect(mh, mv, w, h);
            }
        }
    }
}