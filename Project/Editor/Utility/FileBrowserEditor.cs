using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Lerp2API.Utility;

namespace Lerp2APIEditor.Utility
{

    /*
        File browser for selecting files or folders at runtime.
     */

    /// <summary>
    /// Class FileBrowserEditor.
    /// </summary>
    public class FileBrowserEditor
    {

        // Called when the user clicks cancel or select
        //public delegate void FinishedCallback(string path);
        // Defaults to working directory
        /// <summary>
        /// Gets or sets the current directory.
        /// </summary>
        /// <value>The current directory.</value>
        public string CurrentDirectory
        {
            get
            {
                return m_currentDirectory;
            }
            set
            {
                SetNewDirectory(value);
                SwitchDirectoryNow();
            }
        }
        /// <summary>
        /// The m current directory
        /// </summary>
        protected string m_currentDirectory;
        // Optional pattern for filtering selectable files/folders. See:
        // http://msdn.microsoft.com/en-us/library/wz42302f(v=VS.90).aspx
        // and
        // http://msdn.microsoft.com/en-us/library/6ff71z1w(v=VS.90).aspx
        /// <summary>
        /// Gets or sets the selection pattern.
        /// </summary>
        /// <value>The selection pattern.</value>
        public string SelectionPattern
        {
            get
            {
                return m_filePattern;
            }
            set
            {
                m_filePattern = value;
                ReadDirectoryContents();
            }
        }
        /// <summary>
        /// The m file pattern
        /// </summary>
        protected string m_filePattern;

        // Optional image for directories
        /// <summary>
        /// Gets or sets the directory image.
        /// </summary>
        /// <value>The directory image.</value>
        public Texture2D DirectoryImage
        {
            get
            {
                return m_directoryImage;
            }
            set
            {
                m_directoryImage = value;
                BuildContent();
            }
        }
        /// <summary>
        /// The m directory image
        /// </summary>
        protected Texture2D m_directoryImage;

        // Optional image for files
        /// <summary>
        /// Gets or sets the file image.
        /// </summary>
        /// <value>The file image.</value>
        public Texture2D FileImage
        {
            get
            {
                return m_fileImage;
            }
            set
            {
                m_fileImage = value;
                BuildContent();
            }
        }
        /// <summary>
        /// The m file image
        /// </summary>
        protected Texture2D m_fileImage;

        // Browser type. Defaults to File, but can be set to Folder
        /// <summary>
        /// Gets or sets the type of the browser.
        /// </summary>
        /// <value>The type of the browser.</value>
        public FileBrowserType BrowserType
        {
            get
            {
                return m_browserType;
            }
            set
            {
                m_browserType = value;
                ReadDirectoryContents();
            }
        }
        /// <summary>
        /// The m browser type
        /// </summary>
        protected FileBrowserType m_browserType;
        /// <summary>
        /// The m new directory
        /// </summary>
        protected string m_newDirectory;
        /// <summary>
        /// The m current directory parts
        /// </summary>
        protected string[] m_currentDirectoryParts;

        /// <summary>
        /// The m files
        /// </summary>
        protected string[] m_files;
        /// <summary>
        /// The m files with images
        /// </summary>
        protected GUIContent[] m_filesWithImages;
        /// <summary>
        /// The m selected file
        /// </summary>
        protected int m_selectedFile;

        /// <summary>
        /// The m non matching files
        /// </summary>
        protected string[] m_nonMatchingFiles;
        /// <summary>
        /// The m non matching files with images
        /// </summary>
        protected GUIContent[] m_nonMatchingFilesWithImages;
        /// <summary>
        /// The m selected non matching directory
        /// </summary>
        protected int m_selectedNonMatchingDirectory;

        /// <summary>
        /// The m directories
        /// </summary>
        protected string[] m_directories;
        /// <summary>
        /// The m directories with images
        /// </summary>
        protected GUIContent[] m_directoriesWithImages;
        /// <summary>
        /// The m selected directory
        /// </summary>
        protected int m_selectedDirectory;

        /// <summary>
        /// The m non matching directories
        /// </summary>
        protected string[] m_nonMatchingDirectories;
        /// <summary>
        /// The m non matching directories with images
        /// </summary>
        protected GUIContent[] m_nonMatchingDirectoriesWithImages;

        /// <summary>
        /// The m current directory matches
        /// </summary>
        protected bool m_currentDirectoryMatches;

        /// <summary>
        /// Gets the centred text.
        /// </summary>
        /// <value>The centred text.</value>
        protected GUIStyle CentredText
        {
            get
            {
                if (m_centredText == null)
                {
                    m_centredText = new GUIStyle(GUI.skin.label);
                    m_centredText.alignment = TextAnchor.MiddleLeft;
                    m_centredText.fixedHeight = GUI.skin.button.fixedHeight;
                }
                return m_centredText;
            }
        }
        /// <summary>
        /// The m centred text
        /// </summary>
        protected GUIStyle m_centredText;

        /// <summary>
        /// The m name
        /// </summary>
        protected string m_name;
        /// <summary>
        /// The m window
        /// </summary>
        protected EditorWindow m_window;
        /// <summary>
        /// Gets the screen rect.
        /// </summary>
        /// <value>The screen rect.</value>
        protected Rect screenRect //#if UNITY_EDITOR
        {
            get
            {
                return new Rect(0, 0, m_window.position.width, m_window.position.height);
            }
        }

        /// <summary>
        /// The m scroll position
        /// </summary>
        protected Vector2 m_scrollPosition;

        /// <summary>
        /// The m callback
        /// </summary>
        protected Action<string> m_callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBrowserEditor"/> class.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="browserType">Type of the browser.</param>
        /// <param name="name">The name.</param>
        /// <param name="callback">The callback.</param>
        public FileBrowserEditor(EditorWindow window, FileBrowserType browserType, string name, Action<string> callback)
        {
            m_window = window;
            m_name = name;
            m_browserType = browserType;
            m_callback = callback;
            SetNewDirectory(Directory.GetCurrentDirectory());
            SwitchDirectoryNow();
        }

        /// <summary>
        /// Sets the new directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        protected void SetNewDirectory(string directory)
        {
            m_newDirectory = directory;
        }

        /// <summary>
        /// Switches the directory now.
        /// </summary>
        protected void SwitchDirectoryNow()
        {
            if (m_newDirectory == null || m_currentDirectory == m_newDirectory)
            {
                return;
            }
            m_currentDirectory = m_newDirectory;
            m_scrollPosition = Vector2.zero;
            m_selectedDirectory = m_selectedNonMatchingDirectory = m_selectedFile = -1;
            ReadDirectoryContents();
        }

        /// <summary>
        /// Reads the directory contents.
        /// </summary>
        protected void ReadDirectoryContents()
        {
            if (m_currentDirectory == "/")
            {
                m_currentDirectoryParts = new string[] { "" };
                m_currentDirectoryMatches = false;
            }
            else {
                m_currentDirectoryParts = m_currentDirectory.Split(Path.DirectorySeparatorChar);
                if (SelectionPattern != null)
                {
                    string directoryName = Path.GetDirectoryName(m_currentDirectory);
                    string[] generation = new string[0];
                    if (directoryName != null)
                    {   //This is new: generation should be an empty array for the root directory.
                        //directoryName will be null if it's a root directory
                        generation = Directory.GetDirectories(
                        directoryName,
                        SelectionPattern);
                    }
                    m_currentDirectoryMatches = Array.IndexOf(generation, m_currentDirectory) >= 0;
                }
                else {
                    m_currentDirectoryMatches = false;
                }
            }

            if (BrowserType == FileBrowserType.File || SelectionPattern == null)
            {
                m_directories = Directory.GetDirectories(m_currentDirectory);
                m_nonMatchingDirectories = new string[0];
            }
            else {
                m_directories = Directory.GetDirectories(m_currentDirectory, SelectionPattern);
                var nonMatchingDirectories = new List<string>();
                foreach (string directoryPath in Directory.GetDirectories(m_currentDirectory))
                {
                    if (Array.IndexOf(m_directories, directoryPath) < 0)
                    {
                        nonMatchingDirectories.Add(directoryPath);
                    }
                }
                m_nonMatchingDirectories = nonMatchingDirectories.ToArray();
                for (int i = 0; i < m_nonMatchingDirectories.Length; ++i)
                {
                    int lastSeparator = m_nonMatchingDirectories[i].LastIndexOf(Path.DirectorySeparatorChar);
                    m_nonMatchingDirectories[i] = m_nonMatchingDirectories[i].Substring(lastSeparator + 1);
                }
                Array.Sort(m_nonMatchingDirectories);
            }

            for (int i = 0; i < m_directories.Length; ++i)
            {
                m_directories[i] = m_directories[i].Substring(m_directories[i].LastIndexOf(Path.DirectorySeparatorChar) + 1);
            }

            if (BrowserType == FileBrowserType.Directory || SelectionPattern == null)
            {
                m_files = Directory.GetFiles(m_currentDirectory);
                m_nonMatchingFiles = new string[0];
            }
            else {
                m_files = Directory.GetFiles(m_currentDirectory, SelectionPattern);
                var nonMatchingFiles = new List<string>();
                foreach (string filePath in Directory.GetFiles(m_currentDirectory))
                {
                    if (Array.IndexOf(m_files, filePath) < 0)
                    {
                        nonMatchingFiles.Add(filePath);
                    }
                }
                m_nonMatchingFiles = nonMatchingFiles.ToArray();
                for (int i = 0; i < m_nonMatchingFiles.Length; ++i)
                {
                    m_nonMatchingFiles[i] = Path.GetFileName(m_nonMatchingFiles[i]);
                }
                Array.Sort(m_nonMatchingFiles);
            }
            for (int i = 0; i < m_files.Length; ++i)
            {
                m_files[i] = Path.GetFileName(m_files[i]);
            }
            Array.Sort(m_files);
            BuildContent();
            m_newDirectory = null;
        }

        /// <summary>
        /// Builds the content.
        /// </summary>
        protected void BuildContent()
        {
            m_directoriesWithImages = new GUIContent[m_directories.Length];
            for (int i = 0; i < m_directoriesWithImages.Length; ++i)
            {
                m_directoriesWithImages[i] = new GUIContent(" " + m_directories[i], DirectoryImage);
            }
            m_nonMatchingDirectoriesWithImages = new GUIContent[m_nonMatchingDirectories.Length];
            for (int i = 0; i < m_nonMatchingDirectoriesWithImages.Length; ++i)
            {
                m_nonMatchingDirectoriesWithImages[i] = new GUIContent(" " + m_nonMatchingDirectories[i], DirectoryImage);
            }
            m_filesWithImages = new GUIContent[m_files.Length];
            for (int i = 0; i < m_filesWithImages.Length; ++i)
            {
                m_filesWithImages[i] = new GUIContent(" " + m_files[i], FileImage);
            }
            m_nonMatchingFilesWithImages = new GUIContent[m_nonMatchingFiles.Length];
            for (int i = 0; i < m_nonMatchingFilesWithImages.Length; ++i)
            {
                m_nonMatchingFilesWithImages[i] = new GUIContent(" " + m_nonMatchingFiles[i], FileImage);
            }
        }

        /// <summary>
        /// Called when [GUI].
        /// </summary>
        public void OnGUI()
        {
            GUILayout.BeginArea(
                screenRect,
                m_name,
                GUI.skin.window
            );
            GUILayout.BeginHorizontal();
            for (int parentIndex = 0; parentIndex < m_currentDirectoryParts.Length; ++parentIndex)
            {
                if (parentIndex == m_currentDirectoryParts.Length - 1)
                {
                    GUILayout.Label(m_currentDirectoryParts[parentIndex], CentredText);
                }
                else if (GUILayout.Button(m_currentDirectoryParts[parentIndex]))
                {
                    string parentDirectoryName = m_currentDirectory;
                    for (int i = m_currentDirectoryParts.Length - 1; i > parentIndex; --i)
                    {
                        parentDirectoryName = Path.GetDirectoryName(parentDirectoryName);
                    }
                    SetNewDirectory(parentDirectoryName);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            m_scrollPosition = GUILayout.BeginScrollView(
                m_scrollPosition,
                false,
                true,
                GUI.skin.horizontalScrollbar,
                GUI.skin.verticalScrollbar,
                GUI.skin.box
            );
            m_selectedDirectory = GUILayoutx.SelectionList(
                m_selectedDirectory,
                m_directoriesWithImages,
                DirectoryDoubleClickCallback
            );
            if (m_selectedDirectory > -1)
            {
                m_selectedFile = m_selectedNonMatchingDirectory = -1;
            }
            m_selectedNonMatchingDirectory = GUILayoutx.SelectionList(
                m_selectedNonMatchingDirectory,
                m_nonMatchingDirectoriesWithImages,
                NonMatchingDirectoryDoubleClickCallback
            );
            if (m_selectedNonMatchingDirectory > -1)
            {
                m_selectedDirectory = m_selectedFile = -1;
            }
            GUI.enabled = BrowserType == FileBrowserType.File;
            m_selectedFile = GUILayoutx.SelectionList(
                m_selectedFile,
                m_filesWithImages,
                FileDoubleClickCallback
            );
            GUI.enabled = true;
            if (m_selectedFile > -1)
            {
                m_selectedDirectory = m_selectedNonMatchingDirectory = -1;
            }
            GUI.enabled = false;
            GUILayoutx.SelectionList(
                -1,
                m_nonMatchingFilesWithImages
            );
            GUI.enabled = true;
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Cancel", GUILayout.Width(50)))
                m_callback(null);
            if (BrowserType == FileBrowserType.File)
                GUI.enabled = m_selectedFile > -1;
            else
            {
                if (SelectionPattern == null)
                    GUI.enabled = m_selectedDirectory > -1;
                else
                    GUI.enabled = m_selectedDirectory > -1 ||
                                    (
                                        m_currentDirectoryMatches &&
                                        m_selectedNonMatchingDirectory == -1 &&
                                        m_selectedFile == -1
                                    );
            }
            if (GUILayout.Button("Select", GUILayout.Width(50)))
            {
                if (BrowserType == FileBrowserType.File)
                    m_callback(Path.Combine(m_currentDirectory, m_files[m_selectedFile]));
                else
                {
                    if (m_selectedDirectory > -1)
                        m_callback(Path.Combine(m_currentDirectory, m_directories[m_selectedDirectory]));
                    else 
                        m_callback(m_currentDirectory);
                }
                m_window.Close();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (Event.current.type == EventType.Repaint)
                SwitchDirectoryNow();
        }

        /// <summary>
        /// Files the double click callback.
        /// </summary>
        /// <param name="i">The i.</param>
        protected void FileDoubleClickCallback(int i)
        {
            if (BrowserType == FileBrowserType.File)
                m_callback(Path.Combine(m_currentDirectory, m_files[i]));
        }

        /// <summary>
        /// Directories the double click callback.
        /// </summary>
        /// <param name="i">The i.</param>
        protected void DirectoryDoubleClickCallback(int i)
        {
            SetNewDirectory(Path.Combine(m_currentDirectory, m_directories[i]));
        }

        /// <summary>
        /// Nons the matching directory double click callback.
        /// </summary>
        /// <param name="i">The i.</param>
        protected void NonMatchingDirectoryDoubleClickCallback(int i)
        {
            SetNewDirectory(Path.Combine(m_currentDirectory, m_nonMatchingDirectories[i]));
        }

    }
}