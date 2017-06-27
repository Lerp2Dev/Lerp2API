using Lerp2API.Hepers.Unity_Extensions;
using System.Collections;
using UnityEngine;

namespace Lerp2API.Effects._Text.Controllers
{
    /// <summary>
    /// Class TextGuideController.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class TextGuideController : MonoBehaviour
    {
        /// <summary>
        /// The transition started
        /// </summary>
        [HideInInspector]
        public bool transitionStarted;

        private bool breakAnim, isShowed;
        private Transform plane;

        /// <summary>
        /// The text guide
        /// </summary>
        public TextGuideBase txtGuide;

        /// <summary>
        /// Gets a value indicating whether [int showed].
        /// </summary>
        /// <value><c>true</c> if [int showed]; otherwise, <c>false</c>.</value>
        public bool intShowed
        {
            get
            {
                return transform.localScale == txtGuide.mainStoredScale;
            }
        }

        // Use this for initialization
        private void Start()
        {
            plane = transform.Find("Plane");

            InvokeRepeating("CheckScale", 0, 1);

            if (Camera.main == null)
                Debug.LogWarning("You must set your main Camera, if not, you will have some 'incomplete results', click this message and know why.");
        }

        // Update is called once per frame
        private void Update()
        {
            if (Camera.main != null)
                transform.VerticalLookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
        }

        private void CheckScale()
        {
            if (!txtGuide.freezeVisibility)
            {
                if (intShowed != isShowed)
                    StartCoroutine(ToggleVisibility(isShowed));
                else
                    isShowed = txtGuide.casted;
            }
        }

        /// <summary>
        /// News the instance.
        /// </summary>
        /// <param name="tg">The tg.</param>
        /// <returns>TextGuideController.</returns>
        public TextGuideController NewInstance(TextGuideBase tg)
        {
            txtGuide = tg;
            return this;
        }

        /// <summary>
        /// Breaks the animation.
        /// </summary>
        /// <param name="showed">if set to <c>true</c> [showed].</param>
        public void BreakAnimation(bool showed)
        {
            breakAnim = true;
        }

        /// <summary>
        /// Shows this instance.
        /// </summary>
        /// <returns>IEnumerator.</returns>
        public IEnumerator Show()
        {
            StartCoroutine(ToggleVisibility(true));
            yield return null;
        }

        /// <summary>
        /// Hides this instance.
        /// </summary>
        /// <returns>IEnumerator.</returns>
        public IEnumerator Hide()
        {
            StartCoroutine(ToggleVisibility(false));
            yield return null;
        }

        private IEnumerator ToggleVisibility(bool showed)
        {
            transitionStarted = true;
            float frames = txtGuide.animSecs / Time.fixedDeltaTime,
                  step = 1 / frames, // or fontSize / frames
                  intScale = showed ? 0 : 1;
            int i = 0;

            while (i < frames)
            {
                if (showed)
                    intScale += step;
                else
                    intScale -= step;
                if (breakAnim)
                    break;
                yield return new WaitForFixedUpdate();
                transform.localScale = txtGuide.mainStoredScale * intScale;
                ++i;
            }
            transitionStarted = false;
            if (breakAnim)
            {
                transform.localScale = showed ? txtGuide.mainStoredScale : Vector3.zero;
                breakAnim = false;
            }

            isShowed = showed;
        }
    }
}