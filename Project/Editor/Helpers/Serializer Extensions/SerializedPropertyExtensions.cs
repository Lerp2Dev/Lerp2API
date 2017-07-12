using sp = UnityEditor.SerializedProperty;
using UnityEditor;
using UnityEngine;

namespace Lerp2API.Hepers.Serializer_Extensions
{
    /// <summary>
    /// Class SerializedPropertyExtensions.
    /// </summary>
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="value">The value.</param>
        public static void Add(this sp prop, Object value)
        {
            ++prop.arraySize;
            prop.GetAt(prop.arraySize - 1).objectReferenceValue = value;
        }

        /// <summary>
        /// Gets at.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="i">The i.</param>
        /// <returns>sp.</returns>
        public static sp GetAt(this sp prop, int i)
        {
            try
            {
                return prop.GetArrayElementAtIndex(i);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Sets the object value at.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="i">The i.</param>
        /// <param name="toValue">To value.</param>
        public static void SetObjectValueAt(this sp prop, int i, System.Object toValue)
        {
            prop.GetAt(i).SetObjectValue(toValue);
        }

        /// <summary>
        /// Sets the object value.
        /// </summary>
        /// <param name="prop">The property.</param>
        /// <param name="toValue">To value.</param>
        public static void SetObjectValue(this sp prop, System.Object toValue)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    prop.boolValue = (bool)toValue;
                    break;

                case SerializedPropertyType.Bounds:
                    prop.boundsValue = (Bounds)toValue;
                    break;

                case SerializedPropertyType.Color:
                    prop.colorValue = (Color)toValue;
                    break;

                case SerializedPropertyType.Float:
                    prop.floatValue = (float)toValue;
                    break;

                case SerializedPropertyType.Integer:
                    prop.intValue = (int)toValue;
                    break;

                case SerializedPropertyType.ObjectReference:
                    prop.objectReferenceValue = toValue as UnityEngine.Object;
                    break;

                case SerializedPropertyType.Rect:
                    prop.rectValue = (Rect)toValue;
                    break;

                case SerializedPropertyType.String:
                    prop.stringValue = (string)toValue;
                    break;

                case SerializedPropertyType.Vector2:
                    prop.vector2Value = (Vector2)toValue;
                    break;

                case SerializedPropertyType.Vector3:
                    prop.vector3Value = (Vector3)toValue;
                    break;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>System.Object.</returns>
        public static object GetValue(this sp property)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);
            return fi.GetValue(property.serializedObject.targetObject);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        public static void SetValue(this sp property, object value)
        {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);//this FieldInfo contains the type.
            fi.SetValue(property.serializedObject.targetObject, value);
        }
    }
}