using System;
using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2API.Hepers.Unity_Extensions.Utils
{
    /// <summary>
    /// Class LineRendererUtils.
    /// </summary>
    public static class LineRendererUtils
    {
        /// <summary>
        /// The maximum nodes
        /// </summary>
        public const int maxNodes = 1000;

        /// <summary>
        /// Instantiates the specified start width.
        /// </summary>
        /// <param name="startWidth">The start width.</param>
        /// <param name="endWidth">The end width.</param>
        /// <param name="startColor">The start color.</param>
        /// <param name="endColor">The end color.</param>
        /// <param name="obj">The object.</param>
        /// <param name="lr">The lr.</param>
        public static void Instantiate(float startWidth, float endWidth, Color startColor, Color endColor, ref GameObject obj, out LineRenderer lr)
        {
            lr = obj.GetComponent<LineRenderer>();
            if (lr == null)
                lr = obj.AddComponent<LineRenderer>();

            //Set width
            lr.startWidth = startWidth;
            lr.endWidth = endWidth;

            //Set color
            lr.startColor = startColor;
            lr.endColor = endColor;

            lr.material = new Material(Shader.Find("Particles/Additive"));
            lr.numPositions = 0;
        }

        /// <summary>
        /// Instantiates the specified LRS.
        /// </summary>
        /// <param name="lrs">The LRS.</param>
        /// <param name="obj">The object.</param>
        /// <param name="lr">The lr.</param>
        public static void Instantiate(LineRendererStyle lrs, ref GameObject obj, out LineRenderer lr)
        {
            lr = obj.GetComponent<LineRenderer>();
            if (lr == null)
                lr = obj.AddComponent<LineRenderer>();

            if (lrs == null)
            {
                Debug.LogWarning("You must pass LineRendererStyle not null.");
                return;
            }

            lrs.ApplyStyles(ref lr);
            lr.numPositions = 0;
        }

        /// <summary>
        /// Adds the point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="lr">The lr.</param>
        public static void AddPoint(Vector3 point, ref LineRenderer lr)
        {
            int pos = lr.numPositions;
            ++pos;
            lr.numPositions = pos;

            lr.SetPosition(pos - 1, point);
        }

        /*
         * ZONA TESTEO - NO USAR
         */

        /// <summary>
        /// Instatiates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="startWidth">The start width.</param>
        /// <param name="endWidth">The end width.</param>
        /// <param name="startColor">The start color.</param>
        /// <param name="endColor">The end color.</param>
        /// <returns>LineRenderer.</returns>
        public static LineRenderer Instatiate(GameObject obj, float startWidth, float endWidth, Color startColor, Color endColor)
        {
            LineRenderer lr = obj.GetComponent<LineRenderer>();
            if (lr == null)
            {
                lr = obj.AddComponent<LineRenderer>();

                //Set width
                lr.startWidth = startWidth;
                lr.endWidth = endWidth;

                //Set color
                lr.startColor = startColor;
                lr.endColor = endColor;

                lr.material = new Material(Shader.Find("Particles/Additive"));
            }

            return lr;
        }

        /// <summary>
        /// Gets the first point.
        /// </summary>
        /// <param name="lr">The lr.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 GetFirstPoint(this LineRenderer lr)
        {
            if (lr != null)
                return lr.GetPoint(0);
            return default(Vector3);
        }

        /// <summary>
        /// Gets the last point.
        /// </summary>
        /// <param name="lr">The lr.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 GetLastPoint(this LineRenderer lr)
        {
            if (lr != null)
                return lr.GetPoint(lr.numPositions - 1);
            return default(Vector3);
        }

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <param name="lr">The lr.</param>
        /// <param name="index">The index.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 GetPoint(this LineRenderer lr, int index)
        {
            if (lr != null && index < lr.numPositions)
                return lr.GetPosition(index);
            else
            {
                if (lr == null)
                    Debug.LogWarning("LineRenderer is null!");
                else
                    Debug.LogWarning("LineRenerer index request is out of bounds!");
                return default(Vector3);
            }
        }

        //No se si esto funcionará... O con lo de arriba añadiendo el componente ya se añade.
        /*public static void Instantiate(GameObject obj, LineRenderer lr, float startWidth, float endWidth, Color startColor, Color endColor)
        {
            Instantiate(startWidth, endWidth, startColor, endColor, ref,  out lr);
            obj.CopyComponent(lr); //Creo que esto puede funcionar?
        }*/
    }

    /// <summary>
    /// Class LineRendererStyle.
    /// </summary>
    [Serializable]
    public class LineRendererStyle
    {
        /// <summary>
        /// The mat
        /// </summary>
        public Material mat = new Material(Shader.Find("Particles/Additive"));

        /// <summary>
        /// The start width
        /// </summary>
        public float startWidth = .2f, endWidth = .2f;

        /// <summary>
        /// The start color
        /// </summary>
        public Color startColor = Color.red, endColor = Color.magenta;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineRendererStyle"/> class.
        /// </summary>
        /// <param name="sc">The sc.</param>
        /// <param name="ec">The ec.</param>
        /// <param name="sw">The sw.</param>
        /// <param name="ew">The ew.</param>
        /// <param name="m">The m.</param>
        public LineRendererStyle(Color sc, Color ec, float sw, float ew, Material m = null)
        {
            startColor = sc;
            endColor = sc;

            startWidth = sw;
            endWidth = ew;

            mat = m;
        }

        /// <summary>
        /// Applies the styles.
        /// </summary>
        /// <param name="lr">The lr.</param>
        public void ApplyStyles(ref LineRenderer lr)
        {
            if (mat != null)
                lr.material = mat;

            if (startColor != default(Color))
                lr.startColor = startColor;

            if (endColor != default(Color))
                lr.endColor = endColor;

            if (startWidth != default(float))
                lr.startWidth = startWidth;

            if (endWidth != default(float))
                lr.endWidth = endWidth;
        }
    }
}