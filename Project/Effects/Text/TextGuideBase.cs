using Lerp2API.Controllers._Canvas;
using Lerp2API.Effects._Text.Controllers;
using UnityEngine;
using UnityEngine.UI;
using Debug = Lerp2API._Debug.Debug;
using Object = UnityEngine.Object;

namespace Lerp2API.Effects._Text
{
    /// <summary>
    /// Class TextGuideBase.
    /// </summary>
    public class TextGuideBase
    {
        //Public attr
        /// <summary>
        /// The parent
        /// </summary>
        public Transform parent;

        /// <summary>
        /// The text object
        /// </summary>
        public GameObject textObj, backPlane;

        /// <summary>
        /// The text
        /// </summary>
        public string text, neededTag;

        /// <summary>
        /// The anim secs
        /// </summary>
        public float animSecs = 1;

        //public Color backColor = new Color(0, 0, 0, .5f);
        /// <summary>
        /// The back stored scale
        /// </summary>
        public Vector3 backStoredScale = Vector3.one,
                   /// <summary>
                   /// The main stored scale
                   /// </summary>
                   mainStoredScale = Vector3.one;

        /// <summary>
        /// The freeze visibility
        /// </summary>
        public bool freezeVisibility,
                    showPlane;

        /// <summary>
        /// The guide
        /// </summary>
        public TextGuideController guide;

        //Private attrs
        private bool _showing;

        private float textWidth = 50;
        private TextGuideController controller;
        private Text txtGuide;
        private CircleOutline outline;
        private Vector2 txtSize;

        //Props
        /// <summary>
        /// Gets or sets a value indicating whether this instance is showed.
        /// </summary>
        /// <value><c>true</c> if this instance is showed; otherwise, <c>false</c>.</value>
        public bool isShowed
        {
            get
            {
                return _showing;
            }
            set
            {
                _showing = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TextGuideBase"/> is casted.
        /// </summary>
        /// <value><c>true</c> if casted; otherwise, <c>false</c>.</value>
        public bool casted
        {
            get
            {
                if (Camera.main != null)
                {
                    //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 5, Color.red);
                    Transform camform = Camera.main.transform.parent != null ? Camera.main.transform.parent : Camera.main.transform; //I have to revisar esto, porque no tengo ni idea de porque lo hice... 26 días han pasado, normal xd
                    RaycastHit hit;
                    return Physics.Raycast(camform.position, camform.forward, out hit, 3) && (!string.IsNullOrEmpty(neededTag) && hit.transform.tag == neededTag || string.IsNullOrEmpty(neededTag));
                }
                return true; //Always visible.
            }
        }

        //Advanced case
        /// <summary>
        /// Sets the default values.
        /// </summary>
        /// <param name="tg">The tg.</param>
        /// <returns>TextGuideBase.</returns>
        public static TextGuideBase SetDefaultValues(TextGuideBase tg)
        {
            tg.PlaneColor = new Color(0, 0, 0, .5f);
            tg.OutlineColor = Color.black;
            tg.OutlineSize = new Vector2(1, 1);

            return tg;
        }

        /// <summary>
        /// Gets or sets the color of the outline.
        /// </summary>
        /// <value>The color of the outline.</value>
        public Color OutlineColor
        {
            set
            {
                if (outline != null)
                    outline.effectColor = value;
            }
            get
            {
                if (outline != null)
                    return outline.effectColor;
                else
                {
                    Debug.LogError("Outline element cannot be found!");
                    return default(Color);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size of the outline.
        /// </summary>
        /// <value>The size of the outline.</value>
        public Vector2 OutlineSize
        {
            set
            {
                Vector2 size = value;
                if (size.x > 5)
                    size.x = 5;
                if (size.y > 5)
                    size.y = 5;
                if (outline != null)
                {
                    outline.effectDistance = size;
                    outline.circleCount = (int)size.x + (int)size.y;
                }
            }
            get
            {
                if (outline != null)
                    return outline.effectDistance;
                else
                {
                    Debug.LogError("Outline element cannot be found!");
                    return default(Vector2);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the plane.
        /// </summary>
        /// <value>The color of the plane.</value>
        public Color PlaneColor
        {
            set
            {
                if (showPlane && backPlane != null && backPlane.GetComponent<MeshRenderer>() != null)
                    backPlane.GetComponent<MeshRenderer>().material.color = value;
                else
                    Debug.LogError("There was a problem with the background plane!");
            }
            get
            {
                if (backPlane != null && backPlane.GetComponent<MeshRenderer>() != null)
                    return backPlane.GetComponent<MeshRenderer>().material.color;
                else
                {
                    Debug.LogError("There was a problem with the background plane!");
                    return default(Color);
                }
            }
        }

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TextGuideBase"/> class.
        /// </summary>
        /// <param name="pa">The pa.</param>
        /// <param name="po">The po.</param>
        /// <param name="sc">The sc.</param>
        /// <param name="sz">The sz.</param>
        /// <param name="te">The te.</param>
        /// <param name="tg">The tg.</param>
        /// <param name="sp">if set to <c>true</c> [sp].</param>
        /// <param name="ans">The ans.</param>
        public TextGuideBase(Transform pa, Vector3 po, Vector3 sc, Vector2 sz, string te, string tg, bool sp = true, float ans = 1)
        {
            //Set the local variables...
            parent = pa;
            text = te;
            neededTag = tg;
            showPlane = sp;
            animSecs = ans;
            txtSize = sz;
            mainStoredScale = sc;

            //Create the gameobject, the text and the background...
            textObj = CanvasController.AddTextToCanvas(CanvasController.CreateCanvas("TextGuideBase - Canvas"), txtSize);
            controller = textObj.AddComponent<TextGuideController>();
            txtGuide = textObj.transform.Find("Text Render").gameObject.GetComponent<Text>();
            textObj.transform.SetParent(pa);

            //Create the outline
            outline = txtGuide.gameObject.AddComponent<CircleOutline>();

            //Create a new instance for the controller
            guide = controller.NewInstance(this);

            //Set the text of the guide
            txtGuide.text = text;

            //If we need to show the background, we proceed...
            if (showPlane)
            { //By setting, position, rotation and scale.
                backStoredScale = new Vector3(sz.x, 10, sz.y) / 10;

                backPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                backPlane.transform.localPosition = Vector3.right;
                backPlane.transform.localEulerAngles = new Vector3(-90, 0, 0);
                backPlane.transform.localScale = backStoredScale;

                //Destroy the collider of the background
                Object.Destroy(backPlane.GetComponent<MeshCollider>());

                //Get the renderer and edit it.
                MeshRenderer ren = backPlane.GetComponent<MeshRenderer>();

                ren.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                ren.receiveShadows = false;

                //Generate the new material
                Material mat = new Material(Shader.Find("Transparent/Diffuse"));
                //mat.color = backColor;

                ren.material = mat; //Set the new material

                backPlane.transform.parent = textObj.transform;
            }

            //Set the text position and scale.
            textObj.transform.localPosition = po;
            textObj.transform.localScale = mainStoredScale;
            textObj.transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Toggles the visibility.
        /// </summary>
        public void ToggleVisibility()
        {
            if (isShowed != casted)
            { //Wait until coroutine is finished
                if (animSecs > 0)
                {
                    if (!controller.transitionStarted)
                    {
                        if (casted)
                            controller.StartCoroutine("Show");
                        else
                            controller.StartCoroutine("Hide");
                    }
                    else
                        controller.BreakAnimation(casted);
                }
                else
                    textObj.transform.localScale = casted ? mainStoredScale : Vector3.zero;

                isShowed = casted;
            }
        }

        //If the visibility is freezed we won't have any problem.
        /// <summary>
        /// Shows the specified update state.
        /// </summary>
        /// <param name="updateState">if set to <c>true</c> [update state].</param>
        public void Show(bool updateState = true)
        {
            if (updateState) isShowed = true;
            if (animSecs > 0)
                controller.StartCoroutine("Show");
            else
                textObj.transform.localScale = mainStoredScale;
        }

        /// <summary>
        /// Hides the specified update state.
        /// </summary>
        /// <param name="updateState">if set to <c>true</c> [update state].</param>
        public void Hide(bool updateState = true)
        {
            if (updateState) isShowed = false;
            if (animSecs > 0)
                controller.StartCoroutine("Hide");
            else
                textObj.transform.localScale = Vector3.zero;
        }
    }
}