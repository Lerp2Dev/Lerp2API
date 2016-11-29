using System;
using System.IO;
using System.Collections.Generic;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2APIEditor.Utility
{
    public class LerpedThread<T>
    {
        public T value = default(T);
        public bool isCalled = false;
        public string methodCalled = "";
        public Dictionary<string, Action> matchedMethods = new Dictionary<string, Action>();

        public FileSystemWatcher FSW
        {
            get
            {
                return (FileSystemWatcher)(object)value;
            }
        }
        public LerpedThread(string name, FSWParams pars)
        {
            if(typeof(T) == typeof(FileSystemWatcher))
            {
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
        public void StartFSW()
        {
            FSW.EnableRaisingEvents = true;
        }
        public void CancelFSW()
        {
            FSW.EnableRaisingEvents = false;
        }
        public void ApplyChanges<T1>(T1 obj)
        {
            value = (T)(object)obj;
        }
    }
    public class FSWParams
    {
        public string path,
                      filter;
        public NotifyFilters notifiers;
        public bool includeSubfolders;
        public FSWParams(string p, string f, NotifyFilters nf, bool isf)
        {
            path = p;
            filter = f;
            notifiers = nf;
            includeSubfolders = isf;
        }
    }
}
