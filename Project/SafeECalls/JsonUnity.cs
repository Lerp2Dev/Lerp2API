using Lerp2API.DebugHandler;
using System;

namespace Lerp2API.SafeECalls
{ //Use this in case, you can't reference UnityEngine
    public class JsonUtility : SafeECall
    {
        public static string ToJson(object obj)
        {
            try
            {
                if (!LerpedCore.safeECallEnabled)
                    return UnityEngine.JsonUtility.ToJson(obj);
                else
                    return JSONHelpers.Serialize(obj);
            }
            catch (Exception ex)
            {
                LerpedCore.logger.LogError(error);
                Debug.WriteSafeStacktrace();
                return "";
            }
        }

        public static string ToJson(object obj, bool prettyPrint)
        {
            try
            {
                if (!LerpedCore.safeECallEnabled)
                    return UnityEngine.JsonUtility.ToJson(obj, prettyPrint);
                else
                    return JSONHelpers.Serialize(obj, prettyPrint);
            }
            catch (Exception ex)
            {
                LerpedCore.logger.LogError(error);
                Debug.WriteSafeStacktrace();
                return "";
            }
        }

        public static T FromJson<T>(string json)
        {
            try
            {
                if (!LerpedCore.safeECallEnabled)
                    return UnityEngine.JsonUtility.FromJson<T>(json);
                else
                    return JSONHelpers.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                LerpedCore.logger.LogError(error);
                Debug.WriteSafeStacktrace();
                return default(T);
            }
        }

        public static object FromJson(string json)
        {
            try
            {
                if (!LerpedCore.safeECallEnabled)
                    return UnityEngine.JsonUtility.FromJson<object>(json);
                else
                    return JSONHelpers.Deserialize<object>(json);
            }
            catch (Exception ex)
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
