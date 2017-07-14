//Based on Unity's PostEffectsBase.js, just so I don't have to import their standard assets all the time.
//Also so I can customize it

using UnityEngine;
using System.Collections;

namespace M8.ImageEffects
{
    /// <summary>
    /// Class PostEffectsBase.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("")]
    public abstract class PostEffectsBase : MonoBehaviour
    {
        /// <summary>
        /// The support HDR textures
        /// </summary>
        protected bool supportHDRTextures = true;

        /// <summary>
        /// The support d X11
        /// </summary>
        protected bool supportDX11 = false;

        /// <summary>
        /// The is supported
        /// </summary>
        protected bool isSupported = true;

        private bool mStarted = false;

        /// <summary>
        /// Checks the shader and create material.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="m2Create">The m2 create.</param>
        /// <returns>Material.</returns>
        public Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
        {
            if (s == null)
            {
                Debug.Log("Missing shader in " + name);
                enabled = false;
                return null;
            }

            if (s.isSupported && m2Create && m2Create.shader == s)
                return m2Create;

            if (!s.isSupported)
            {
                NotSupported();
                Debug.Log("The shader " + s.ToString() + " on effect " + this.ToString() + " is not supported on this platform!");
                return null;
            }
            else
            {
                m2Create = new Material(s);
                m2Create.hideFlags = HideFlags.DontSave;
                if (m2Create)
                    return m2Create;
                else return null;
            }
        }

        /// <summary>
        /// Creates the material.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="m2Create">The m2 create.</param>
        /// <returns>Material.</returns>
        public Material CreateMaterial(Shader s, Material m2Create)
        {
            if (s == null)
            {
                Debug.Log("Missing shader in " + this.ToString());
                return null;
            }

            if (m2Create != null && (m2Create.shader == s) && (s.isSupported))
                return m2Create;

            if (!s.isSupported)
            {
                return null;
            }
            else
            {
                m2Create = new Material(s);
                m2Create.hideFlags = HideFlags.DontSave;
                if (m2Create)
                    return m2Create;
                else return null;
            }
        }

        /// <summary>
        /// Checks the support.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CheckSupport()
        {
            return CheckSupport(false);
        }

        /// <summary>
        /// Checks the resources.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public abstract bool CheckResources();

        /// <summary>
        /// Checks the support.
        /// </summary>
        /// <param name="needDepth">if set to <c>true</c> [need depth].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CheckSupport(bool needDepth)
        {
            isSupported = true;
            supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
            supportDX11 = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;

            if (!SystemInfo.supportsImageEffects)
            {
                NotSupported();
                return false;
            }

            if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
            {
                NotSupported();
                return false;
            }

            if (needDepth)
            {
                Camera cam = GetComponent<Camera>();
                if (cam)
                    cam.depthTextureMode |= DepthTextureMode.Depth;
            }

            return true;
        }

        /// <summary>
        /// Checks the support.
        /// </summary>
        /// <param name="needDepth">if set to <c>true</c> [need depth].</param>
        /// <param name="needHdr">if set to <c>true</c> [need HDR].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CheckSupport(bool needDepth, bool needHdr)
        {
            if (!CheckSupport(needDepth))
                return false;

            if (needHdr && !supportHDRTextures)
            {
                NotSupported();
                return false;
            }

            return true;
        }

        /// <summary>
        /// DX11s the support.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Dx11Support()
        {
            return supportDX11;
        }

        /// <summary>
        /// Reports the automatic disable.
        /// </summary>
        public void ReportAutoDisable()
        {
            Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
        }

        // deprecated but needed for old effects to survive upgrading
        /// <summary>
        /// Checks the shader.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CheckShader(Shader s)
        {
            Debug.Log("The shader " + s.ToString() + " on effect " + this.ToString() + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package.");
            if (!s.isSupported)
            {
                NotSupported();
                return false;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Nots the supported.
        /// </summary>
        public void NotSupported()
        {
            enabled = false;
            isSupported = false;
            return;
        }

        /// <summary>
        /// Draws the border.
        /// </summary>
        /// <param name="dest">The dest.</param>
        /// <param name="material">The material.</param>
        public void DrawBorder(RenderTexture dest, Material material)
        {
            float x1, x2, y1, y2;

            RenderTexture.active = dest;
            bool invertY = true; // source.texelSize.y < 0.0f;
            // Set up the simple Matrix
            GL.PushMatrix();
            GL.LoadOrtho();

            for (int i = 0; i < material.passCount; i++)
            {
                material.SetPass(i);

                float y1_, y2_;

                if (invertY)
                {
                    y1_ = 1.0f; y2_ = 0.0f;
                }
                else
                {
                    y1_ = 0.0f; y2_ = 1.0f;
                }

                // left
                x1 = 0.0f;
                x2 = 0.0f + 1.0f / (dest.width * 1.0f);
                y1 = 0.0f;
                y2 = 1.0f;
                GL.Begin(GL.QUADS);

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // right
                x1 = 1.0f - 1.0f / (dest.width * 1.0f);
                x2 = 1.0f;
                y1 = 0.0f;
                y2 = 1.0f;

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // top
                x1 = 0.0f;
                x2 = 1.0f;
                y1 = 0.0f;
                y2 = 0.0f + 1.0f / (dest.height * 1.0f);

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // bottom
                x1 = 0.0f;
                x2 = 1.0f;
                y1 = 1.0f - 1.0f / (dest.height * 1.0f);
                y2 = 1.0f;

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                GL.End();
            }

            GL.PopMatrix();
        }

        private void OnEnable()
        {
            if (mStarted)
                CheckResources();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        protected virtual void Start()
        {
            mStarted = true;
            CheckResources();
        }
    }
}