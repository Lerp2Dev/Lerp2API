//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace MinimuAsyncBridgeUnitTest
{
    /// <summary>
    /// <see cref="SynchronizationContext"/> which has a message pump on a single thread.
    /// </summary>
    public class SingleThreadSynchronizationContext : SynchronizationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleThreadSynchronizationContext"/> class.
        /// </summary>
        public SingleThreadSynchronizationContext()
        {
            _cts = new CancellationTokenSource();
            _thread = new Thread(() => UpdateLoop(_cts.Token));
            _thread.Start();
        }

        private Thread _thread;
        private CancellationTokenSource _cts;

        private void UpdateLoop(CancellationToken ct)
        {
            MainThreadId = Thread.CurrentThread.ManagedThreadId;
            SetSynchronizationContext(this);

            while (!ct.IsCancellationRequested)
            {
                Update();

                if (Count == 0)
                    Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
            _thread.Join();
        }

        private List<Action> _actions = new List<Action>();

        /// <summary>
        /// <see cref="SynchronizationContext.Post(SendOrPostCallback, object)"/>
        /// </summary>
        /// <param name="d"></param>
        /// <param name="state"></param>
        public override void Post(SendOrPostCallback d, object state)
        {
            lock (_actions)
            {
                _actions.Add(() => d(state));
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count => _actions.Count;

        /// <summary>
        /// Gets the main thread identifier.
        /// </summary>
        /// <value>The main thread identifier.</value>
        public int MainThreadId { get; private set; }

        /// <summary>
        /// Assume this method is called on a single thread。
        /// </summary>
        private void Update()
        {
            while (true)
            {
                Action[] actions;

                lock (_actions)
                {
                    if (_actions.Count == 0)
                        break;

                    actions = _actions.ToArray();
                    _actions.Clear();
                }

                if (!actions.Any())
                    break;

                foreach (var a in actions)
                {
                    try
                    {
                        a();
                    }
                    catch (Exception ex)
                    {
                        Log(ex);
                    }
                }
            }
        }

        private void Log(Exception ex)
        {
            if (ex is OperationCanceledException
                || ex is TaskCanceledException)
                return;

            //Assert.Fail();
            Assert.raiseExceptions = true;
        }
    }
}