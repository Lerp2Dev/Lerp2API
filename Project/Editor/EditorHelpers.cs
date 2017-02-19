using System;
using System.Diagnostics;
using System.Threading;
using UnityEditor;
using Debug = Lerp2API.DebugHandler.Debug;

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

    #region "Editor Reflection Extensions"
    public class EditorReflectionHelpers
    {
        public Action fin;
        public EditorReflectionHelpers(Action f)
        {
            fin = f;
        }
        public void WaitUntilClassIsAvailable(string type)
        {
            Debug.LogFormat("Waiting for '{0}' class.", type);
            ThreadSafeEditor tse = new ThreadSafeEditor();
            Thread th = new Thread(() =>
            {
                tse.Message("echo prueba xd", "echo hola");
                int secs = 30;
                bool av = false;
                while (--secs > 0)
                {
                    av = Type.GetType(type) != null;
                    if (av)
                        break;
                    Thread.Sleep(1000);
                }
                //Process.Start(string.Format("cmd.exe /c echo {0} {1} & pause", av, secs));
                if (av) fin();
            });
            th.Start();
        }
    }
    #endregion

    #region "Thread Safe Editor Extensions"
    public class ThreadSafeEditor
    {
        private Process cmd;
        public ThreadSafeEditor()
        {
            cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
        }
        public void Message(params string[] commands)
        {
            //Process.Start("cmd.exe", string.Format("/c {0} & pause", string.Join(" & ", commands)));
            cmd.StartInfo.Arguments = string.Format("/c {0} & pause", string.Join(" & ", commands));
            cmd.Start();
        }
    }
    #endregion
}
