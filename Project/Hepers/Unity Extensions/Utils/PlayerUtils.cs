using UnityEngine;

namespace Lerp2API.Hepers.Unity_Extensions.Utils
{
    /// <summary>
    /// Class PlayerUtils.
    /// </summary>
    public class PlayerUtils
    {
        private static GameObject _p;

        /// <summary>
        /// Gets the player.
        /// </summary>
        /// <value>The player.</value>
        public static GameObject player
        {
            get
            {
                if (_p == null)
                    _p = GameObject.FindGameObjectWithTag("Player");
                return _p;
            }
        }
    }
}