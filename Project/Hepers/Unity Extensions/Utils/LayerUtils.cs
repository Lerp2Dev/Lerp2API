using UnityEngine;

namespace Lerp2API.Hepers.Unity_Extensions.Utils
{
    /// <summary>
    /// Class LayerUtils.
    /// </summary>
    public static class LayerUtils
    {
        /// <summary>
        /// Alls the layers.
        /// </summary>
        /// <param name="cullingMask">The culling mask.</param>
        /// <param name="layers">The layers.</param>
        /// <returns>System.Int32.</returns>
        public static int AllLayers(this int cullingMask, params string[] layers)
        {
            foreach (string layer in layers)
                cullingMask |= 1 << LayerMask.NameToLayer(layer);
            return cullingMask;
        }

        /// <summary>
        /// Called when [layer].
        /// </summary>
        /// <param name="cullingMask">The culling mask.</param>
        /// <param name="layer">The layer.</param>
        /// <returns>System.Int32.</returns>
        public static int OneLayer(this int cullingMask, string layer)
        {
            cullingMask |= 1 << LayerMask.NameToLayer(layer);
            return cullingMask;
        }

        /// <summary>
        /// Excepts the layers.
        /// </summary>
        /// <param name="cullingMask">The culling mask.</param>
        /// <param name="layers">The layers.</param>
        /// <returns>System.Int32.</returns>
        public static int ExceptLayers(this int cullingMask, params string[] layers)
        {
            foreach (string layer in layers)
                cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
            return cullingMask;
        }

        /// <summary>
        /// Excepts the one layer.
        /// </summary>
        /// <param name="cullingMask">The culling mask.</param>
        /// <param name="layer">The layer.</param>
        /// <returns>System.Int32.</returns>
        public static int ExceptOneLayer(this int cullingMask, string layer)
        {
            cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
            return cullingMask;
        }

        /// <summary>
        /// Multis the xor layers.
        /// </summary>
        /// <param name="cullingMask">The culling mask.</param>
        /// <param name="layers">The layers.</param>
        /// <returns>System.Int32.</returns>
        public static int MultiXORLayers(this int cullingMask, params string[] layers)
        {
            foreach (string layer in layers)
                cullingMask ^= 1 << LayerMask.NameToLayer(layer);
            return cullingMask;
        }

        /// <summary>
        /// Singles the xor layer.
        /// </summary>
        /// <param name="cullingMask">The culling mask.</param>
        /// <param name="layer">The layer.</param>
        /// <returns>System.Int32.</returns>
        public static int SingleXORLayer(this int cullingMask, string layer)
        {
            cullingMask ^= 1 << LayerMask.NameToLayer(layer);
            return cullingMask;
        }

        //Physics.DefaultRaycastLayers

        /// <summary>
        /// Alls the layers.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <returns>System.Int32.</returns>
        public static int AllLayers(params string[] layers)
        {
            return AllLayers(Physics.DefaultRaycastLayers, layers);
        }

        /// <summary>
        /// Called when [layer].
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>System.Int32.</returns>
        public static int OneLayer(string layer)
        {
            return OneLayer(Physics.DefaultRaycastLayers, layer);
        }

        /// <summary>
        /// Excepts the layers.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <returns>System.Int32.</returns>
        public static int ExceptLayers(params string[] layers)
        {
            return ExceptLayers(Physics.DefaultRaycastLayers, layers);
        }

        /// <summary>
        /// Excepts the one layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>System.Int32.</returns>
        public static int ExceptOneLayer(string layer)
        {
            return ExceptOneLayer(Physics.DefaultRaycastLayers, layer);
        }

        /// <summary>
        /// Multis the xor layers.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <returns>System.Int32.</returns>
        public static int MultiXORLayers(params string[] layers)
        {
            return MultiXORLayers(Physics.DefaultRaycastLayers, layers);
        }

        /// <summary>
        /// Singles the xor layer.
        /// </summary>
        /// <param name="layer">The layer.</param>
        /// <returns>System.Int32.</returns>
        public static int SingleXORLayer(string layer)
        {
            return SingleXORLayer(Physics.DefaultRaycastLayers, layer);
        }
    }
}