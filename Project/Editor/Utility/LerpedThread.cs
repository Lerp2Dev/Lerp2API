using System;
using System.IO;
using System.Collections.Generic;
using Debug = Lerp2API._Debug.Debug;
using Lerp2API;

namespace Lerp2APIEditor.Utility
{
    /// <summary>
    /// Class LerpedThread.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LerpedThread<T>
    {
        /// <summary>
        /// The value
        /// </summary>
        public T value = default(T);

        /// <summary>
        /// The is called
        /// </summary>
        public bool isCalled = false;

        /// <summary>
        /// The method called
        /// </summary>
        public string methodCalled = "";

        /// <summary>
        /// The matched methods
        /// </summary>
        public Dictionary<string, Action> matchedMethods = new Dictionary<string, Action>();

        /// <summary>
        /// Gets the FSW.
        /// </summary>
        /// <value>The FSW.</value>
        public FileSystemWatcher FSW
        {
            get
            {
                return (FileSystemWatcher)(object)value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LerpedThread{T}"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="pars">The pars.</param>
        public LerpedThread(string name, FSWParams pars)
        {
            if (typeof(T) == typeof(FileSystemWatcher))
            {
                if (LerpedCore.disableFileSystemWatcher)
                    return;
                try
                {
                    FileSystemWatcher watcher = new FileSystemWatcher(pars.path, pars.filter);

                    watcher.NotifyFilter = pars.notifiers;
                    watcher.IncludeSubdirectories = pars.includeSubfolders;

                    watcher.Changed += OnChanged;
                    watcher.Created += OnChanged;
                    watcher.Deleted += OnChanged;
                    watcher.Renamed += OnRenamed;

                    ApplyChanges(watcher);
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Folder not found!\n{0}\n{1}", pars.path, ex);
                }
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            methodCalled = e.ChangeType.ToString();
            isCalled = true;
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            methodCalled = e.ChangeType.ToString();
            isCalled = true;
        }

        /// <summary>
        /// Starts the FSW.
        /// </summary>
        public void StartFSW()
        {
            FSW.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Cancels the FSW.
        /// </summary>
        public void CancelFSW()
        {
            FSW.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Applies the changes.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <param name="obj">The object.</param>
        public void ApplyChanges<T1>(T1 obj)
        {
            value = (T)(object)obj;
        }
    }

    /// <summary>
    /// Class FSWParams.
    /// </summary>
    public class FSWParams
    {
        /// <summary>
        /// The path
        /// </summary>
        public string path,
                      /// <summary>
                      /// The filter
                      /// </summary>
                      filter;

        /// <summary>
        /// The notifiers
        /// </summary>
        public NotifyFilters notifiers;

        /// <summary>
        /// The include subfolders
        /// </summary>
        public bool includeSubfolders;

        /// <summary>
        /// Initializes a new instance of the <see cref="FSWParams"/> class.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="f">The f.</param>
        /// <param name="nf">The nf.</param>
        /// <param name="isf">if set to <c>true</c> [isf].</param>
        public FSWParams(string p, string f, NotifyFilters nf, bool isf)
        {
            path = p;
            filter = f;
            notifiers = nf;
            includeSubfolders = isf;
        }
    }
}