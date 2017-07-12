using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Malee.Editor
{
    /// <summary>
    /// Class ReorderableDrawer.
    /// </summary>
    /// <seealso cref="UnityEditor.PropertyDrawer" />
    [CustomPropertyDrawer(typeof(ReorderableAttribute))]
    public class ReorderableDrawer : PropertyDrawer
    {
        private static Dictionary<int, ReorderableList> lists = new Dictionary<int, ReorderableList>();

        /// <summary>
        /// Gets the height of the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        /// <returns>System.Single.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ReorderableList list = GetList(property, attribute as ReorderableAttribute);

            return list != null ? list.GetHeight() : EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Called when [GUI].
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="property">The property.</param>
        /// <param name="label">The label.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ReorderableList list = GetList(property, attribute as ReorderableAttribute);

            if (list != null)
            {
                list.DoList(EditorGUI.IndentedRect(position), label);
            }
            else
            {
                GUI.Label(position, "Array must extend from ReorderableArray", EditorStyles.label);
            }
        }

        /// <summary>
        /// Gets the list identifier.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>System.Int32.</returns>
        public static int GetListId(SerializedProperty property)
        {
            if (property != null)
            {
                int h1 = property.serializedObject.targetObject.GetHashCode();
                int h2 = property.propertyPath.GetHashCode();

                return (((h1 << 5) + h1) ^ h2);
            }

            return 0;
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>ReorderableList.</returns>
        public static ReorderableList GetList(SerializedProperty property)
        {
            return GetList(property, null, GetListId(property));
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="attrib">The attribute.</param>
        /// <returns>ReorderableList.</returns>
        public static ReorderableList GetList(SerializedProperty property, ReorderableAttribute attrib)
        {
            return GetList(property, attrib, GetListId(property));
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ReorderableList.</returns>
        public static ReorderableList GetList(SerializedProperty property, int id)
        {
            return GetList(property, null, id);
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="attrib">The attribute.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>ReorderableList.</returns>
        public static ReorderableList GetList(SerializedProperty property, ReorderableAttribute attrib, int id)
        {
            if (property == null)
            {
                return null;
            }

            ReorderableList list = null;
            SerializedProperty array = property.FindPropertyRelative("array");

            if (array != null && array.isArray)
            {
                if (!lists.TryGetValue(id, out list))
                {
                    if (attrib != null)
                    {
                        Texture icon = !string.IsNullOrEmpty(attrib.elementIconPath) ? AssetDatabase.GetCachedIcon(attrib.elementIconPath) : null;

                        list = new ReorderableList(array, attrib.add, attrib.remove, attrib.draggable, ElementDisplayType.Auto, attrib.elementNameProperty, attrib.elementNameOverride, icon);
                    }
                    else
                    {
                        list = new ReorderableList(array, true, true, true);
                    }

                    lists.Add(id, list);
                }
                else
                {
                    list.List = array;
                }
            }

            return list;
        }
    }
}