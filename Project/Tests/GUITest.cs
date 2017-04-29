using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2API.Tests
{
    public class GUITest : MonoBehaviour
    {
        public void OnGUI()
        {
            if (GUI.Button(new Rect(5, 5, 100, 25), "Test"))
                Debug.Log("Test" + (int)(Random.value * 1000));
        }
    }
}
