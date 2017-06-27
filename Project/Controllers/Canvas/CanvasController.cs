using UnityEngine;
using UnityEngine.UI;

namespace Lerp2API.Controllers._Canvas
{
    /// <summary>
    /// Class CanvasController.
    /// </summary>
    public class CanvasController
    {
        /// <summary>
        /// Creates the canvas.
        /// </summary>
        /// <param name="goName">Name of the go.</param>
        /// <returns>GameObject.</returns>
        public static GameObject CreateCanvas(string goName)
        {
            GameObject go = new GameObject(goName);

            go.AddComponent<RectTransform>();
            go.AddComponent<Canvas>();
            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();

            scaler.dynamicPixelsPerUnit = 4;

            return go;
        }

        /// <summary>
        /// Adds the text to canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="size">The size.</param>
        /// <returns>GameObject.</returns>
        public static GameObject AddTextToCanvas(GameObject canvas, Vector2 size)
        {
            GameObject text = new GameObject("Text Render");

            RectTransform rt = canvas.GetComponent<RectTransform>();
            rt.sizeDelta = size;

            RectTransform rtt = text.AddComponent<RectTransform>();
            rtt.sizeDelta = size;

            text.AddComponent<CanvasRenderer>();
            Text txt = text.AddComponent<Text>();

            txt.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            txt.fontSize = 0;
            txt.fontStyle = FontStyle.Normal;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.resizeTextForBestFit = true;

            text.transform.SetParent(canvas.transform);

            return canvas;
        }
    }
}