using UnityEngine;

namespace UnitySerializerNG.FilePreferences
{
    /// <summary>
    /// Class SaveOnQuit.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class SaveOnQuit : MonoBehaviour
    {
        /// <summary>
        /// Gets the instances.
        /// </summary>
        /// <value>The instances.</value>
        public static int Instances
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveOnQuit"/> class.
        /// </summary>
        public SaveOnQuit()
        {
            Instances++;
        }

        private void OnApplicationQuit()
        {
            FilePrefs.Save();
        }
    }
}