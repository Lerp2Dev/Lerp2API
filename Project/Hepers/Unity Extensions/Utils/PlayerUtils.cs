using Lerp2API.Controllers.PersonView;
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

        public static Transform subFirstObject
        {
            get
            {
                return player.transform.GetChild(0);
            }
        }

        public static PersonView GetCurView()
        {
            if (player == null)
            {
                Debug.LogWarning("Please, define a player by setting the tag to a gameObject.");
                return default(PersonView);
            }

            return player.GetComponent("FirstPersonController") != null ? PersonView.First : PersonView.Third;
        }
    }
}