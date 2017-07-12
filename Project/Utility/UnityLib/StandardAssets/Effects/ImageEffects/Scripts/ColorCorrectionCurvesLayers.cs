using M8.ImageEffects;
using System;
using UnityEngine;

// Usage: Place this file into /Standard Assets/Effects/ImageEffects/Scripts/-folder
// Requires: https://github.com/UnityCommunity/UnityLibrary/blob/master/Scripts/Editor/ImageEffects/ColorCorrectionCurvesEditorLayers

namespace UnityStandardAssets.ImageEffects
{
    /// <summary>
    /// Class ColorCorrectionCurvesLayers.
    /// </summary>
    /// <seealso cref="M8.ImageEffects.PostEffectsBase" />
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Color Adjustments/Color Correction (Curves, Saturation) Layers")]
    public partial class ColorCorrectionCurvesLayers : PostEffectsBase
    {
        /// <summary>
        /// The red channel
        /// </summary>
        public AnimationCurve redChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// <summary>
        /// The green channel
        /// </summary>
        public AnimationCurve greenChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// <summary>
        /// The blue channel
        /// </summary>
        public AnimationCurve blueChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// <summary>
        /// The use depth correction
        /// </summary>
        public bool useDepthCorrection = false;

        /// <summary>
        /// The z curve
        /// </summary>
        public AnimationCurve zCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// <summary>
        /// The depth red channel
        /// </summary>
        public AnimationCurve depthRedChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// <summary>
        /// The depth green channel
        /// </summary>
        public AnimationCurve depthGreenChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// <summary>
        /// The depth blue channel
        /// </summary>
        public AnimationCurve depthBlueChannel = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        private Material ccMaterial;
        private Material ccDepthMaterial;
        private Material selectiveCcMaterial;

        private Texture2D rgbChannelTex;
        private Texture2D rgbDepthChannelTex;
        private Texture2D zCurveTex;

        /// <summary>
        /// The saturation
        /// </summary>
        public float saturation = 1.0f;

        /// <summary>
        /// The selective cc
        /// </summary>
        public bool selectiveCc = false;

        /// <summary>
        /// The selective from color
        /// </summary>
        public Color selectiveFromColor = Color.white;

        /// <summary>
        /// The selective to color
        /// </summary>
        public Color selectiveToColor = Color.white;

        /// <summary>
        /// The mode
        /// </summary>
        public ColorCorrectionMode mode;

        /// <summary>
        /// The update textures
        /// </summary>
        public bool updateTextures = true;

        /// <summary>
        /// The color correction curves shader
        /// </summary>
        public Shader colorCorrectionCurvesShader = null;

        /// <summary>
        /// The simple color correction curves shader
        /// </summary>
        public Shader simpleColorCorrectionCurvesShader = null;

        /// <summary>
        /// The color correction selective shader
        /// </summary>
        public Shader colorCorrectionSelectiveShader = null;

        private bool updateTexturesOnStartup = true;

        /// <summary>
        /// The exclude layers
        /// </summary>
        public LayerMask excludeLayers = 0;

        private GameObject tmpCam = null;
        private Camera _camera;

        private new void Start()
        {
            base.Start();
            updateTexturesOnStartup = true;
        }

        private void Awake()
        { }

        /// <summary>
        /// Checks the resources.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool CheckResources()
        {
            CheckSupport(mode == ColorCorrectionMode.Advanced);

            ccMaterial = CheckShaderAndCreateMaterial(simpleColorCorrectionCurvesShader, ccMaterial);
            ccDepthMaterial = CheckShaderAndCreateMaterial(colorCorrectionCurvesShader, ccDepthMaterial);
            selectiveCcMaterial = CheckShaderAndCreateMaterial(colorCorrectionSelectiveShader, selectiveCcMaterial);

            if (!rgbChannelTex)
                rgbChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
            if (!rgbDepthChannelTex)
                rgbDepthChannelTex = new Texture2D(256, 4, TextureFormat.ARGB32, false, true);
            if (!zCurveTex)
                zCurveTex = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);

            rgbChannelTex.hideFlags = HideFlags.DontSave;
            rgbDepthChannelTex.hideFlags = HideFlags.DontSave;
            zCurveTex.hideFlags = HideFlags.DontSave;

            rgbChannelTex.wrapMode = TextureWrapMode.Clamp;
            rgbDepthChannelTex.wrapMode = TextureWrapMode.Clamp;
            zCurveTex.wrapMode = TextureWrapMode.Clamp;

            if (!isSupported)
                ReportAutoDisable();
            return isSupported;
        }

        /// <summary>
        /// Updates the parameters.
        /// </summary>
        public void UpdateParameters()
        {
            CheckResources(); // textures might not be created if we're tweaking UI while disabled

            if (redChannel != null && greenChannel != null && blueChannel != null)
            {
                for (float i = 0.0f; i <= 1.0f; i += 1.0f / 255.0f)
                {
                    float rCh = Mathf.Clamp(redChannel.Evaluate(i), 0.0f, 1.0f);
                    float gCh = Mathf.Clamp(greenChannel.Evaluate(i), 0.0f, 1.0f);
                    float bCh = Mathf.Clamp(blueChannel.Evaluate(i), 0.0f, 1.0f);

                    rgbChannelTex.SetPixel((int)Mathf.Floor(i * 255.0f), 0, new Color(rCh, rCh, rCh));
                    rgbChannelTex.SetPixel((int)Mathf.Floor(i * 255.0f), 1, new Color(gCh, gCh, gCh));
                    rgbChannelTex.SetPixel((int)Mathf.Floor(i * 255.0f), 2, new Color(bCh, bCh, bCh));

                    float zC = Mathf.Clamp(zCurve.Evaluate(i), 0.0f, 1.0f);

                    zCurveTex.SetPixel((int)Mathf.Floor(i * 255.0f), 0, new Color(zC, zC, zC));

                    rCh = Mathf.Clamp(depthRedChannel.Evaluate(i), 0.0f, 1.0f);
                    gCh = Mathf.Clamp(depthGreenChannel.Evaluate(i), 0.0f, 1.0f);
                    bCh = Mathf.Clamp(depthBlueChannel.Evaluate(i), 0.0f, 1.0f);

                    rgbDepthChannelTex.SetPixel((int)Mathf.Floor(i * 255.0f), 0, new Color(rCh, rCh, rCh));
                    rgbDepthChannelTex.SetPixel((int)Mathf.Floor(i * 255.0f), 1, new Color(gCh, gCh, gCh));
                    rgbDepthChannelTex.SetPixel((int)Mathf.Floor(i * 255.0f), 2, new Color(bCh, bCh, bCh));
                }

                rgbChannelTex.Apply();
                rgbDepthChannelTex.Apply();
                zCurveTex.Apply();
            }
        }

        private void UpdateTextures()
        {
            UpdateParameters();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (CheckResources() == false)
            {
                Graphics.Blit(source, destination);
                return;
            }

            if (updateTexturesOnStartup)
            {
                UpdateParameters();
                updateTexturesOnStartup = false;
            }

            if (useDepthCorrection)
                GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;

            RenderTexture renderTarget2Use = destination;

            if (selectiveCc)
            {
                renderTarget2Use = RenderTexture.GetTemporary(source.width, source.height);
            }

            if (useDepthCorrection)
            {
                ccDepthMaterial.SetTexture("_RgbTex", rgbChannelTex);
                ccDepthMaterial.SetTexture("_ZCurve", zCurveTex);
                ccDepthMaterial.SetTexture("_RgbDepthTex", rgbDepthChannelTex);
                ccDepthMaterial.SetFloat("_Saturation", saturation);

                Graphics.Blit(source, renderTarget2Use, ccDepthMaterial);
            }
            else
            {
                ccMaterial.SetTexture("_RgbTex", rgbChannelTex);
                ccMaterial.SetFloat("_Saturation", saturation);

                Graphics.Blit(source, renderTarget2Use, ccMaterial);
            }

            if (selectiveCc)
            {
                selectiveCcMaterial.SetColor("selColor", selectiveFromColor);
                selectiveCcMaterial.SetColor("targetColor", selectiveToColor);
                Graphics.Blit(renderTarget2Use, destination, selectiveCcMaterial);

                RenderTexture.ReleaseTemporary(renderTarget2Use);
            }

            // exclude layers
            Camera cam = null;
            if (excludeLayers.value != 0) cam = GetTmpCam();

            if (cam && excludeLayers.value != 0)
            {
                cam.targetTexture = destination;
                cam.cullingMask = excludeLayers;
                cam.Render();
            }
        }

        private Camera GetTmpCam()
        {
            if (tmpCam == null)
            {
                if (_camera == null) _camera = GetComponent<Camera>();

                string name = "_" + _camera.name + "_ColorCorrectionTmpCam";
                GameObject go = GameObject.Find(name);

                if (null == go) // couldn't find, recreate
                {
                    tmpCam = new GameObject(name, typeof(Camera));
                }
                else
                {
                    tmpCam = go;
                }
            }

            tmpCam.hideFlags = HideFlags.DontSave;
            tmpCam.transform.position = _camera.transform.position;
            tmpCam.transform.rotation = _camera.transform.rotation;
            tmpCam.transform.localScale = _camera.transform.localScale;
            tmpCam.GetComponent<Camera>().CopyFrom(_camera);

            tmpCam.GetComponent<Camera>().enabled = false;
            tmpCam.GetComponent<Camera>().depthTextureMode = DepthTextureMode.None;
            tmpCam.GetComponent<Camera>().clearFlags = CameraClearFlags.Nothing;

            return tmpCam.GetComponent<Camera>();
        }
    }
}