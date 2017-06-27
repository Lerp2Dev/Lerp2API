using UnityEditor;
using UnityEngine;
using System;
using Lerp2APIEditor.Utility;
using Lerp2API.Utility;

namespace Lerp2APIEditor.EditorWindows
{
    /// <summary>
    /// Class BrowserWindow.
    /// </summary>
    /// <seealso cref="UnityEditor.EditorWindow" />
    public class BrowserWindow : EditorWindow
    {
        internal GUISkin skin;

        internal static Texture2D m_directoryImage,
                                  m_fileImage;

        internal FileBrowserEditor m_fileBrowser;

        internal string m_Path,
                        m_fileName = "",
                        m_ext;

        /// <summary>
        /// The name
        /// </summary>
        public new string name;
        /// <summary>
        /// The width
        /// </summary>
        public int width = 600,
                   /// <summary>
                   /// The height
                   /// </summary>
                   height = 200;
        /// <summary>
        /// The FBT
        /// </summary>
        public FileBrowserType fbt;
        /// <summary>
        /// The cb
        /// </summary>
        public Action<string> cb;

        //public BrowserWindow(FileBrowserType fbt, string name, Action<string> cb) : this(fbt, name, 600, 40, cb) { }

        /*public BrowserWindow()
        {
            /*skin = Resources.Load<GUISkin>("Skins/Default");
            m_directoryImage = Resources.Load<Texture2D>("Textures/folder");
            m_fileImage = Resources.Load<Texture2D>("Textures/file");*/
        //} //Working on the textures... Maybe resources could save my ass?

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Init()
        {
            m_fileBrowser = new FileBrowserEditor(this, fbt, name, cb);

            m_fileBrowser.DirectoryImage = m_directoryImage ?? Resources.Load<Texture2D>("Textures/folder"); //No estoy seguro de si esto funcionará
            m_fileBrowser.FileImage = m_fileImage ?? Resources.Load<Texture2D>("Textures/file");

            skin = skin = Resources.Load<GUISkin>("Skins/File Browser");

            titleContent = new GUIContent(name);
            minSize = new Vector2(width, height);
            maxSize = new Vector2(width, height);
            Show();
        }

        void OnGUI()
        {
            GUI.skin = skin;
            if (m_fileBrowser != null)
                m_fileBrowser.OnGUI();
            else
                OnGUIMain();
        }

        /// <summary>
        /// Called when [GUI main].
        /// </summary>
        protected virtual void OnGUIMain()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Folder to save the file:", GUILayout.Width(200));
            GUILayout.FlexibleSpace();
            GUILayout.Label(m_Path ?? "None selected");
            if (GUILayout.Button("...", GUILayout.ExpandWidth(false)))
            {
                m_fileBrowser = new FileBrowserEditor(
                    this,
                    FileBrowserType.Directory,
                    "Choose directory to save code file",
                    FileSelectedCallback
                );
                m_fileBrowser.DirectoryImage = m_directoryImage;
                m_fileBrowser.FileImage = m_fileImage;
            }
            GUILayout.EndHorizontal();
        }

        void FileSelectedCallback(string path)
        {
            m_fileBrowser = null;
            m_Path = path;
        }
    }
}
