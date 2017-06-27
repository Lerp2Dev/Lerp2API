// Type: UnityEngine.Mathd
// Assembly: UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Program Files (x86)\Unity\Editor\Data\Managed\UnityEngine.dll
using System;

namespace UnityEngine
{
    /// <summary>
    /// Struct Mathd
    /// </summary>
    public struct Mathd
    {
        /// <summary>
        /// The pi
        /// </summary>
        public const double PI = 3.141593d;

        /// <summary>
        /// The infinity
        /// </summary>
        public const double Infinity = double.PositiveInfinity;

        /// <summary>
        /// The negative infinity
        /// </summary>
        public const double NegativeInfinity = double.NegativeInfinity;

        /// <summary>
        /// The deg2 RAD
        /// </summary>
        public const double Deg2Rad = 0.01745329d;

        /// <summary>
        /// The rad2 deg
        /// </summary>
        public const double Rad2Deg = 57.29578d;

        /// <summary>
        /// The epsilon
        /// </summary>
        public const double Epsilon = 1.401298E-45d;

        /// <summary>
        /// Sins the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Sin(double d)
        {
            return Math.Sin(d);
        }

        /// <summary>
        /// Coses the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Cos(double d)
        {
            return Math.Cos(d);
        }

        /// <summary>
        /// Tans the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Tan(double d)
        {
            return Math.Tan(d);
        }

        /// <summary>
        /// Asins the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Asin(double d)
        {
            return Math.Asin(d);
        }

        /// <summary>
        /// Acoses the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Acos(double d)
        {
            return Math.Acos(d);
        }

        /// <summary>
        /// Atans the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Atan(double d)
        {
            return Math.Atan(d);
        }

        /// <summary>
        /// Atan2s the specified y.
        /// </summary>
        /// <param name="y">The y.</param>
        /// <param name="x">The x.</param>
        /// <returns>System.Double.</returns>
        public static double Atan2(double y, double x)
        {
            return Math.Atan2(y, x);
        }

        /// <summary>
        /// SQRTs the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Sqrt(double d)
        {
            return Math.Sqrt(d);
        }

        /// <summary>
        /// Abses the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Abs(double d)
        {
            return Math.Abs(d);
        }

        /// <summary>
        /// Abses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public static int Abs(int value)
        {
            return Math.Abs(value);
        }

        /// <summary>
        /// Minimums the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Double.</returns>
        public static double Min(double a, double b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        /// <summary>
        /// Minimums the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>System.Double.</returns>
        public static double Min(params double[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0.0d;
            double num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] < num)
                    num = values[index];
            }
            return num;
        }

        /// <summary>
        /// Minimums the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Int32.</returns>
        public static int Min(int a, int b)
        {
            if (a < b)
                return a;
            else
                return b;
        }

        /// <summary>
        /// Minimums the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>System.Int32.</returns>
        public static int Min(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0;
            int num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] < num)
                    num = values[index];
            }
            return num;
        }

        /// <summary>
        /// Maximums the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Double.</returns>
        public static double Max(double a, double b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        /// <summary>
        /// Maximums the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>System.Double.</returns>
        public static double Max(params double[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0d;
            double num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if ((double)values[index] > (double)num)
                    num = values[index];
            }
            return num;
        }

        /// <summary>
        /// Maximums the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Int32.</returns>
        public static int Max(int a, int b)
        {
            if (a > b)
                return a;
            else
                return b;
        }

        /// <summary>
        /// Maximums the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>System.Int32.</returns>
        public static int Max(params int[] values)
        {
            int length = values.Length;
            if (length == 0)
                return 0;
            int num = values[0];
            for (int index = 1; index < length; ++index)
            {
                if (values[index] > num)
                    num = values[index];
            }
            return num;
        }

        /// <summary>
        /// Pows the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="p">The p.</param>
        /// <returns>System.Double.</returns>
        public static double Pow(double d, double p)
        {
            return Math.Pow(d, p);
        }

        /// <summary>
        /// Exps the specified power.
        /// </summary>
        /// <param name="power">The power.</param>
        /// <returns>System.Double.</returns>
        public static double Exp(double power)
        {
            return Math.Exp(power);
        }

        /// <summary>
        /// Logs the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="p">The p.</param>
        /// <returns>System.Double.</returns>
        public static double Log(double d, double p)
        {
            return Math.Log(d, p);
        }

        /// <summary>
        /// Logs the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Log(double d)
        {
            return Math.Log(d);
        }

        /// <summary>
        /// Log10s the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Log10(double d)
        {
            return Math.Log10(d);
        }

        /// <summary>
        /// Ceils the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Ceil(double d)
        {
            return Math.Ceiling(d);
        }

        /// <summary>
        /// Floors the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Floor(double d)
        {
            return Math.Floor(d);
        }

        /// <summary>
        /// Rounds the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Round(double d)
        {
            return Math.Round(d);
        }

        /// <summary>
        /// Ceils to int.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Int32.</returns>
        public static int CeilToInt(double d)
        {
            return (int)Math.Ceiling(d);
        }

        /// <summary>
        /// Floors to int.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Int32.</returns>
        public static int FloorToInt(double d)
        {
            return (int)Math.Floor(d);
        }

        /// <summary>
        /// Rounds to int.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Int32.</returns>
        public static int RoundToInt(double d)
        {
            return (int)Math.Round(d);
        }

        /// <summary>
        /// Signs the specified d.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>System.Double.</returns>
        public static double Sign(double d)
        {
            return d >= 0.0 ? 1d : -1d;
        }

        /// <summary>
        /// Clamps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>System.Double.</returns>
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        /// <summary>
        /// Clamps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>System.Int32.</returns>
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        /// <summary>
        /// Clamp01s the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public static double Clamp01(double value)
        {
            if (value < 0.0)
                return 0.0d;
            if (value > 1.0)
                return 1d;
            else
                return value;
        }

        /// <summary>
        /// Lerps the specified from.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="t">The t.</param>
        /// <returns>System.Double.</returns>
        public static double Lerp(double from, double to, double t)
        {
            return from + (to - from) * Mathd.Clamp01(t);
        }

        /// <summary>
        /// Lerps the angle.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <param name="t">The t.</param>
        /// <returns>System.Double.</returns>
        public static double LerpAngle(double a, double b, double t)
        {
            double num = Mathd.Repeat(b - a, 360d);
            if (num > 180.0d)
                num -= 360d;
            return a + num * Mathd.Clamp01(t);
        }

        /// <summary>
        /// Moves the towards.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="target">The target.</param>
        /// <param name="maxDelta">The maximum delta.</param>
        /// <returns>System.Double.</returns>
        public static double MoveTowards(double current, double target, double maxDelta)
        {
            if (Mathd.Abs(target - current) <= maxDelta)
                return target;
            else
                return current + Mathd.Sign(target - current) * maxDelta;
        }

        /// <summary>
        /// Moves the towards angle.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="target">The target.</param>
        /// <param name="maxDelta">The maximum delta.</param>
        /// <returns>System.Double.</returns>
        public static double MoveTowardsAngle(double current, double target, double maxDelta)
        {
            target = current + Mathd.DeltaAngle(current, target);
            return Mathd.MoveTowards(current, target, maxDelta);
        }

        /// <summary>
        /// Smoothes the step.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="t">The t.</param>
        /// <returns>System.Double.</returns>
        public static double SmoothStep(double from, double to, double t)
        {
            t = Mathd.Clamp01(t);
            t = (-2.0 * t * t * t + 3.0 * t * t);
            return to * t + from * (1.0 - t);
        }

        /// <summary>
        /// Gammas the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="absmax">The absmax.</param>
        /// <param name="gamma">The gamma.</param>
        /// <returns>System.Double.</returns>
        public static double Gamma(double value, double absmax, double gamma)
        {
            bool flag = false;
            if (value < 0.0)
                flag = true;
            double num1 = Mathd.Abs(value);
            if (num1 > absmax)
            {
                if (flag)
                    return -num1;
                else
                    return num1;
            }
            else
            {
                double num2 = Mathd.Pow(num1 / absmax, gamma) * absmax;
                if (flag)
                    return -num2;
                else
                    return num2;
            }
        }

        /// <summary>
        /// Approximatelies the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Approximately(double a, double b)
        {
            return Mathd.Abs(b - a) < Mathd.Max(1E-06d * Mathd.Max(Mathd.Abs(a), Mathd.Abs(b)), 1.121039E-44d);
        }

        /// <summary>
        /// Smoothes the damp.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="target">The target.</param>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <param name="smoothTime">The smooth time.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <returns>System.Double.</returns>
        public static double SmoothDamp(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed)
        {
            double deltaTime = (double)Time.deltaTime;
            return Mathd.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        /// <summary>
        /// Smoothes the damp.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="target">The target.</param>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <param name="smoothTime">The smooth time.</param>
        /// <returns>System.Double.</returns>
        public static double SmoothDamp(double current, double target, ref double currentVelocity, double smoothTime)
        {
            double deltaTime = Time.deltaTime;
            double maxSpeed = double.PositiveInfinity;
            return Mathd.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        /// <summary>
        /// Smoothes the damp.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="target">The target.</param>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <param name="smoothTime">The smooth time.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <param name="deltaTime">The delta time.</param>
        /// <returns>System.Double.</returns>
        public static double SmoothDamp(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
        {
            smoothTime = Mathd.Max(0.0001d, smoothTime);
            double num1 = 2d / smoothTime;
            double num2 = num1 * deltaTime;
            double num3 = (1.0d / (1.0d + num2 + 0.479999989271164d * num2 * num2 + 0.234999999403954d * num2 * num2 * num2));
            double num4 = current - target;
            double num5 = target;
            double max = maxSpeed * smoothTime;
            double num6 = Mathd.Clamp(num4, -max, max);
            target = current - num6;
            double num7 = (currentVelocity + num1 * num6) * deltaTime;
            currentVelocity = (currentVelocity - num1 * num7) * num3;
            double num8 = target + (num6 + num7) * num3;
            if (num5 - current > 0.0 == num8 > num5)
            {
                num8 = num5;
                currentVelocity = (num8 - num5) / deltaTime;
            }
            return num8;
        }

        /// <summary>
        /// Smoothes the damp angle.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="target">The target.</param>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <param name="smoothTime">The smooth time.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <returns>System.Double.</returns>
        public static double SmoothDampAngle(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed)
        {
            double deltaTime = (double)Time.deltaTime;
            return Mathd.SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        /// <summary>
        /// Smoothes the damp angle.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="target">The target.</param>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <param name="smoothTime">The smooth time.</param>
        /// <returns>System.Double.</returns>
        public static double SmoothDampAngle(double current, double target, ref double currentVelocity, double smoothTime)
        {
            double deltaTime = (double)Time.deltaTime;
            double maxSpeed = double.PositiveInfinity;
            return Mathd.SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        /// <summary>
        /// Smoothes the damp angle.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="target">The target.</param>
        /// <param name="currentVelocity">The current velocity.</param>
        /// <param name="smoothTime">The smooth time.</param>
        /// <param name="maxSpeed">The maximum speed.</param>
        /// <param name="deltaTime">The delta time.</param>
        /// <returns>System.Double.</returns>
        public static double SmoothDampAngle(double current, double target, ref double currentVelocity, double smoothTime, double maxSpeed, double deltaTime)
        {
            target = current + Mathd.DeltaAngle(current, target);
            return Mathd.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
        }

        /// <summary>
        /// Repeats the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.Double.</returns>
        public static double Repeat(double t, double length)
        {
            return t - Mathd.Floor(t / length) * length;
        }

        /// <summary>
        /// Pings the pong.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.Double.</returns>
        public static double PingPong(double t, double length)
        {
            t = Mathd.Repeat(t, length * 2d);
            return length - Mathd.Abs(t - length);
        }

        /// <summary>
        /// Inverses the lerp.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Double.</returns>
        public static double InverseLerp(double from, double to, double value)
        {
            if (from < to)
            {
                if (value < from)
                    return 0d;
                if (value > to)
                    return 1d;
                value -= from;
                value /= to - from;
                return value;
            }
            else
            {
                if (from <= to)
                    return 0d;
                if (value < to)
                    return 1d;
                if (value > from)
                    return 0d;
                else
                    return (1.0d - (value - to) / (from - to));
            }
        }

        /// <summary>
        /// Deltas the angle.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="target">The target.</param>
        /// <returns>System.Double.</returns>
        public static double DeltaAngle(double current, double target)
        {
            double num = Mathd.Repeat(target - current, 360d);
            if (num > 180.0d)
                num -= 360d;
            return num;
        }

        internal static bool LineIntersection(Vector2d p1, Vector2d p2, Vector2d p3, Vector2d p4, ref Vector2d result)
        {
            double num1 = p2.x - p1.x;
            double num2 = p2.y - p1.y;
            double num3 = p4.x - p3.x;
            double num4 = p4.y - p3.y;
            double num5 = num1 * num4 - num2 * num3;
            if (num5 == 0.0d)
                return false;
            double num6 = p3.x - p1.x;
            double num7 = p3.y - p1.y;
            double num8 = (num6 * num4 - num7 * num3) / num5;
            result = new Vector2d(p1.x + num8 * num1, p1.y + num8 * num2);
            return true;
        }

        internal static bool LineSegmentIntersection(Vector2d p1, Vector2d p2, Vector2d p3, Vector2d p4, ref Vector2d result)
        {
            double num1 = p2.x - p1.x;
            double num2 = p2.y - p1.y;
            double num3 = p4.x - p3.x;
            double num4 = p4.y - p3.y;
            double num5 = (num1 * num4 - num2 * num3);
            if (num5 == 0.0d)
                return false;
            double num6 = p3.x - p1.x;
            double num7 = p3.y - p1.y;
            double num8 = (num6 * num4 - num7 * num3) / num5;
            if (num8 < 0.0d || num8 > 1.0d)
                return false;
            double num9 = (num6 * num2 - num7 * num1) / num5;
            if (num9 < 0.0d || num9 > 1.0d)
                return false;
            result = new Vector2d(p1.x + num8 * num1, p1.y + num8 * num2);
            return true;
        }
    }
}