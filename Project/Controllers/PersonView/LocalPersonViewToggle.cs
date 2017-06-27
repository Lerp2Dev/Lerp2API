using Lerp2API.Hepers.Unity_Extensions;
using Lerp2API.Hepers.Unity_Extensions.Utils;
using UnityEngine;

namespace Lerp2API.Controllers.PersonView
{
    /// <summary>
    /// Class LocalPersonViewToggle.
    /// Toggle Camera View in a local component, not used in the player
    /// </summary>
    public class LocalPersonViewToggle : PersonViewToggle
    {
        /// <summary>
        /// The local object position
        /// </summary>
        public Vector3 localObjPos,
                   /// <summary>
                   /// The local object rot
                   /// </summary>
                   localObjRot;

        /// <summary>
        /// The t local object
        /// </summary>
        [HideInInspector]
        public Transform t_localObj;

        private int defaultCullingMask;

        /// <summary>
        /// Gets the world object position.
        /// </summary>
        /// <value>The world object position.</value>
        public Vector3 worldObjPos
        {
            get
            {
                if (t_localObj != null && localObjPos != default(Vector3))
                    return t_localObj.TransformPoint(localObjPos);
                return default(Vector3);
            }
        }

        /// <summary>
        /// Initializes the specified go.
        /// </summary>
        /// <param name="go">The go.</param>
        /// <param name="map">The map.</param>
        /// <returns>LocalPersonViewToggle.</returns>
        public static LocalPersonViewToggle Init(GameObject go, KeyMap map = null)
        {
            LocalPersonViewToggle lpvt = go.GetComponent<LocalPersonViewToggle>();

            if (lpvt == null)
                lpvt = go.AddComponent<LocalPersonViewToggle>();

            if (map != null)
            {
                lpvt.changeView = map["changeView"];
            }

            lpvt.t_localObj = go.transform;

            lpvt.defaultCullingMask = Camera.main.cullingMask;

            if (!CharacterUtils.isInit)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                    CharacterUtils.Init(player.transform);
            }

            return lpvt;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (localObjPos == default(Vector3) || localObjRot == default(Vector3))
                Debug.LogWarning(string.Format("You must set positions of the person view script from '{0}'.", name));
            if (!CharacterUtils.isInit)
                Debug.LogWarning("CharacterUtils must be inited to be used by LocalPersonViewToggle");
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            if (isLocal && Input.GetKeyDown(changeView))
                ChangeView();
        }

        private void ChangeView()
        {
            //Tenemos que jugar con el gameobject del camera.main y decirle la posición que tiene que seguir relativa al objeto en cuestion.
            if (isLocal) //The view controller is available.
                Toggle(() =>
                {
                    StartCoroutine(UnityHelpers.FadePosition(Camera.main.transform, curView != PersonView.First ? worldObjPos : thirdPersonCameraPos, curView != PersonView.First ? firstPersonCameraPos : worldObjPos, 2, false, () =>
                    {
                        //I have to arreglar que la camara se mueva mucho mientras giro la torreta...

                        if (curView == PersonView.First)
                            RecheckChars();
                        Camera.main.transform.parent = lastView == PersonView.First ? t_FirstPerson : t_localObj;
                        if (curView == PersonView.Third)
                            RecheckChars();

                        Camera.main.transform.localPosition = curView == PersonView.First ? localObjPos : thirdPersonCameraLocalPos;
                        Camera.main.transform.localEulerAngles = curView == PersonView.First ? localObjRot : thirdPersonCameraLocalRot;
                        Camera.main.cullingMask = curView == PersonView.Third ? defaultCullingMask : Camera.main.cullingMask.ExceptOneLayer("Player");

                        if (CharacterUtils.IsControlled) //If the player forgets that is being controlled by something we have to lend our controls.
                            CharacterUtils.ToggleControl();
                    }));
                });
        }
    }
}