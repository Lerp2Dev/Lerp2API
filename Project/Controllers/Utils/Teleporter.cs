using UnityEngine;

namespace Lerp2API.Controllers.Utils
{
    /// <summary>
    /// Class Teleporter.
    /// </summary>
    public class Teleporter
    {
        /// <summary>
        /// Teleports the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="z">The z.</param>
        public static void Teleport(float x, float z)
        {
            _Teleport(x, null, z, null, null);
        }

        /// <summary>
        /// Teleports the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public static void Teleport(float x, float y, float z)
        {
            _Teleport(x, y, z, null, null);
        }

        /// <summary>
        /// Teleports the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="z">The z.</param>
        /// <param name="yaw">The yaw.</param>
        /// <param name="pitch">The pitch.</param>
        public static void Teleport(float x, float z, float yaw, float pitch)
        {
            _Teleport(x, null, z, yaw, pitch);
        }

        /// <summary>
        /// Teleports the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        /// <param name="yaw">The yaw.</param>
        /// <param name="pitch">The pitch.</param>
        public static void Teleport(float x, float y, float z, float yaw, float pitch)
        {
            _Teleport(x, y, z, yaw, pitch);
        }

        private static void _Teleport(object _x, object _y, object _z, object _yaw, object _pitch)
        {
            float x = (float)_x,
                  y = _y == null ? 0 : (float)_y,
                  z = (float)_z,
                  yaw = _yaw == null ? 0 : (float)_yaw,
                  pitch = _pitch == null ? 0 : (float)_pitch;

            //Get player with GameObject Player

            GameObject player = GameObject.FindGameObjectWithTag("Player");

            float posX = x,
                  posY = 0,
                  posZ = z;

            //Establecer x & z

            if (_y != null)
                posY = y;

            if (_yaw != null && _pitch != null)
            {
                //Girar cabeza
            }

            if (_y == null)
                FallingAvoider.m_current.StartCoroutine("FindGround", true);

            player.transform.position = new Vector3(posX, _y == null ? 100 : posY, posZ);
            Debug.LogFormat("Player teletransported to {0} successfully!", player.transform.position);
        }
    }
}