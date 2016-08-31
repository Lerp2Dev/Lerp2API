using UnityEditor;

namespace Lerp2APIEditor
{
    public static class EditorHelpers
    {
        #region "Editor Extensions"

        public static void DefineTag(this string tagName)
        { //Credits: http://answers.unity3d.com/questions/33597/is-it-possible-to-create-a-tag-programmatically.html
          // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // For Unity 5 we need this too
            //SerializedProperty layersProp = tagManager.FindProperty("layers");

            // Adding a Tag
            // First check if it is not already present
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tagName)) { found = true; break; }
            }

            // if not found, add it
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = tagName;
            }
        }

        #endregion "Editor Extensions"
    }
}
