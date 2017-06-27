using System;
using System.Collections;
using UnityEngine;

namespace Lerp2Assets.Utility
{
    /// <summary>
    /// Class LerpControlledBob.
    /// </summary>
    [Serializable]
    public class LerpControlledBob
    {
        /// <summary>
        /// The bob duration
        /// </summary>
        public float BobDuration;

        /// <summary>
        /// The bob amount
        /// </summary>
        public float BobAmount;

        private float m_Offset = 0f;

        // provides the offset that can be used
        /// <summary>
        /// Offsets this instance.
        /// </summary>
        /// <returns>System.Single.</returns>
        public float Offset()
        {
            return m_Offset;
        }

        /// <summary>
        /// Does the bob cycle.
        /// </summary>
        /// <returns>IEnumerator.</returns>
        public IEnumerator DoBobCycle()
        {
            // make the camera move down slightly
            float t = 0f;
            while (t < BobDuration)
            {
                m_Offset = Mathf.Lerp(0f, BobAmount, t / BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            // make it move back to neutral
            t = 0f;
            while (t < BobDuration)
            {
                m_Offset = Mathf.Lerp(BobAmount, 0f, t / BobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            m_Offset = 0f;
        }
    }
}