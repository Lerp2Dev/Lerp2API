using System;
using UnityEngine;

namespace Lerp2Assets.Utility
{
    /// <summary>
    /// Class CurveControlledBob.
    /// </summary>
    [Serializable]
    public class CurveControlledBob
    {
        /// <summary>
        /// The horizontal bob range
        /// </summary>
        public float HorizontalBobRange = 0.33f;

        /// <summary>
        /// The vertical bob range
        /// </summary>
        public float VerticalBobRange = 0.33f;

        //[NonSerialized]
        /// <summary>
        /// The bobcurve
        /// </summary>
        public AnimationCurve Bobcurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f),
                                                            new Keyframe(1f, 0f), new Keyframe(1.5f, -1f),
                                                            new Keyframe(2f, 0f)); // sin curve for head bob

        /// <summary>
        /// The verticalto horizontal ratio
        /// </summary>
        public float VerticaltoHorizontalRatio = 1f;

        private float m_CyclePositionX;
        private float m_CyclePositionY;
        private float m_BobBaseInterval;

        [NonSerialized]
        private Vector3 m_OriginalCameraPosition;

        private float m_Time;

        /// <summary>
        /// Setups the specified camera.
        /// </summary>
        /// <param name="camera">The camera.</param>
        /// <param name="bobBaseInterval">The bob base interval.</param>
        public void Setup(Camera camera, float bobBaseInterval)
        {
            m_BobBaseInterval = bobBaseInterval;
            m_OriginalCameraPosition = camera.transform.localPosition;

            // get the length of the curve in time
            m_Time = Bobcurve[Bobcurve.length - 1].time;
        }

        /// <summary>
        /// Does the head bob.
        /// </summary>
        /// <param name="speed">The speed.</param>
        /// <returns>Vector3.</returns>
        public Vector3 DoHeadBob(float speed)
        {
            float xPos = m_OriginalCameraPosition.x + (Bobcurve.Evaluate(m_CyclePositionX) * HorizontalBobRange);
            float yPos = m_OriginalCameraPosition.y + (Bobcurve.Evaluate(m_CyclePositionY) * VerticalBobRange);

            m_CyclePositionX += (speed * Time.deltaTime) / m_BobBaseInterval;
            m_CyclePositionY += ((speed * Time.deltaTime) / m_BobBaseInterval) * VerticaltoHorizontalRatio;

            if (m_CyclePositionX > m_Time)
            {
                m_CyclePositionX = m_CyclePositionX - m_Time;
            }
            if (m_CyclePositionY > m_Time)
            {
                m_CyclePositionY = m_CyclePositionY - m_Time;
            }

            return new Vector3(xPos, yPos, 0f);
        }
    }
}