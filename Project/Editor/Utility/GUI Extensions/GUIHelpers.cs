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
    public enum ReferType { Editor, EditorWindow }

    public class LerpedList
    {
        public bool _debug = true;

        public ReorderableList m_List;
        public SerializedObject m_Obj;
        public SerializedProperty m_Prop;

        private string label;

        //private List<float> m_Heights;

        internal Texture2D backgroundTex;
        internal ReferType m_Type;
        internal ScriptableObject m_Refer;

        public string Label
        {
            get
            {
                return label;
            }
        }

        public int PropLength
        {
            get
            {
                return m_Prop.arraySize;
            }
        }

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

        public LerpedList(Editor refer, SerializedObject obj, SerializedProperty prop, string lbl, bool debug = true)
            : this(refer, obj, prop, lbl, ReferType.Editor, debug)
        {
        }

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

        public void SetHeaderCallback(int idx, Func<int, HeaderCallbackDelegate> act)
        {
            m_List.drawHeaderCallback = act(idx);
        }

        public void SetElementCallback(int idx, Func<int, ElementCallbackDelegate> act)
        {
            m_List.drawElementCallback = act(idx);
        }

        public void SetHeightElementCallback(int idx, Func<int, ElementHeightCallbackDelegate> act)
        {
            m_List.elementHeightCallback = act(idx);
        }

        public void SetElementBackgroundCallback(int idx, Func<int, ElementCallbackDelegate> act)
        {
            m_List.drawElementBackgroundCallback = act(idx);
        }

        public void SetAddDropdownCallback(int idx, Func<int, AddDropdownCallbackDelegate> act)
        {
            m_List.onAddDropdownCallback = act(idx);
        }

        public void SetHeaderCallback(HeaderCallbackDelegate dlg)
        {
            m_List.drawHeaderCallback = dlg;
        }

        public void SetElementCallback(ElementCallbackDelegate dlg)
        {
            m_List.drawElementCallback = dlg;
        }

        public void SetHeightElementCallback(ElementHeightCallbackDelegate dlg)
        {
            m_List.elementHeightCallback = dlg;
        }

        public void SetElementBackgroundCallback(ElementCallbackDelegate dlg)
        {
            m_List.drawElementBackgroundCallback = dlg;
        }

        public void SetAddDropdownCallback(AddDropdownCallbackDelegate dlg)
        {
            m_List.onAddDropdownCallback = dlg;
        }

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