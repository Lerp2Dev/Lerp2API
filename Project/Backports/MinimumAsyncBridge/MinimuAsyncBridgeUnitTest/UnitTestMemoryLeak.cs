//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using static System.Reflection.BindingFlags;

namespace MinimuAsyncBridgeUnitTest
{
    /// <summary>
    /// Class UnitTestMemoryLeak.
    /// </summary>
    public class UnitTestMemoryLeak
    {
        private List<WeakReference> _refs = new List<WeakReference>();

        private void Add<T>(T item)
        {
            lock (_refs)
            {
                _refs.Add(new WeakReference(item));
            }
        }

        private void AllReferencesShouldBeGarbageCollected()
        {
            GC.Collect(2, GCCollectionMode.Forced);

            lock (_refs)
            {
                foreach (var r in _refs)
                    Assert.IsNull(r.Target);
                _refs.Clear();
            }
        }

        [System.Diagnostics.Conditional("V35")]
        private static void CancellationTokenSourceShouldHaveNoEventListener(CancellationTokenSource cts)
        {
            var f = cts.GetType().GetField("m_linkingRegistrations", NonPublic | Instance);
            var v = f.GetValue(cts);
            Assert.IsNull(v);
        }

        /// <summary>
        /// Tests the task run.
        /// </summary>
        public void TestTaskRun()
        {
            TaskRun().Wait();
            AllReferencesShouldBeGarbageCollected();
        }

        private async Task TaskRun()
        {
            for (int i = 0; i < 1000; i++)
            {
                var t = Task.Run(() => { });
                Add(t);
                await t;
            }
        }

        /// <summary>
        /// Tests the task delay.
        /// </summary>
        public void TestTaskDelay()
        {
            TaskDelay().Wait();
            AllReferencesShouldBeGarbageCollected();
        }

        /// <summary>
        /// Tests the task delay without cancel.
        /// </summary>
        public void TestTaskDelayWithoutCancel()
        {
            var cts = new CancellationTokenSource();
            TaskDelay(cts.Token).Wait();
            CancellationTokenSourceShouldHaveNoEventListener(cts);
            AllReferencesShouldBeGarbageCollected();
        }

        /// <summary>
        /// Tests the task delay with cancel.
        /// </summary>
        public void TestTaskDelayWithCancel()
        {
            var cts = new CancellationTokenSource();
            var t = TaskDelay(cts.Token);
            cts.Cancel();
            t.Wait();
            CancellationTokenSourceShouldHaveNoEventListener(cts);
            AllReferencesShouldBeGarbageCollected();
        }

        private async Task TaskDelay()
        {
            for (int i = 0; i < 100; i++)
            {
                var t = Task.Delay(1);
                Add(t);
                await t;
            }
        }

        private async Task TaskDelay(CancellationToken ct)
        {
            for (int i = 0; i < 50; i++)
            {
                var t = Task.Delay(1, ct);
                Add(t);
                try
                {
                    await t;
                }
                catch (OperationCanceledException) { }
            }
        }
    }
}