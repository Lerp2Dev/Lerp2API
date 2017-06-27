using System;
using UnityEngine;

namespace Lerp2API.Hepers.Unity_Extensions
{
    /// <summary>
    /// Class AdvancedCoroutine.
    /// </summary>
    public class AdvancedCoroutine
    {
        /// <summary>
        /// The start time
        /// </summary>
        public float startTime;

        /// <summary>
        /// The cor
        /// </summary>
        public Coroutine cor;

        private MonoBehaviour mono;
        private Action<AdvancedCoroutine> act;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedCoroutine"/> class.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="a">a.</param>
        public AdvancedCoroutine(MonoBehaviour m, Action<AdvancedCoroutine> a)
        {
            startTime = Time.realtimeSinceStartup;
            mono = m;
            act = a;
        }

        /// <summary>
        /// Stops the coroutine.
        /// </summary>
        public void StopCoroutine()
        {
            mono.StopCoroutine(cor);
        }
    }
}