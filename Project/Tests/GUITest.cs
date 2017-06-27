using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2API.Tests
{
    /// <summary>
    /// Class GUITest.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class GUITest : MonoBehaviour
    {
        /// <summary>
        /// Called when [GUI].
        /// </summary>
        public void OnGUI()
        {
            if (GUI.Button(new Rect(5, 5, 100, 25), "Test"))
                Debug.Log("Test" + (int)(Random.value * 1000));
        }
    }
}
