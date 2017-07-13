using Lerp2APIEditor;
using UnityEditor;

namespace Lerp2Raw
{
    [CustomEditor(typeof(LerpedHook))]
    public class LerpedEditor : Editor
    {
        private bool localDisabledFSW;

        public override void OnInspectorGUI()
        {
            bool curFSWValue = serializedObject.FindProperty("m_disableFileSystemWatcher").boolValue;
            if (localDisabledFSW != curFSWValue)
            {
                if (curFSWValue)
                    LerpedEditorCore.DisableFSW();
                else
                    LerpedEditorCore.EnableFSW(); //...aaacccaaaddfgfdfg
            }
        }
    }
}