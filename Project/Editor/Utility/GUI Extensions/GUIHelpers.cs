using Lerp2API.Hepers.Serializer_Extensions;
using Lerp2API.Utility.StandardInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2APIEditor.Utility.GUI_Extensions
{
    /// <summary>
    /// Class LerpedList.
    /// </summary>
    public class LerpedList
    {
        /// <summary>
        /// The debug
        /// </summary>
        public bool _debug = true;

        /// <summary>
        /// The m list
        /// </summary>
        public ReorderableList m_List;

        /// <summary>
        /// The m object
        /// </summary>
        public SerializedObject m_Obj;

        /// <summary>
        /// The m property
        /// </summary>
        public SerializedProperty m_Prop;

        private string label;

        //private List<float> m_Heights;

        internal Texture2D backgroundTex;
        internal ReferType m_Type;
        internal ScriptableObject m_Refer;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <value>The label.</value>
        public string Label
        {
            get
            {
                return label;
            }
        }

        /// <summary>
        /// Gets the length of the property.
        /// </summary>
        /// <value>The length of the property.</value>
        public int PropLength
        {
            get
            {
                return m_Prop.arraySize;
            }
        }

        /// <summary>
        /// Gets the default header callback.
        /// </summary>
        /// <value>The default header callback.</value>
        public HeaderCallbackDelegate DefaultHeaderCallback
        {
            get
            {
                return rect =>
                {
                    EditorGUI.LabelField(rect, label);
                };
            }
        }

        /// <summary>
        /// Gets the default element callback.
        /// </summary>
        /// <value>The default element callback.</value>
        public ElementCallbackDelegate DefaultElementCallback
        {
            get
            {
                return (rect, index, active, focused) =>
                {
                    SerializedProperty element = m_List.serializedProperty.GetArrayElementAtIndex(index);
                    Sprite s = (element.objectReferenceValue as Sprite);

                    bool foldout = active;
                    float height = EditorGUIUtility.singleLineHeight * 1.25f;
                    if (foldout)
                    {
                        height = EditorGUIUtility.singleLineHeight * 5;
                    }

                    /*try
                    {
                        m_Heights[index] = height;
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        if (_debug)
                            Debug.LogWarning(e.Message); //Esto me da una excepción
                    }
                    finally
                    {
                        float[] floats = m_Heights.ToArray();
                        Array.Resize(ref floats, m_Prop.arraySize);
                        m_Heights = floats.ToList();
                    }*/

                    float margin = height / 10;
                    rect.y += margin;
                    rect.height = (height / 5) * 4;
                    rect.width = rect.width / 2 - margin / 2;

                    if (foldout)
                        if (s)
                            EditorGUI.DrawPreviewTexture(rect, s.texture);

                    rect.x += rect.width + margin;
                    EditorGUI.ObjectField(rect, element, GUIContent.none);
                };
            }
        }

        /// <summary>
        /// Gets the default height element callback.
        /// </summary>
        /// <value>The default height element callback.</value>
        public ElementHeightCallbackDelegate DefaultHeightElementCallback
        {
            get
            {
                return (index) =>
                {
                    if (m_Type == ReferType.Editor)
                        ((Editor)m_Refer).Repaint();
                    else
                        ((EditorWindow)m_Refer).Repaint();

                    float height = 0;

                    /*try
                    {
                        height = m_Heights[index];
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        if (_debug)
                            Debug.LogWarning(e.Message);
                    }
                    finally
                    {
                        float[] floats = m_Heights.ToArray();
                        Array.Resize(ref floats, m_Prop.arraySize);
                        m_Heights = floats.ToList();
                    }*/

                    return height;
                };
            }
        }

        /// <summary>
        /// Gets the default element background callback.
        /// </summary>
        /// <value>The default element background callback.</value>
        public ElementCallbackDelegate DefaultElementBackgroundCallback
        {
            get
            {
                return (rect, index, active, focused) =>
                {
                    //rect.height = m_Heights[index];
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                    tex.Apply();
                    if (active)
                        GUI.DrawTexture(rect, tex as Texture);
                };
            }
        }

        /// <summary>
        /// Gets the default add dropdown callback.
        /// </summary>
        /// <value>The default add dropdown callback.</value>
        public AddDropdownCallbackDelegate DefaultAddDropdownCallback
        {
            get
            {
                return (rect, li) =>
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add Element"), false, () =>
                    {
                        m_Obj.Update();
                        ++li.serializedProperty.arraySize;
                        m_Obj.ApplyModifiedProperties();
                    });

                    menu.ShowAsContext();

                    /*float[] floats = m_Heights.ToArray();
                    Array.Resize(ref floats, m_Prop.arraySize);
                    m_Heights = floats.ToList();*/
                };
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LerpedList"/> class.
        /// </summary>
        /// <param name="refer">The refer.</param>
        /// <param name="obj">The object.</param>
        /// <param name="prop">The property.</param>
        /// <param name="lbl">The label.</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        public LerpedList(Editor refer, SerializedObject obj, SerializedProperty prop, string lbl, bool debug = true)
            : this(refer, obj, prop, lbl, ReferType.Editor, debug)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LerpedList"/> class.
        /// </summary>
        /// <param name="refer">The refer.</param>
        /// <param name="obj">The object.</param>
        /// <param name="prop">The property.</param>
        /// <param name="lbl">The label.</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        public LerpedList(EditorWindow refer, SerializedObject obj, SerializedProperty prop, string lbl, bool debug = true)
            : this(refer, obj, prop, lbl, ReferType.EditorWindow, debug)
        {
        }

        private LerpedList(ScriptableObject refer, SerializedObject obj, SerializedProperty prop, string lbl, ReferType type, bool debug = true)
        { //Refer can be also a Editor class
            ReorderableList list = new ReorderableList(obj, prop, true, true, true, true);

            //m_Heights = new List<float>(prop.arraySize);

            label = lbl;
            _debug = debug;

            m_Obj = obj;
            m_Prop = prop;
            m_Refer = refer;
            m_Type = type;

            m_List = list;

            backgroundTex = new Lerp2API.Optimizers.Color(85, 170, 255, 170).ToTexture();

            //Tests

            /*Debug.Log(m_Prop.GetAt(0) == null);
            Debug.Log(m_Prop.GetAt(0).CountInProperty());
            Debug.Log(m_Prop.GetAt(0).GetEndProperty().type);*/

            //Debug.Log(m_Obj.FindProperty("assets").type);
            //Debug.Log(m_Obj.FindProperty("assets").propertyType);
            //Debug.Log(m_Obj.FindProperty("assets").arrayElementType);
            //Debug.Log(((AssetsLocation)m_Obj.targetObject).serializedObj.FindProperty("assets").GetArrayElementAtIndex(0).objectReferenceValue == null);
            //Debug.Log(m_Obj.FindProperty("assets").CountInProperty());
            //Debug.Log("ISNULL?: " + m_Obj.FindProperty("assets.Array.data[0]").objectReferenceValue == null);
            //Debug.Log("LEN: " + m_Obj.FindProperty("assets").GetArrayElementAtIndex(0).FindPropertyRelative("active") == null);
        }

        internal void ResizeArr(int size)
        {
            m_Prop.arraySize = size;
            m_Obj.ApplyModifiedProperties();
        }

        /// <summary>
        /// Sets the header callback.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <param name="act">The act.</param>
        public void SetHeaderCallback(int idx, Func<int, HeaderCallbackDelegate> act)
        {
            m_List.drawHeaderCallback = act(idx);
        }

        /// <summary>
        /// Sets the element callback.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <param name="act">The act.</param>
        public void SetElementCallback(int idx, Func<int, ElementCallbackDelegate> act)
        {
            m_List.drawElementCallback = act(idx);
        }

        /// <summary>
        /// Sets the height element callback.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <param name="act">The act.</param>
        public void SetHeightElementCallback(int idx, Func<int, ElementHeightCallbackDelegate> act)
        {
            m_List.elementHeightCallback = act(idx);
        }

        /// <summary>
        /// Sets the element background callback.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <param name="act">The act.</param>
        public void SetElementBackgroundCallback(int idx, Func<int, ElementCallbackDelegate> act)
        {
            m_List.drawElementBackgroundCallback = act(idx);
        }

        /// <summary>
        /// Sets the add dropdown callback.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <param name="act">The act.</param>
        public void SetAddDropdownCallback(int idx, Func<int, AddDropdownCallbackDelegate> act)
        {
            m_List.onAddDropdownCallback = act(idx);
        }

        /// <summary>
        /// Sets the header callback.
        /// </summary>
        /// <param name="dlg">The dialog.</param>
        public void SetHeaderCallback(HeaderCallbackDelegate dlg)
        {
            m_List.drawHeaderCallback = dlg;
        }

        /// <summary>
        /// Sets the element callback.
        /// </summary>
        /// <param name="dlg">The dialog.</param>
        public void SetElementCallback(ElementCallbackDelegate dlg)
        {
            m_List.drawElementCallback = dlg;
        }

        /// <summary>
        /// Sets the height element callback.
        /// </summary>
        /// <param name="dlg">The dialog.</param>
        public void SetHeightElementCallback(ElementHeightCallbackDelegate dlg)
        {
            m_List.elementHeightCallback = dlg;
        }

        /// <summary>
        /// Sets the element background callback.
        /// </summary>
        /// <param name="dlg">The dialog.</param>
        public void SetElementBackgroundCallback(ElementCallbackDelegate dlg)
        {
            m_List.drawElementBackgroundCallback = dlg;
        }

        /// <summary>
        /// Sets the add dropdown callback.
        /// </summary>
        /// <param name="dlg">The dialog.</param>
        public void SetAddDropdownCallback(AddDropdownCallbackDelegate dlg)
        {
            m_List.onAddDropdownCallback = dlg;
        }

        /// <summary>
        /// Draws this instance.
        /// </summary>
        public void Draw()
        {
            if (m_List.drawHeaderCallback == null)
                m_List.drawHeaderCallback = DefaultHeaderCallback;

            if (m_List.drawElementCallback == null)
                m_List.drawElementCallback = DefaultElementCallback;

            if (m_List.elementHeightCallback == null)
                m_List.elementHeightCallback = DefaultHeightElementCallback;

            if (m_List.drawElementBackgroundCallback == null)
                m_List.drawElementBackgroundCallback = DefaultElementBackgroundCallback;

            if (m_List.onAddDropdownCallback == null)
                m_List.onAddDropdownCallback = DefaultAddDropdownCallback;

            m_List.DoLayoutList();
        }
    }
}