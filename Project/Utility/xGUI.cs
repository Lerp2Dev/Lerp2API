using UnityEngine;

namespace Lerp2API.Utility.xGUI
{
    /// <summary>
    /// Class ProgressBar.
    /// </summary>
    public class ProgressBar
    {
        /// <summary>
        /// The position
        /// </summary>
        public Rect position;
        /// <summary>
        /// The background color
        /// </summary>
        public Color backgroundColor, progressColor, outlineColor;
        private double eta = -1, ceta;
        private float reajTimer, downTimer;
        private Texture2D tbc, tpc;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="progressColor">Color of the progress.</param>
        /// <param name="outlineColor">Color of the outline.</param>
        public ProgressBar(Rect pos, Color backgroundColor, Color progressColor, Color outlineColor)
            : this(pos, backgroundColor, progressColor, outlineColor, -1) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="progressColor">Color of the progress.</param>
        /// <param name="outlineColor">Color of the outline.</param>
        /// <param name="eta">The eta.</param>
        public ProgressBar(Rect pos, Color backgroundColor, Color progressColor, Color outlineColor, double eta)
        {
            this.eta = eta;
            ceta = eta;
            position = pos;
            tbc = TextureBorder.SimpleBorder(TextureUtils.Fill(backgroundColor, (int)pos.width, (int)pos.height), outlineColor);
            tpc = TextureUtils.Fill(progressColor, (int)pos.width, (int)pos.height);
        }
        /// <summary>
        /// Shows the specified percentage.
        /// </summary>
        /// <param name="percentage">The percentage.</param>
        /// <param name="finished">if set to <c>true</c> [finished].</param>
        /// <param name="showPercentage">if set to <c>true</c> [show percentage].</param>
        public void Show(double percentage, bool finished, bool showPercentage = true)
        {
            float fill = position.width - 10f,
                  cur = (float)(fill * percentage);
            GUI.DrawTexture(position, tbc);
            GUI.DrawTexture(new Rect((cur - fill) / fill + position.xMin + 5f, position.yMin + 5f, fill * Mathf.Clamp01((float)percentage), position.height - 10f), tpc);
            if (showPercentage)
            {
                percentage = Mathf.Clamp01((float)percentage);
                double dur = finished || eta == -1 ? -1 : ceta;
                GUI.Label(position, string.Format("{0:F2}%{1}", percentage * 100.0, dur == -1 || dur == double.MaxValue ? "" : string.Format(" (Time Remaining: {0:F0}s)", Mathf.Clamp((float)dur, 0, (float)eta))), new GUIStyle("label") { alignment = TextAnchor.MiddleCenter, normal = new GUIStyleState { textColor = Color.black } });
            }
        }
        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <param name="per">The per.</param>
        public void GetDuration(double per)
        {
            if (reajTimer - Time.fixedTime <= 0)
            {
                ceta = eta - eta * per; //Every 10 seconds make a reajustement
                reajTimer = Time.fixedTime + 10;
            }
            if (downTimer - Time.fixedTime <= 0)
            { //Substract every second 1 unit
                --ceta;
                downTimer = Time.fixedTime + 1;
            }
        }
    }
}
