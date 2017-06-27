using UnityEngine;

/// <summary>
/// Class GUILayoutx.
/// </summary>
public class GUILayoutx
{

    /// <summary>
    /// Delegate DoubleClickCallback
    /// </summary>
    /// <param name="index">The index.</param>
    public delegate void DoubleClickCallback(int index);

    /// <summary>
    /// Selections the list.
    /// </summary>
    /// <param name="selected">The selected.</param>
    /// <param name="list">The list.</param>
    /// <returns>System.Int32.</returns>
    public static int SelectionList(int selected, GUIContent[] list)
    {
        return SelectionList(selected, list, "List Item", null);
    }

    /// <summary>
    /// Selections the list.
    /// </summary>
    /// <param name="selected">The selected.</param>
    /// <param name="list">The list.</param>
    /// <param name="elementStyle">The element style.</param>
    /// <returns>System.Int32.</returns>
    public static int SelectionList(int selected, GUIContent[] list, GUIStyle elementStyle)
    {
        return SelectionList(selected, list, elementStyle, null);
    }

    /// <summary>
    /// Selections the list.
    /// </summary>
    /// <param name="selected">The selected.</param>
    /// <param name="list">The list.</param>
    /// <param name="callback">The callback.</param>
    /// <returns>System.Int32.</returns>
    public static int SelectionList(int selected, GUIContent[] list, DoubleClickCallback callback)
    {
        return SelectionList(selected, list, "List Item", callback);
    }

    /// <summary>
    /// Selections the list.
    /// </summary>
    /// <param name="selected">The selected.</param>
    /// <param name="list">The list.</param>
    /// <param name="elementStyle">The element style.</param>
    /// <param name="callback">The callback.</param>
    /// <returns>System.Int32.</returns>
    public static int SelectionList(int selected, GUIContent[] list, GUIStyle elementStyle, DoubleClickCallback callback)
    {
        for (int i = 0; i < list.Length; ++i)
        {
            Rect elementRect = GUILayoutUtility.GetRect(list[i], elementStyle);
            bool hover = elementRect.Contains(Event.current.mousePosition);
            if (hover && Event.current.type == EventType.MouseDown && Event.current.clickCount == 1)
            {
                selected = i;
                Event.current.Use();
            }
            else if (hover && callback != null && Event.current.type == EventType.MouseDown && Event.current.clickCount == 2)
            {
                callback(i);
                Event.current.Use();
            }
            else if (Event.current.type == EventType.repaint)
            {
                elementStyle.Draw(elementRect, list[i], hover, false, i == selected, false);
            }
        }
        return selected;
    }

    /// <summary>
    /// Selections the list.
    /// </summary>
    /// <param name="selected">The selected.</param>
    /// <param name="list">The list.</param>
    /// <returns>System.Int32.</returns>
    public static int SelectionList(int selected, string[] list)
    {
        return SelectionList(selected, list, "List Item", null);
    }

    /// <summary>
    /// Selections the list.
    /// </summary>
    /// <param name="selected">The selected.</param>
    /// <param name="list">The list.</param>
    /// <param name="elementStyle">The element style.</param>
    /// <returns>System.Int32.</returns>
    public static int SelectionList(int selected, string[] list, GUIStyle elementStyle)
    {
        return SelectionList(selected, list, elementStyle, null);
    }

    /// <summary>
    /// Selections the list.
    /// </summary>
    /// <param name="selected">The selected.</param>
    /// <param name="list">The list.</param>
    /// <param name="callback">The callback.</param>
    /// <returns>System.Int32.</returns>
    public static int SelectionList(int selected, string[] list, DoubleClickCallback callback)
    {
        return SelectionList(selected, list, "List Item", callback);
    }

    /// <summary>
    /// Selections the list.
    /// </summary>
    /// <param name="selected">The selected.</param>
    /// <param name="list">The list.</param>
    /// <param name="elementStyle">The element style.</param>
    /// <param name="callback">The callback.</param>
    /// <returns>System.Int32.</returns>
    public static int SelectionList(int selected, string[] list, GUIStyle elementStyle, DoubleClickCallback callback)
    {
        for (int i = 0; i < list.Length; ++i)
        {
            Rect elementRect = GUILayoutUtility.GetRect(new GUIContent(list[i]), elementStyle);
            bool hover = elementRect.Contains(Event.current.mousePosition);
            if (hover && Event.current.type == EventType.MouseDown && Event.current.clickCount == 1)
            {
                selected = i;
                Event.current.Use();
            }
            else if (hover && callback != null && Event.current.type == EventType.MouseDown && Event.current.clickCount == 2)
            {
                callback(i);
                Event.current.Use();
            }
            else if (Event.current.type == EventType.repaint)
            {
                elementStyle.Draw(elementRect, list[i], hover, false, i == selected, false);
            }
        }
        return selected;
    }

}