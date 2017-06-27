using System;

namespace RadicalLibrary
{
    /// <summary>
    /// Class Easing.
    /// </summary>
    public static class Easing
    {
        // Adapted from source : http://www.robertpenner.com/easing/

        /// <summary>
        /// Eases the specified linear step.
        /// </summary>
        /// <param name="linearStep">The linear step.</param>
        /// <param name="acceleration">The acceleration.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Single.</returns>
        public static float Ease(double linearStep, float acceleration, EasingType type)
        {
            float easedStep = acceleration > 0 ? EaseIn(linearStep, type) :
                              acceleration < 0 ? EaseOut(linearStep, type) :
                              (float)linearStep;

            return MathHelper.Lerp(linearStep, easedStep, Math.Abs(acceleration));
        }

        /// <summary>
        /// Eases the in.
        /// </summary>
        /// <param name="linearStep">The linear step.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Single.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static float EaseIn(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear: return (float)linearStep;
                case EasingType.Sine: return Sine.EaseIn(linearStep);
                case EasingType.Quadratic: return Power.EaseIn(linearStep, 2);
                case EasingType.Cubic: return Power.EaseIn(linearStep, 3);
                case EasingType.Quartic: return Power.EaseIn(linearStep, 4);
                case EasingType.Quintic: return Power.EaseIn(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Eases the out.
        /// </summary>
        /// <param name="linearStep">The linear step.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Single.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static float EaseOut(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear: return (float)linearStep;
                case EasingType.Sine: return Sine.EaseOut(linearStep);
                case EasingType.Quadratic: return Power.EaseOut(linearStep, 2);
                case EasingType.Cubic: return Power.EaseOut(linearStep, 3);
                case EasingType.Quartic: return Power.EaseOut(linearStep, 4);
                case EasingType.Quintic: return Power.EaseOut(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Eases the in out.
        /// </summary>
        /// <param name="linearStep">The linear step.</param>
        /// <param name="easeInType">Type of the ease in.</param>
        /// <param name="easeOutType">Type of the ease out.</param>
        /// <returns>System.Single.</returns>
        public static float EaseInOut(double linearStep, EasingType easeInType, EasingType easeOutType)
        {
            return linearStep < 0.5 ? EaseInOut(linearStep, easeInType) : EaseInOut(linearStep, easeOutType);
        }

        /// <summary>
        /// Eases the in out.
        /// </summary>
        /// <param name="linearStep">The linear step.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Single.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static float EaseInOut(double linearStep, EasingType type)
        {
            switch (type)
            {
                case EasingType.Step: return linearStep < 0.5 ? 0 : 1;
                case EasingType.Linear: return (float)linearStep;
                case EasingType.Sine: return Sine.EaseInOut(linearStep);
                case EasingType.Quadratic: return Power.EaseInOut(linearStep, 2);
                case EasingType.Cubic: return Power.EaseInOut(linearStep, 3);
                case EasingType.Quartic: return Power.EaseInOut(linearStep, 4);
                case EasingType.Quintic: return Power.EaseInOut(linearStep, 5);
            }
            throw new NotImplementedException();
        }

        private static class Sine
        {
            /// <summary>
            /// Eases the in.
            /// </summary>
            /// <param name="s">The s.</param>
            /// <returns>System.Single.</returns>
            public static float EaseIn(double s)
            {
                return (float)Math.Sin(s * MathHelper.HalfPi - MathHelper.HalfPi) + 1;
            }

            /// <summary>
            /// Eases the out.
            /// </summary>
            /// <param name="s">The s.</param>
            /// <returns>System.Single.</returns>
            public static float EaseOut(double s)
            {
                return (float)Math.Sin(s * MathHelper.HalfPi);
            }

            /// <summary>
            /// Eases the in out.
            /// </summary>
            /// <param name="s">The s.</param>
            /// <returns>System.Single.</returns>
            public static float EaseInOut(double s)
            {
                return (float)(Math.Sin(s * MathHelper.Pi - MathHelper.HalfPi) + 1) / 2;
            }
        }

        private static class Power
        {
            /// <summary>
            /// Eases the in.
            /// </summary>
            /// <param name="s">The s.</param>
            /// <param name="power">The power.</param>
            /// <returns>System.Single.</returns>
            public static float EaseIn(double s, int power)
            {
                return (float)Math.Pow(s, power);
            }

            /// <summary>
            /// Eases the out.
            /// </summary>
            /// <param name="s">The s.</param>
            /// <param name="power">The power.</param>
            /// <returns>System.Single.</returns>
            public static float EaseOut(double s, int power)
            {
                var sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign * (Math.Pow(s - 1, power) + sign));
            }

            /// <summary>
            /// Eases the in out.
            /// </summary>
            /// <param name="s">The s.</param>
            /// <param name="power">The power.</param>
            /// <returns>System.Single.</returns>
            public static float EaseInOut(double s, int power)
            {
                if (s < 0.5)
                    return EaseIn(s * 2, power) / 2;
                return (EaseOut((s - 0.5) * 2, power) / 2) + 0.5f;

                //var sign = power % 2 == 0 ? -1 : 1;
                //return (float)(sign / 2.0 * (Math.Pow(s - 2, power) + sign * 2));
            }
        }
    }

    /// <summary>
    /// Class MathHelper.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// The pi
        /// </summary>
        public const float Pi = (float)Math.PI;
        /// <summary>
        /// The half pi
        /// </summary>
        public const float HalfPi = (float)(Math.PI / 2);

        /// <summary>
        /// Lerps the specified from.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="step">The step.</param>
        /// <returns>System.Single.</returns>
        public static float Lerp(double from, double to, double step)
        {
            return (float)((to - from) * step + from);
        }
    }
}