using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2APIEditor.Utility
{
    public class WWWHandler : ContinuationManager
    {
        internal static List<WWW> wwws = new List<WWW>();
        /*public static void Handle(WWW www, Action finishedAction)
        { //Obsolete
            AddJob(() => www.isDone, finishedAction);
        }*/
        public static void Add(WWW www)
        {
            wwws.Add(www);
        }
        public static void Add(WWW[] wws)
        {
            foreach(WWW www in wws)
                wwws.Add(www);
        }
        public static void Start(Action finishedAction)
        {
            if (wwws.Count > 1)
            {
                for (int i = 0; i < wwws.Count - 1; ++i)
                    AddJob(() => wwws[i].isDone);
                Action a = delegate {
                    finishedAction();
                    start = false;
                };
                AddJob(() => wwws.Last().isDone, a);
                start = true;
            }
            else if (wwws.Count == 1)
                AddJob(() => wwws[0].isDone, finishedAction);
            else
                Debug.LogError("You have to add any WWW value to start!");
        }
    }
    public class ContinuationManager
    {
        protected static bool start = true;
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
        protected static void AddJob(Func<bool> completed, Action continueWith = null)
        {
            if (!jobs.Any()) EditorApplication.update += Update;
            jobs.Add(new Job(completed, continueWith));
        }

        private static void Update()
        {
            if (!start)
                return;
            for (int i = 0; i >= 0; --i)
            {
                var jobIt = jobs[i];
                if (jobIt.Completed())
                {
                    jobIt.ContinueWith?.Invoke();
                    jobs.RemoveAt(i);
                }
            }
            if (!jobs.Any()) EditorApplication.update -= Update;
        }
    }
}