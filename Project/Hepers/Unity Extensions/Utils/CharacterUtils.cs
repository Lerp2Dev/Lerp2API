using Lerp2API.Controllers.PersonView;
using Lerp2Assets.Characters.FirstPerson;
using Lerp2Assets.Characters.ThirdPerson;
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

        private static FirstPersonController fControl;

        private static ThirdPersonUserControl tControl;
        private static Animator anim;
        private static Rigidbody body;

        /// <summary>
        /// Gets a value indicating whether this instance is controlled.
        /// </summary>
        /// <value><c>true</c> if this instance is controlled; otherwise, <c>false</c>.</value>
        public static bool IsControlled
        {
            get
            {
                if (curView == PersonView.Third)
                    return tControl.enabled;
                else
                    return fControl.enabled;
            }
        }

        /// <summary>
        /// Initializes the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        public static void Init(Transform player)
        {
            Init(player.Find("FirstPerson").gameObject, player.Find("ThirdPerson").gameObject);
        }

        /// <summary>
        /// Initializes the specified first person.
        /// </summary>
        /// <param name="firstPerson">The first person.</param>
        /// <param name="thirdPerson">The third person.</param>
        public static void Init(GameObject firstPerson, GameObject thirdPerson)
        {
            if (firstPerson == null || thirdPerson == null)
            {
                Debug.LogError("Passed player is null.");
                return;
            }

            fControl = firstPerson.GetComponent<FirstPersonController>();

            tControl = thirdPerson.GetComponent<ThirdPersonUserControl>();
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
            if (fControl == null || tControl == null)
            {
                Debug.LogError("Player object is null.");
                return;
            }

            if (curView == PersonView.Third)
            {
                tControl.enabled = !tControl.enabled;
                anim.speed = tControl.enabled ? 1 : 0;
                if (!tControl.enabled)
                    anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0);
                body.constraints = tControl.enabled ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.FreezeAll;
            }
            else
                fControl.enabled = !fControl.enabled;
        }
    }
}