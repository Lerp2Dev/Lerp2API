using System.Collections;
using UnityEngine;

namespace Lerp2API.Game
{
    public class FPSCounter : MonoBehaviour
    {
        public float m_updateInterval = 0.5f;
        public int width = 200,
                   height = 40;
        public Position m_counterPosition;
        public int m_fontSize = 14;

        private const string display = "{0:F2} FPS";
        private string m_Text = "";
        private bool m_Active = true;

        public float FramesPerSec { get; protected set; }

        private void Start()
        {
            StartCoroutine("FPS");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F10))
                m_Active = !m_Active;
        }

        private IEnumerator FPS()
        {
            for (;;)
            {
                // Capture frame-per-second
                int lastFrameCount = Time.frameCount;
                float lastTime = Time.realtimeSinceStartup;
                yield return new WaitForSeconds(m_updateInterval);
                float timeSpan = Time.realtimeSinceStartup - lastTime;
                int frameCount = Time.frameCount - lastFrameCount;

                // Display it
                FramesPerSec = frameCount / timeSpan;
                m_Text = string.Format(display, FramesPerSec);
            }
        }

        private void OnGUI()
        {
            if (m_Active)
                GUI.Label(m_counterPosition.GetPosition(width, height), m_Text, new GUIStyle("label") { fontSize = m_fontSize, normal = new GUIStyleState() { textColor = GetTextColor() } });
        }

        private Color GetTextColor()
        {
            return Color.Lerp(Color.red, Color.green, Mathf.Clamp01((FramesPerSec - 10) / 50f));
        }
    }
}