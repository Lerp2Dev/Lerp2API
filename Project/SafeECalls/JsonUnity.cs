using Lerp2API._Debug;
using Lerp2API.Hepers.JSON_Extensions;
using System;

namespace Lerp2API.SafeECalls
{ //Use this in case, you can't reference UnityEngine
  /// <summary>
  /// Class JsonUtility.
  /// </summary>
  /// <seealso cref="Lerp2API.SafeECalls.SafeECall" />
    public class JsonUtility : SafeECall
    {
        /// <summary>
        /// To the json.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.String.</returns>
        public static string ToJson(object obj)
        {
            try
            {
                if (!LerpedCore.safeECallEnabled)
                    return UnityEngine.JsonUtility.ToJson(obj);
                else
                    return JSONHelpers.Serialize(obj);
            }
            catch
            {
                LerpedCore.logger.LogError(error);
                Debug.WriteSafeStacktrace();
                return "";
            }
        }

        /// <summary>
        /// To the json.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="prettyPrint">if set to <c>true</c> [pretty print].</param>
        /// <returns>System.String.</returns>
        public static string ToJson(object obj, bool prettyPrint)
        {
            try
            {
                if (!LerpedCore.safeECallEnabled)
                    return UnityEngine.JsonUtility.ToJson(obj, prettyPrint);
                else
                    return JSONHelpers.Serialize(obj, prettyPrint);
            }
            catch
            {
                LerpedCore.logger.LogError(error);
                Debug.WriteSafeStacktrace();
                return "";
            }
        }

        /// <summary>
        /// Froms the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns>T.</returns>
        public static T FromJson<T>(string json)
        {
            try
            {
                if (!LerpedCore.safeECallEnabled)
                    return UnityEngine.JsonUtility.FromJson<T>(json);
                else
                    return JSONHelpers.Deserialize<T>(json);
            }
            catch
            {
                LerpedCore.logger.LogError(error);
                Debug.WriteSafeStacktrace();
                return default(T);
            }
        }

        /// <summary>
        /// Froms the json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>System.Object.</returns>
        public static object FromJson(string json)
        {
            try
            {
                if (!LerpedCore.safeECallEnabled)
                    return UnityEngine.JsonUtility.FromJson<object>(json);
                else
                    return JSONHelpers.Deserialize<object>(json);
            }
            catch
            {
                LerpedCore.logger.LogError(error);
                Debug.WriteSafeStacktrace();
                return null;
            }
        }

        //Non-equivalent solution
        /*public static void FromJsonOverwrite(string json, object objectToOverwrite)
        {
            JsonUtility.FromJsonOverwrite(json, objectToOverwrite);
        }*/
    }
}
