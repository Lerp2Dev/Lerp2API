using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using Debug = Lerp2API.DebugHandler.Debug;

namespace Lerp2API.Utility
{
    public class WWWHandler
    {
        internal List<WWW> wwws = new List<WWW>();
        internal ContinuationManager<WWW> worker = new ContinuationManager<WWW>();
        public void Add(WWW www)
        {
            SharedAdd<WWW>(new WWW[] { www }, false, null);
        }
        public void Add(WWW[] wws)
        {
            SharedAdd<WWW[]>(wws, false, null);
        }
        public void AddAndStart(WWW www, bool play, Action<WWW> action)
        {
            SharedAdd(new WWW[] { www }, play, action);
        }
        public void AddAndStart(WWW[] wws, bool play, Action<WWW[]> action)
        {
            SharedAdd(wws, play, action);
        }
        private void SharedAdd<T>(WWW[] wws, bool play, Action<T> action)
        {
            foreach (WWW www in wws)
                wwws.Add(www);
            if (action != null)
                Start(play, action);
        }
        public void Start<T>(bool play, Action<T> finishedAction)
        {
            Action<WWW[]> finalAction = (x) =>
            {
                if (typeof(T).IsArray)
                    finishedAction((T)(object)x);
                else
                    finishedAction((T)(object)x[0]);
                //EditorApplication.update -= worker.JobWorker;
            };
            if (wwws.Count > 0)
                worker.SetJob(wwws.ToArray(), () => wwws.All(x => x.isDone), finalAction);
            else
            {
                Debug.LogError("You have to add any WWW value to start!");
                return;
            }
            worker.Start(play);
            //EditorApplication.update += worker.JobWorker; //This is better to have inside of the worker class, but I profer to do this as that because, I like to see it clearly
        }
    }
    public class ContinuationManager<T>
    {
        private class Job
        {
            public Job(T[] objs, Func<bool> completed, Action<T[]> finish)
            {
                Completed = completed;
                Finish = finish;
                Objects = objs;
            }
            public Func<bool> Completed;
            public Action<T[]> Finish;
            public T[] Objects;
        }

        private static int lastStartJobId = -1;

        private Job job;
        public int assignedJobId = -1;

        public void SetJob(T[] objs, Func<bool> completed, Action<T[]> continueWith)
        {
            job = new Job(objs, completed, continueWith);
        }
        public void Start(bool forcePlay = false)
        {
            ++lastStartJobId;
            assignedJobId = lastStartJobId;
            if (!forcePlay)
            {
                if (Application.isEditor)
                    JobEditor();
                else
                    GameLoopEntry.StartCoroutine(JobPlaying());
            }
            else
                GameLoopEntry.StartCoroutine(JobPlaying());
        }

        internal void JobEditor()
        {
            //Debug.Log("Editor job is completed?:" + job.Completed());
            while (!job.Completed())
            {
                JobWorker();
                Thread.Sleep(1000);
            }
            JobWorker(false);
        }

        internal IEnumerator JobPlaying()
        {
            //Debug.Log("Play job is completed?:" + job.Completed());
            while (!job.Completed())
            {
                JobWorker();
                yield return new WaitForSeconds(1);
            }
            JobWorker(false);
        }

        internal void JobWorker(bool working = true)
        {
            if (!working)
                job.Finish(job.Objects);
        }
    }
}