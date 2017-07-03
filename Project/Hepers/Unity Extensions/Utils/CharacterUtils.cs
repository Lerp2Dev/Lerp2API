using Lerp2API.Controllers.PersonView;
using UnityEngine;

namespace Lerp2API.Hepers.Unity_Extensions.Utils
{
    /// <summary>
    /// Class CharacterUtils.
    /// </summary>
    public class CharacterUtils : PersonViewToggle //This is a MonoBehaviour, (I have to) not to make it attachable
    {
        /// <summary>
        /// The is initialize
        /// </summary>
        public static bool isInit;

        private static MonoBehaviour fControl
        {
            get
            {
                if (playerRef != null)
                    return (MonoBehaviour)firstPerson.GetComponent("FirstPersonController");
                return (MonoBehaviour)player.GetComponent("FirstPersonController");
            }
        }

        private static MonoBehaviour tControl
        {
            get
            {
                if (playerRef != null)
                    return (MonoBehaviour)thirdPerson.GetComponent("ThirdPersonUserControl");
                return (MonoBehaviour)PlayerUtils.player.GetComponent("ThirdPersonUserControl");
            }
        }

        private static Animator anim;
        private static Rigidbody body;

        private static bool freezed;
        private static Transform playerRef;

        private static GameObject firstPerson,
                                  thirdPerson;

        /// <summary>
        /// Gets a value indicating whether this instance is controlled.
        /// </summary>
        /// <value><c>true</c> if this instance is controlled; otherwise, <c>false</c>.</value>
        public static bool IsControlled
        {
            get
            {
                return !freezed;
            }
        }

        /// <summary>
        /// Initializes the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        public static void Init(Transform player = null)
        {
            if (player == null)
                player = PlayerUtils.player.transform;

            playerRef = player;

            firstPerson = player.Find("FirstPerson").gameObject;
            thirdPerson = player.Find("ThirdPerson").gameObject;

            if (!(player != null && firstPerson != null && thirdPerson != null))
            {
                Debug.LogError("Passed player is null.");
                return;
            }

            anim = thirdPerson.GetComponent<Animator>();
            body = thirdPerson.GetComponent<Rigidbody>();

            isInit = true; //Por si las excepciones...
        }

        /// <summary>
        /// Toggles the control.
        /// </summary>
        public static void ToggleControl()
        {
            //Tengo que parar el movimiento. O poner la animación por el primer frame, que viene a ser lo mismo (I have to)
            //Ya no hace falta salirse de aqui si los controladores son nulos, porque el tControl y el fControl se obtiene de otra forma, de hecho, en el asset de Ballisitic Physics ya le puedo meter el script de los Standard Assets

            freezed = !freezed;

            PersonView view = playerRef == null ? PlayerUtils.GetCurView() : curView;

            if (view == PersonView.Third)
            {
                tControl.enabled = !freezed;
                anim.speed = freezed ? 1 : 0;
                if (!freezed)
                    anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0);
                body.constraints = freezed ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeAll;
            }
            else
                fControl.enabled = !freezed;
        }
    }
}