using System.Reflection;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2API.Hepers.Unity_Extensions.Utils
{
    /// <summary>
    /// Class AppReflection.
    /// </summary>
    public static class AppReflection
    {
        /// <summary>
        /// Sets the field.
        /// </summary>
        /// <param name="inObj">The in object.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="newValue">The new value.</param>
        public static void SetField(this object inObj, string fieldName, object newValue)
        {
            FieldInfo info = inObj.GetType().GetField(fieldName);
            if (info != null)
                info.SetValue(inObj, newValue);
            else
                Debug.LogErrorFormat("The field '{0}' from '{1}' class isn't defined.", fieldName, inObj.GetType().Name);
        }

        /*internal static T GetReference<T>(this object inObj, string fieldName) where T : class
        {
            return GetField(inObj, fieldName, false) as T;
        }*/

        internal static T GetValue<T>(this object inObj, string fieldName) where T : struct
        {
            return (T)GetField(inObj, fieldName, false);
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="inObj">The in object.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>System.Object.</returns>
        public static object GetField(this object inObj, string fieldName)
        {
            return GetField(inObj, fieldName, false);
        }

        private static object GetField(this object inObj, string fieldName, bool fromProp = false)
        {
            if (inObj != null)
            {
                if (inObj.GetType().GetField(fieldName) != null)
                    return inObj.GetType().GetField(fieldName).GetValue(inObj);
                else if (inObj.GetType().GetProperty(fieldName) != null)
                    return inObj.GetType().GetProperty(fieldName).GetValue(inObj, null);
                else
                {
                    Debug.LogError("Trying to search the desired value as a property also didn't worked. You haven't declared any variable with this name.");
                    return null;
                }
            }
            else
            {
                Debug.LogWarning("Object passed is null to check a field from it!");
                return null;
            }
        }

        internal static T GetPropValue<T>(this object inObj, string propName)
        {
            return (T)GetProp(inObj, propName, false);
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="inObj">The in object.</param>
        /// <param name="propName">Name of the property.</param>
        /// <returns>System.Object.</returns>
        public static object GetProp(this object inObj, string propName)
        {
            return GetProp(inObj, propName, false);
        }

        private static object GetProp(this object inObj, string propName, bool fromField = false)
        {
            if (inObj != null)
            {
                if (inObj.GetType().GetProperty(propName) != null)
                    return inObj.GetType().GetProperty(propName).GetValue(inObj, null);
                else if (inObj.GetType().GetField(propName) != null)
                    return inObj.GetType().GetField(propName).GetValue(inObj);
                else
                {
                    Debug.LogError("Trying to search the desired value as a field also didn't worked. You haven't declared any variable with this name.");
                    return null;
                }
            }
            else
            {
                Debug.LogWarning("Object passed is null to check a field from it!");
                return null;
            }
        }
    }
}