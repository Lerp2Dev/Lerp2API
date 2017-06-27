using Lerp2Assets.CrossPlatformInput;
using System;
using UnityEngine;

namespace Lerp2Assets.Characters.FirstPerson
{
    /// <summary>
    /// Class MouseLook.
    /// </summary>
    [Serializable]
    public class MouseLook
    {
        /// <summary>
        /// The x sensitivity
        /// </summary>
        public float XSensitivity = 2f;

        /// <summary>
        /// The y sensitivity
        /// </summary>
        public float YSensitivity = 2f;

        /// <summary>
        /// The clamp vertical rotation
        /// </summary>
        public bool clampVerticalRotation = true;

        /// <summary>
        /// The minimum x
        /// </summary>
        public float MinimumX = -90F;

        /// <summary>
        /// The maximum x
        /// </summary>
        public float MaximumX = 90F;

        /// <summary>
        /// The smooth
        /// </summary>
        public bool smooth;

        /// <summary>
        /// The smooth time
        /// </summary>
        public float smoothTime = 5f;

        /// <summary>
        /// The lock cursor
        /// </summary>
        public bool lockCursor = true;

        [NonSerialized]
        private Quaternion m_CharacterTargetRot = Quaternion.identity;

        [NonSerialized]
        private Quaternion m_CameraTargetRot = Quaternion.identity;

        private bool m_cursorIsLocked = true;

        /// <summary>
        /// Initializes the specified character.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="camera">The camera.</param>
        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }

        /// <summary>
        /// Looks the rotation.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="camera">The camera.</param>
        public void LookRotation(Transform character, Transform camera)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }

            UpdateCursorLock();
        }

        /// <summary>
        /// Sets the cursor lock.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if (!lockCursor)
            { //we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        /// <summary>
        /// Updates the cursor lock.
        /// </summary>
        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
                m_cursorIsLocked = false;
            else if (Input.GetMouseButtonUp(0))
                m_cursorIsLocked = true;

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}