using System.Linq;
using UnityEngine;

namespace RadicalLibrary
{
    /// <summary>
    /// Class QuadBez.
    /// </summary>
    public static class QuadBez
    {
        /// <summary>
        /// Interps the specified st.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="en">The en.</param>
        /// <param name="ctrl">The control.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Interp(Vector3 st, Vector3 en, Vector3 ctrl, float t)
        {
            float d = 1f - t;
            return d * d * st + 2f * d * t * ctrl + t * t * en;
        }

        /// <summary>
        /// Velocities the specified st.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="en">The en.</param>
        /// <param name="ctrl">The control.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Velocity(Vector3 st, Vector3 en, Vector3 ctrl, float t)
        {
            return (2f * st - 4f * ctrl + 2f * en) * t + 2f * ctrl - 2f * st;
        }

        /// <summary>
        /// Gizmoes the draw.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="en">The en.</param>
        /// <param name="ctrl">The control.</param>
        /// <param name="t">The t.</param>
        public static void GizmoDraw(Vector3 st, Vector3 en, Vector3 ctrl, float t)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(st, ctrl);
            Gizmos.DrawLine(ctrl, en);

            Gizmos.color = Color.white;
            Vector3 prevPt = st;

            for (int i = 1; i <= 20; i++)
            {
                float pm = (float)i / 20f;
                Vector3 currPt = Interp(st, en, ctrl, pm);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }

            Gizmos.color = Color.blue;
            Vector3 pos = Interp(st, en, ctrl, t);
            Gizmos.DrawLine(pos, pos + Velocity(st, en, ctrl, t));
        }
    }

    /// <summary>
    /// Class CubicBez.
    /// </summary>
    public static class CubicBez
    {
        /// <summary>
        /// Interps the specified st.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="en">The en.</param>
        /// <param name="ctrl1">The CTRL1.</param>
        /// <param name="ctrl2">The CTRL2.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Interp(Vector3 st, Vector3 en, Vector3 ctrl1, Vector3 ctrl2, float t)
        {
            float d = 1f - t;
            return d * d * d * st + 3f * d * d * t * ctrl1 + 3f * d * t * t * ctrl2 + t * t * t * en;
        }

        /// <summary>
        /// Velocities the specified st.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="en">The en.</param>
        /// <param name="ctrl1">The CTRL1.</param>
        /// <param name="ctrl2">The CTRL2.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Velocity(Vector3 st, Vector3 en, Vector3 ctrl1, Vector3 ctrl2, float t)
        {
            return (-3f * st + 9f * ctrl1 - 9f * ctrl2 + 3f * en) * t * t
                + (6f * st - 12f * ctrl1 + 6f * ctrl2) * t
                - 3f * st + 3f * ctrl1;
        }

        /// <summary>
        /// Gizmoes the draw.
        /// </summary>
        /// <param name="st">The st.</param>
        /// <param name="en">The en.</param>
        /// <param name="ctrl1">The CTRL1.</param>
        /// <param name="ctrl2">The CTRL2.</param>
        /// <param name="t">The t.</param>
        public static void GizmoDraw(Vector3 st, Vector3 en, Vector3 ctrl1, Vector3 ctrl2, float t)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(st, ctrl1);
            Gizmos.DrawLine(ctrl2, en);

            Gizmos.color = Color.white;
            Vector3 prevPt = st;

            for (int i = 1; i <= 20; i++)
            {
                float pm = (float)i / 20f;
                Vector3 currPt = Interp(st, en, ctrl1, ctrl2, pm);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }

            Gizmos.color = Color.blue;
            Vector3 pos = Interp(st, en, ctrl1, ctrl2, t);
            Gizmos.DrawLine(pos, pos + Velocity(st, en, ctrl1, ctrl2, t));
        }
    }

    /// <summary>
    /// Class CRSpline.
    /// </summary>
    public static class CRSpline
    {
        /// <summary>
        /// Interps the specified PTS.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Interp(Vector3[] pts, float t)
        {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt;

            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];

            return .5f * (
                (-a + 3f * b - 3f * c + d) * (u * u * u)
                + (2f * a - 5f * b + 4f * c - d) * (u * u)
                + (-a + c) * u
                + 2f * b
            );
        }

        /// <summary>
        /// Interps the constant speed old.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 InterpConstantSpeedOld(Vector3[] pts, float t)
        {
            int numSections = pts.Length - 3;
            float mag = 0;
            float[] sizes = new float[pts.Length - 1];
            for (var i = 0; i < pts.Length - 1; i++)
            {
                var m = (pts[i + 1] - pts[i]).magnitude;
                sizes[i] = m;
                mag += m;
            }

            int currPt = 1;
            float s = 0;
            double u = 0;
            do
            {
                double tAtBeginning = s / mag;
                double tAtEnd = (s + sizes[currPt + 0]) / mag;
                u = (t - tAtBeginning) / (tAtEnd - tAtBeginning);
                if (double.IsNaN(u) || u < 0 || u > 1)
                {
                    s += sizes[currPt];
                    currPt++;
                }
                else
                    break;
            } while (currPt < numSections + 1);
            u = Mathf.Clamp01((float)u);

            Vector3 a = pts[currPt - 1];
            Vector3 b = pts[currPt + 0];
            Vector3 c = pts[currPt + 1];
            Vector3 d = pts[currPt + 2];

            //return CubicBez.Interp(a,d,b,c,(float)u);
            return .5f * (
                (-a + 3f * b - 3f * c + d) * (float)(u * u * u)
                + (2f * a - 5f * b + 4f * c - d) * (float)(u * u)
                + (-a + c) * (float)u
                + 2f * b
            );
        }

        /// <summary>
        /// Interps the constant speed.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 InterpConstantSpeed(Vector3[] pts, float t)
        {
            int numSections = pts.Length - 3;
            float mag = 0;
            float[] sizes = new float[pts.Length - 1];
            for (var i = 0; i < pts.Length - 1; i++)
            {
                var m = (pts[i + 1] - pts[i]).magnitude;
                sizes[i] = m;
                mag += m;
            }

            int currPt = 1;
            float s = 0;
            double u = 0;
            do
            {
                double tAtBeginning = s / mag;
                double tAtEnd = (s + sizes[currPt + 0]) / mag;
                u = (t - tAtBeginning) / (tAtEnd - tAtBeginning);
                if (double.IsNaN(u) || u < 0 || u > 1)
                {
                    s += sizes[currPt];
                    currPt++;
                }
                else
                    break;
            } while (currPt < numSections + 1);
            u = Mathf.Clamp01((float)u);

            Vector3 a = pts[currPt - 1];
            Vector3 b = pts[currPt + 0];
            Vector3 c = pts[currPt + 1];
            Vector3 d = pts[currPt + 2];

            //return CubicBez.Interp(a,d,b,c,(float)u);
            return .5f * (
                (-a + 3f * b - 3f * c + d) * (float)(u * u * u)
                + (2f * a - 5f * b + 4f * c - d) * (float)(u * u)
                + (-a + c) * (float)u
                + 2f * b
            );
        }

        /// <summary>
        /// Velocities the specified PTS.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Velocity(Vector3[] pts, float t)
        {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt;

            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];

            return 1.5f * (-a + 3f * b - 3f * c + d) * (u * u)
                    + (2f * a - 5f * b + 4f * c - d) * u
                    + .5f * c - .5f * a;
        }

        /// <summary>
        /// Gizmoes the draw.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        public static void GizmoDraw(Vector3[] pts, float t)
        {
            Gizmos.color = Color.white;
            Vector3 prevPt = Interp(pts, 0);

            for (int i = 1; i <= 20; i++)
            {
                float pm = (float)i / 20f;
                Vector3 currPt = Interp(pts, pm);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }

            Gizmos.color = Color.blue;
            Vector3 pos = Interp(pts, t);
            Gizmos.DrawLine(pos, pos + Velocity(pts, t));
        }
    }

    /// <summary>
    /// Class Interesting.
    /// </summary>
    public class Interesting
    {
    }

    /// <summary>
    /// Class Spline.
    /// </summary>
    public static class Spline
    {
        /// <summary>
        /// Interps the specified PTS.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Interp(Path pts, float t)
        {
            return Interp(pts, t, EasingType.Linear);
        }

        /// <summary>
        /// Interps the specified PTS.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Interp(Path pts, float t, EasingType ease)
        {
            return Interp(pts, t, ease, true);
        }

        /// <summary>
        /// Interps the specified PTS.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Interp(Path pts, float t, EasingType ease, bool easeIn)
        {
            return Interp(pts, t, ease, easeIn, true);
        }

        /// <summary>
        /// Interps the specified PTS.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <param name="easeOut">if set to <c>true</c> [ease out].</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Interp(Path pts, float t, EasingType ease, bool easeIn, bool easeOut)
        {
            t = Ease(t, ease, easeIn, easeOut);

            if (pts.Length == 0)
            {
                return Vector3.zero;
            }
            else if (pts.Length == 1)
            {
                return pts[0];
            }
            else if (pts.Length == 2)
            {
                return Vector3.Lerp(pts[0], pts[1], t);
            }
            else if (pts.Length == 3)
            {
                return QuadBez.Interp(pts[0], pts[2], pts[1], t);
            }
            else if (pts.Length == 4)
            {
                return CubicBez.Interp(pts[0], pts[3], pts[1], pts[2], t);
            }
            else
            {
                return CRSpline.Interp(Wrap(pts), t);
            }
        }

        private static float Ease(float t)
        {
            return Ease(t, EasingType.Linear);
        }

        private static float Ease(float t, EasingType ease)
        {
            return Ease(t, ease, true);
        }

        private static float Ease(float t, EasingType ease, bool easeIn)
        {
            return Ease(t, ease, easeIn, true);
        }

        private static float Ease(float t, EasingType ease, bool easeIn, bool easeOut)
        {
            t = Mathf.Clamp01(t);
            if (easeIn && easeOut)
            {
                t = Easing.EaseInOut(t, ease);
            }
            else if (easeIn)
            {
                t = Easing.EaseIn(t, ease);
            }
            else if (easeOut)
            {
                t = Easing.EaseOut(t, ease);
            }
            return t;
        }

        /// <summary>
        /// Interps the constant speed.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 InterpConstantSpeed(Path pts, float t)
        {
            return InterpConstantSpeed(pts, t, EasingType.Linear);
        }

        /// <summary>
        /// Interps the constant speed.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 InterpConstantSpeed(Path pts, float t, EasingType ease)
        {
            return InterpConstantSpeed(pts, t, ease, true);
        }

        /// <summary>
        /// Interps the constant speed.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <returns>Vector3.</returns>
        public static Vector3 InterpConstantSpeed(Path pts, float t, EasingType ease, bool easeIn)
        {
            return InterpConstantSpeed(pts, t, ease, easeIn, true);
        }

        /// <summary>
        /// Interps the constant speed.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <param name="easeOut">if set to <c>true</c> [ease out].</param>
        /// <returns>Vector3.</returns>
        public static Vector3 InterpConstantSpeed(Path pts, float t, EasingType ease, bool easeIn, bool easeOut)
        {
            t = Ease(t, ease, easeIn, easeOut);

            if (pts.Length == 0)
            {
                return Vector3.zero;
            }
            else
            if (pts.Length == 1)
            {
                return pts[0];
            }
            else
            if (pts.Length == 2)
            {
                return Vector3.Lerp(pts[0], pts[1], t);
            }
            else
            if (pts.Length == 3)
            {
                return QuadBez.Interp(pts[0], pts[2], pts[1], t);
            }
            else
            if (pts.Length == 4)
            {
                return CubicBez.Interp(pts[0], pts[3], pts[1], pts[2], t);
            }
            else
            {
                return CRSpline.InterpConstantSpeed(Wrap(pts), t);
            }
        }

        private static float Clamp(float f)
        {
            return Mathf.Clamp01(f);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition)
        {
            return MoveOnPath(pts, currentPosition, ref pathPosition, 1f);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, float maxSpeed)
        {
            return MoveOnPath(pts, currentPosition, ref pathPosition, maxSpeed, 100);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <param name="smoothnessFactor">The smoothness factor.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, float maxSpeed, float smoothnessFactor)
        {
            return MoveOnPath(pts, currentPosition, ref pathPosition, maxSpeed, smoothnessFactor, EasingType.Linear);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <param name="smoothnessFactor">The smoothness factor.</param>
        /// <param name="ease">The ease.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, float maxSpeed, float smoothnessFactor, EasingType ease)
        {
            return MoveOnPath(pts, currentPosition, ref pathPosition, maxSpeed, smoothnessFactor, ease, true);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <param name="smoothnessFactor">The smoothness factor.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, float maxSpeed, float smoothnessFactor, EasingType ease, bool easeIn)
        {
            return MoveOnPath(pts, currentPosition, ref pathPosition, maxSpeed, smoothnessFactor, ease, easeIn, true);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <param name="smoothnessFactor">The smoothness factor.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <param name="easeOut">if set to <c>true</c> [ease out].</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, float maxSpeed, float smoothnessFactor, EasingType ease, bool easeIn, bool easeOut)
        {
            maxSpeed *= Time.deltaTime;
            pathPosition = Clamp(pathPosition);
            var goal = InterpConstantSpeed(pts, pathPosition, ease, easeIn, easeOut);
            float distance;
            while ((distance = (goal - currentPosition).magnitude) <= maxSpeed && pathPosition != 1)
            {
                //currentPosition = goal;
                //maxSpeed -= distance;
                pathPosition = Clamp(pathPosition + 1 / smoothnessFactor);
                goal = InterpConstantSpeed(pts, pathPosition, ease, easeIn, easeOut);
            }
            if (distance != 0)
            {
                currentPosition = Vector3.MoveTowards(currentPosition, goal, maxSpeed);
            }
            return currentPosition;
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation)
        {
            return MoveOnPath(pts, currentPosition, ref pathPosition, ref rotation, 1f);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation, float maxSpeed)
        {
            return MoveOnPath(pts, currentPosition, ref pathPosition, ref rotation, maxSpeed, 100);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <param name="smoothnessFactor">The smoothness factor.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation, float maxSpeed, float smoothnessFactor)
        {
            return MoveOnPath(pts, currentPosition, ref pathPosition, ref rotation, maxSpeed, smoothnessFactor, EasingType.Linear);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <param name="smoothnessFactor">The smoothness factor.</param>
        /// <param name="ease">The ease.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation, float maxSpeed, float smoothnessFactor, EasingType ease)
        {
            return MoveOnPath(pts, currentPosition, ref pathPosition, ref rotation, maxSpeed, smoothnessFactor, ease, true);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <param name="smoothnessFactor">The smoothness factor.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation, float maxSpeed, float smoothnessFactor, EasingType ease, bool easeIn)
        {
            return MoveOnPath(pts, currentPosition, ref pathPosition, ref rotation, maxSpeed, smoothnessFactor, ease, easeIn, true);
        }

        /// <summary>
        /// Moves the on path.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="currentPosition">The current position.</param>
        /// <param name="pathPosition">The path position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <param name="smoothnessFactor">The smoothness factor.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <param name="easeOut">if set to <c>true</c> [ease out].</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MoveOnPath(Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation, float maxSpeed, float smoothnessFactor, EasingType ease, bool easeIn, bool easeOut)
        {
            var result = MoveOnPath(pts, currentPosition, ref pathPosition, maxSpeed, smoothnessFactor, ease, easeIn, easeOut);

            rotation = result.Equals(currentPosition) ? Quaternion.identity : Quaternion.LookRotation(result - currentPosition);
            return result;
        }

        /// <summary>
        /// Rotations the between.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>Quaternion.</returns>
        public static Quaternion RotationBetween(Path pts, float t1, float t2)
        {
            return RotationBetween(pts, t1, t2, EasingType.Linear);
        }

        /// <summary>
        /// Rotations the between.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <param name="ease">The ease.</param>
        /// <returns>Quaternion.</returns>
        public static Quaternion RotationBetween(Path pts, float t1, float t2, EasingType ease)
        {
            return RotationBetween(pts, t1, t2, ease, true);
        }

        /// <summary>
        /// Rotations the between.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <returns>Quaternion.</returns>
        public static Quaternion RotationBetween(Path pts, float t1, float t2, EasingType ease, bool easeIn)
        {
            return RotationBetween(pts, t1, t2, ease, easeIn, true);
        }

        /// <summary>
        /// Rotations the between.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <param name="easeOut">if set to <c>true</c> [ease out].</param>
        /// <returns>Quaternion.</returns>
        public static Quaternion RotationBetween(Path pts, float t1, float t2, EasingType ease, bool easeIn, bool easeOut)
        {
            return Quaternion.LookRotation(Interp(pts, t2, ease, easeIn, easeOut) - Interp(pts, t1, ease, easeIn, easeOut));
        }

        /// <summary>
        /// Velocities the specified PTS.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Velocity(Path pts, float t)
        {
            return Velocity(pts, t, EasingType.Linear);
        }

        /// <summary>
        /// Velocities the specified PTS.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Velocity(Path pts, float t, EasingType ease)
        {
            return Velocity(pts, t, ease, true);
        }

        /// <summary>
        /// Velocities the specified PTS.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Velocity(Path pts, float t, EasingType ease, bool easeIn)
        {
            return Velocity(pts, t, ease, easeIn, true);
        }

        /// <summary>
        /// Velocities the specified PTS.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <param name="easeOut">if set to <c>true</c> [ease out].</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Velocity(Path pts, float t, EasingType ease, bool easeIn, bool easeOut)
        {
            t = Ease(t);
            if (pts.Length == 0)
            {
                return Vector3.zero;
            }
            else if (pts.Length == 1)
            {
                return pts[0];
            }
            else if (pts.Length == 2)
            {
                return Vector3.Lerp(pts[0], pts[1], t);
            }
            else if (pts.Length == 3)
            {
                return QuadBez.Velocity(pts[0], pts[2], pts[1], t);
            }
            else if (pts.Length == 3)
            {
                return CubicBez.Velocity(pts[0], pts[3], pts[1], pts[2], t);
            }
            else
            {
                return CRSpline.Velocity(Wrap(pts), t);
            }
        }

        /// <summary>
        /// Wraps the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Vector3[].</returns>
        public static Vector3[] Wrap(Vector3[] path)
        {
            return (new Vector3[] { path[0] }).Concat(path).Concat(new Vector3[] { path[path.Length - 1] }).ToArray();
        }

        /// <summary>
        /// Gizmoes the draw.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        public static void GizmoDraw(Vector3[] pts, float t)
        {
            GizmoDraw(pts, t, EasingType.Linear);
        }

        /// <summary>
        /// Gizmoes the draw.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        public static void GizmoDraw(Vector3[] pts, float t, EasingType ease)
        {
            GizmoDraw(pts, t, ease, true);
        }

        /// <summary>
        /// Gizmoes the draw.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        public static void GizmoDraw(Vector3[] pts, float t, EasingType ease, bool easeIn)
        {
            GizmoDraw(pts, t, ease, easeIn, true);
        }

        /// <summary>
        /// Gizmoes the draw.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="t">The t.</param>
        /// <param name="ease">The ease.</param>
        /// <param name="easeIn">if set to <c>true</c> [ease in].</param>
        /// <param name="easeOut">if set to <c>true</c> [ease out].</param>
        public static void GizmoDraw(Vector3[] pts, float t, EasingType ease, bool easeIn, bool easeOut)
        {
            Gizmos.color = Color.white;
            Vector3 prevPt = Interp(pts, 0);

            for (int i = 1; i <= 20; i++)
            {
                float pm = (float)i / 20f;
                Vector3 currPt = Interp(pts, pm, ease, easeIn, easeOut);
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }

            Gizmos.color = Color.blue;
            Vector3 pos = Interp(pts, t, ease, easeIn, easeOut);
            Gizmos.DrawLine(pos, pos + Velocity(pts, t, ease, easeIn, easeOut));
        }

        /// <summary>
        /// Class Path.
        /// </summary>
        public class Path
        {
            private Vector3[] _path;

            /// <summary>
            /// Gets or sets the path.
            /// </summary>
            /// <value>The path.</value>
            public Vector3[] path
            {
                get
                {
                    return _path;
                }
                set
                {
                    _path = value;
                }
            }

            /// <summary>
            /// Gets the length.
            /// </summary>
            /// <value>The length.</value>
            public int Length
            {
                get
                {
                    return path != null ? path.Length : 0;
                }
            }

            /// <summary>
            /// Gets the <see cref="Vector3"/> at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>Vector3.</returns>
            public Vector3 this[int index]
            {
                get
                {
                    return path[index];
                }
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="Vector3[]"/> to <see cref="Path"/>.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator Path(Vector3[] path)
            {
                return new Path() { path = path };
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="Path"/> to <see cref="Vector3[]"/>.
            /// </summary>
            /// <param name="p">The p.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator Vector3[] (Path p)
            {
                return p != null ? p.path : new Vector3[0];
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="Transform[]"/> to <see cref="Path"/>.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator Path(Transform[] path)
            {
                return new Path() { path = path.Select(p => p.position).ToArray() };
            }

            /// <summary>
            /// Performs an implicit conversion from <see cref="GameObject[]"/> to <see cref="Path"/>.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>The result of the conversion.</returns>
            public static implicit operator Path(GameObject[] path)
            {
                return new Path() { path = path.Select(p => p.transform.position).ToArray() };
            }
        }
    }
}