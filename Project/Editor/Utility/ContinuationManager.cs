using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2APIEditor.Utility
{
    public static class WWWHandler
    {
        internal static List<WWW> wwws = new List<WWW>();
        public static void Handle(WWW www, Action finishedAction)
        {
            ContinuationManager.Add(() => www.isDone, finishedAction);
        }
        public static void Add(WWW www)
        {
            wwws.Add(www);
        }
        public static void Start(Action finishedAction)
        {
            if (wwws.Count > 1)
            {
                for(int i = 0; i < wwws.Count - 1; ++i)
                    ContinuationManager.Add(() => wwws[i].isDone);
                ContinuationManager.Add(() => wwws.Last().isDone, finishedAction);
            }
            else if (wwws.Count == 1)
                ContinuationManager.Add(() => wwws[0].isDone, finishedAction);
            else
                Debug.LogError("You have to add any WWW value to start!");
        }
    }
    internal static class ContinuationManager
    {
        private class Job
        {
            public Job(Func<bool> completed, Action continueWith)
            {
                Completed = completed;
                ContinueWith = continueWith;
            }
            public Func<bool> Completed { get; private set; }
            public Action ContinueWith { get; private set; }
        }

        private static readonly List<Job> jobs = new List<Job>();
        public static void Add(Func<bool> completed, Action continueWith = null)
        {
            if (!jobs.Any()) EditorApplication.update += Update;
            jobs.Add(new Job(completed, continueWith));
        }

        private static void Update()
        {
            for (int i = 0; i >= 0; --i)
            {
                var jobIt = jobs[i];
                if (jobIt.Completed())
                {
                    if(jobIt.ContinueWith != null)
                        jobIt.ContinueWith();
                    jobs.RemoveAt(i);
                }
            }
            if (!jobs.Any()) EditorApplication.update -= Update;
        }
    }
}