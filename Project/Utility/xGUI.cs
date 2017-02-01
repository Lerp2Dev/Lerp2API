using UnityEngine;

namespace Lerp2API.Utility.xGUI
{
    public class ProgressBar
    {
        public Rect position;
        public Color backgroundColor, progressColor, outlineColor;
        private double eta = -1, ceta;
        private float reajTimer, downTimer;
        private Texture2D tbc, tpc;
        public ProgressBar(Rect pos, Color backgroundColor, Color progressColor, Color outlineColor)
            : this(pos, backgroundColor, progressColor, outlineColor, -1) { }
        public ProgressBar(Rect pos, Color backgroundColor, Color progressColor, Color outlineColor, double eta)
        {
            this.eta = eta;
            ceta = eta;
            position = pos;
            tbc = TextureBorder.SimpleBorder(TextureUtils.Fill(backgroundColor, (int)pos.width, (int)pos.height), outlineColor);
            tpc = TextureUtils.Fill(progressColor, (int)pos.width, (int)pos.height);
        }
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
