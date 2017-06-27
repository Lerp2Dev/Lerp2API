using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2API.Utility
{
    /// <summary>
    /// Class WWWHandler.
    /// </summary>
    public class WWWHandler
    {
        internal List<WWW> wwws = new List<WWW>();
        internal ContinuationManager<WWW> worker = new ContinuationManager<WWW>();
        /// <summary>
        /// Adds the specified WWW.
        /// </summary>
        /// <param name="www">The WWW.</param>
        public void Add(WWW www)
        {
            SharedAdd<WWW>(new WWW[] { www }, false, null);
        }
        /// <summary>
        /// Adds the specified WWS.
        /// </summary>
        /// <param name="wws">The WWS.</param>
        public void Add(WWW[] wws)
        {
            SharedAdd<WWW[]>(wws, false, null);
        }
        /// <summary>
        /// Adds the and start.
        /// </summary>
        /// <param name="www">The WWW.</param>
        /// <param name="play">if set to <c>true</c> [play].</param>
        /// <param name="action">The action.</param>
        public void AddAndStart(WWW www, bool play, Action<WWW> action)
        {
            SharedAdd(new WWW[] { www }, play, action);
        }
        /// <summary>
        /// Adds the and start.
        /// </summary>
        /// <param name="wws">The WWS.</param>
        /// <param name="play">if set to <c>true</c> [play].</param>
        /// <param name="action">The action.</param>
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
        /// <summary>
        /// Starts the specified play.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="play">if set to <c>true</c> [play].</param>
        /// <param name="finishedAction">The finished action.</param>
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
    /// <summary>
    /// Class ContinuationManager.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ContinuationManager<T>
    {
        private class Job
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Job"/> class.
            /// </summary>
            /// <param name="objs">The objs.</param>
            /// <param name="completed">The completed.</param>
            /// <param name="finish">The finish.</param>
            public Job(T[] objs, Func<bool> completed, Action<T[]> finish)
            {
                Completed = completed;
                Finish = finish;
                Objects = objs;
            }
            /// <summary>
            /// The completed
            /// </summary>
            public Func<bool> Completed;
            /// <summary>
            /// The finish
            /// </summary>
            public Action<T[]> Finish;
            /// <summary>
            /// The objects
            /// </summary>
            public T[] Objects;
        }

        private static int lastStartJobId = -1;

        private Job job;
        /// <summary>
        /// The assigned job identifier
        /// </summary>
        public int assignedJobId = -1;

        /// <summary>
        /// Sets the job.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <param name="completed">The completed.</param>
        /// <param name="continueWith">The continue with.</param>
        public void SetJob(T[] objs, Func<bool> completed, Action<T[]> continueWith)
        {
            job = new Job(objs, completed, continueWith);
        }
        /// <summary>
        /// Starts the specified force play.
        /// </summary>
        /// <param name="forcePlay">if set to <c>true</c> [force play].</param>
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