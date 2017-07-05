using Lerp2API.Hepers.Unity_Extensions.Utils;
using System;
using TeamUtility.IO;
using UnityEngine;

namespace Lerp2API.Controllers.PersonView
{
    /// <summary>
    /// Class PersonViewToggle.
    /// </summary>
    public partial class PersonViewToggle : MonoBehaviour
    {  //Esto lo deberiamos setear al principio (I have to), porque si no no se sabrá nunca si empezamos por defecto con la primera o la tercera persona, lo suyo sería hacerlo desde el LerpedCore, y que esté se encargue de configurar el playr si hay.
        //[NotInheritable]
        /// <summary>
        /// Me
        /// </summary>
        public static PersonViewToggle me; //Only this class

        internal static Transform _firstPersonObj,
                                  _thirdPersonObj;

        internal static GameObject player
        {
            get
            {
                GameObject _player = GameObject.FindGameObjectWithTag("Player");
                if (_player == null)
                    Debug.LogWarning("Player tag is null.");
                return _player;
            }
        }

        /// <summary>
        /// Gets the t first person.
        /// </summary>
        /// <value>The t first person.</value>
        public static Transform t_FirstPerson
        {
            get
            {
                if (player == null)
                    return null;
                return player.transform.Find("FirstPerson");
            }
        }

        /// <summary>
        /// Gets the t third person.
        /// </summary>
        /// <value>The t third person.</value>
        public static Transform t_ThirdPerson
        {
            get
            {
                if (player == null)
                    return null;
                return player.transform.Find("ThirdPerson");
            }
        }

        /// <summary>
        /// Gets the o first person.
        /// </summary>
        /// <value>The o first person.</value>
        public static GameObject o_FirstPerson
        {
            get
            {
                return t_FirstPerson.gameObject;
            }
        }

        /// <summary>
        /// Gets the o third person.
        /// </summary>
        /// <value>The o third person.</value>
        public static GameObject o_ThirdPerson
        {
            get
            {
                return t_ThirdPerson.gameObject;
            }
        }

        //This need to be static, because, we only have a player, a and local object that be controlled at the same time.
        /// <summary>
        /// The is local
        /// </summary>
        public static bool isLocal;

        /// <summary>
        /// The current view
        /// </summary>
        public static PersonView curView = PersonView.Third,
                             /// <summary>
                             /// The last view
                             /// </summary>
                             lastView;

        /// <summary>
        /// The change view
        /// </summary>
        protected KeyCode changeView = KeyCode.V;

        /// <summary>
        /// The first person camera position
        /// </summary>
        protected static Vector3 firstPersonCameraPos,
                                 /// <summary>
                                 /// The third person camera position
                                 /// </summary>
                                 thirdPersonCameraPos,
                                 /// <summary>
                                 /// The first person camera rot
                                 /// </summary>
                                 firstPersonCameraRot,
                                 /// <summary>
                                 /// The third person camera rot
                                 /// </summary>
                                 thirdPersonCameraRot,
                                 /// <summary>
                                 /// The first person camera local position
                                 /// </summary>
                                 firstPersonCameraLocalPos,
                                 /// <summary>
                                 /// The third person camera local position
                                 /// </summary>
                                 thirdPersonCameraLocalPos,
                                 /// <summary>
                                 /// The first person camera local rot
                                 /// </summary>
                                 firstPersonCameraLocalRot,
                                 /// <summary>
                                 /// The third person camera local rot
                                 /// </summary>
                                 thirdPersonCameraLocalRot;

        /// <summary>
        /// Toggles the control.
        /// </summary>
        /// <param name="local">The local.</param>
        public static void ToggleControl(LocalPersonViewToggle local)
        {
            isLocal = !isLocal;

            if (isLocal)
            { //Cogiendo
                Camera.main.transform.parent = curView == PersonView.First ? local.t_localObj : t_ThirdPerson;
                Camera.main.transform.localPosition = curView == PersonView.First ? local.localObjPos : thirdPersonCameraLocalPos;
                Camera.main.transform.localEulerAngles = curView == PersonView.First ? local.localObjRot : thirdPersonCameraLocalRot;
            }
            else
            { //Dejando
                Camera.main.transform.parent = curView == PersonView.First ? t_FirstPerson : t_ThirdPerson;
                Camera.main.transform.localPosition = curView == PersonView.First ? firstPersonCameraLocalPos : thirdPersonCameraLocalPos;
                Camera.main.transform.localEulerAngles = curView == PersonView.First ? firstPersonCameraLocalRot : thirdPersonCameraLocalRot;
            }

            CharacterUtils.ToggleControl(); //Esto lo llamamos despues por si el ReturnCam hace un switch
            lastView = curView;
        }

        internal static void SwitchView()
        {
            lastView = curView;
            curView = curView == PersonView.First ? PersonView.Third : PersonView.First;
        }

        internal static void Toggle(Action a)
        {
            SwitchView();
            a();
        }

        /// <summary>
        /// Rechecks the chars.
        /// </summary>
        public static void RecheckChars()
        {
            o_FirstPerson.SetActive(curView == PersonView.First);
            o_ThirdPerson.gameObject.SetActive(curView == PersonView.Third);
        }

        // Use this for initialization
        private void Start()
        {
            me = this; //There is only one by player, so, this is right.

            //Get the positions & rotations.
            if (curView == PersonView.First)
                o_ThirdPerson.SetActive(true);
            else
                o_FirstPerson.SetActive(true);

            Camera firstCamera = o_FirstPerson.GetComponentInChildren<Camera>(),
                   thirdCamera = o_ThirdPerson.GetComponentInChildren<Camera>();

            firstPersonCameraPos = firstCamera.transform.position;
            thirdPersonCameraPos = thirdCamera.transform.position;
            firstPersonCameraRot = firstCamera.transform.eulerAngles;
            thirdPersonCameraRot = thirdCamera.transform.eulerAngles;

            firstPersonCameraLocalPos = firstCamera.transform.localPosition;
            thirdPersonCameraLocalPos = thirdCamera.transform.localPosition;
            firstPersonCameraLocalRot = firstCamera.transform.localEulerAngles;
            thirdPersonCameraLocalRot = thirdCamera.transform.localEulerAngles;

            if (curView == PersonView.First)
                o_ThirdPerson.SetActive(false);
            else
                o_FirstPerson.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            if (!isLocal && InputManager.GetKeyDown(changeView)) //If the local script hasn't take the permission, by default you can change it.
                ToggleView();
        }

        /// <summary>
        /// Toggles the view.
        /// </summary>
        public static void ToggleView()
        {
            Toggle(() =>
            {
                if (curView == PersonView.First)
                    t_FirstPerson.position = t_ThirdPerson.position;
                else
                { //Litle fixes.
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    t_ThirdPerson.position = t_FirstPerson.position;

                    //Vector3 pos = firstPerson.transform.position;
                    //thirdPerson.transform.position = new Vector3(pos.x, pos.y - firstPerson.GetComponent<CharacterController>().height / 2 + thirdPerson.GetComponent<CapsuleCollider>().height / 2, pos.z); //No es que sea muy genérico...
                }

                RecheckChars();
                if (!CharacterUtils.IsControlled)
                    CharacterUtils.ToggleControl();
            });
        }
    }
}