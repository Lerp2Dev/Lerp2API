using System.Collections.Generic;
using UnityEngine;

namespace Lerp2API.Hepers.Unity_Extensions.Utils
{
    /// <summary>
    /// Class KeyMap.
    /// </summary>
    public partial class KeyMap
    {
        private static Dictionary<string, KeyCode> map = new Dictionary<string, KeyCode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyMap"/> class.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="kc">The kc.</param>
        public KeyMap(string n, KeyCode kc)
        {
            map.Add(n, kc);
        }

        /// <summary>
        /// Gets or sets the <see cref="KeyCode"/> with the specified s.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>KeyCode.</returns>
        public KeyCode this[string s]
        {
            get { return map[s]; }
            set { map[s] = value; }
        }

        /// <summary>
        /// Determines whether [is key definition] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if [is key definition] [the specified key]; otherwise, <c>false</c>.</returns>
        public bool IsKeyDef(string key)
        {
            return map.ContainsKey(key);
        }
    }
}