using Lerp2API._Debug;
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
            DrawDefaultInspector();

            bool curFSWValue = serializedObject.FindProperty("m_disableFileSystemWatcher").boolValue;
            if (localDisabledFSW != curFSWValue)
            {
                if (curFSWValue)
                    LerpedEditorCore.DisableFSW();
                else
                    LerpedEditorCore.EnableFSW();
                localDisabledFSW = curFSWValue;
            }
        }
    }
}