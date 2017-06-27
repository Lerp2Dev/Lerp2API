using System;
using System.Linq;
using System.Threading;

//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using UnityEngine.Assertions;

namespace MinimuAsyncBridgeUnitTest
{
    /// <summary>
    /// Class UnitTestSychronizationContext.
    /// </summary>
    public class UnitTestSychronizationContext
    {
        private SingleThreadSynchronizationContext _context;

        /// <summary>
        /// Tests the initialize.
        /// </summary>
        public void TestInitialize()
        {
            _context = new SingleThreadSynchronizationContext();
        }

        /// <summary>
        /// Tests the celean.
        /// </summary>
        public void TestCelean()
        {
            _context.Stop();
        }

        private void StartOnSingleThreadContext(Func<Task> test)
        {
            var tcs = new TaskCompletionSource<object>();
            _context.Post(t => ((Func<Task>)t)().ContinueWith(result =>
            {
                if (result.IsFaulted) tcs.SetException(result.Exception);
                else if (result.IsCanceled) tcs.SetCanceled();
                else tcs.TrySetResult(null);
            }), test);

            tcs.Task.Wait();
        }

        private void ContextShouldBeSingleThread()
        {
            var sync = SynchronizationContext.Current as SingleThreadSynchronizationContext;
            Assert.AreEqual(_context, sync);

            var tid = Thread.CurrentThread.ManagedThreadId;
            Assert.AreEqual(sync.MainThreadId, tid);
        }

        private static void ContextShouldBeLost()
        {
            Assert.IsNull(SynchronizationContext.Current);
        }

        private static Task RandomDelay(Random r) => Task.Delay(r.Next(1, 5));

        private static readonly Random _defaultRandom = new Random();

        private static Task RandomDelay() => RandomDelay(_defaultRandom);

        /// <summary>
        /// Awaits the operator should preserve synchronization context.
        /// </summary>
        public void AwaitOperatorShouldPreserveSynchronizationContext()
        {
            StartOnSingleThreadContext(RunRandomTasksAsync);
        }

        private async Task RunRandomTasksAsync()
        {
            await Task.Delay(1);
            var r = new Random();
            await Task.WhenAll(Enumerable.Range(0, 50).Select(_ => RunRandomTasksAsync(r.Next())));
            ContextShouldBeSingleThread();
            await Task.WhenAny(Enumerable.Range(0, 50).Select(_ => RunRandomTasksAsync(r.Next())));
            ContextShouldBeSingleThread();
        }

        private async Task RunRandomTasksAsync(int seed)
        {
            var r = new Random(seed);
            for (int i = 0; i < 100; i++)
            {
                await RandomDelay(r);
                ContextShouldBeSingleThread();
            }
            await RandomDelay(r).ConfigureAwait(false);
            ContextShouldBeLost();
        }

        /// <summary>
        /// Contexts the should be preserved over lost method.
        /// </summary>
        public void ContextShouldBePreservedOverLostMethod()
        {
            StartOnSingleThreadContext(ContextShouldBePreservedOverLostMethodAsync);
        }

        private async Task ContextShouldBePreservedOverLostMethodAsync()
        {
            ContextShouldBeSingleThread();

            await ContextShouldNeverReturnEverAfterLostAsync();

            ContextShouldBeSingleThread();
        }

        private async Task ContextShouldNeverReturnEverAfterLostAsync()
        {
            ContextShouldBeSingleThread();

            await RandomDelay();
            ContextShouldBeSingleThread();

            await RandomDelay().ConfigureAwait(true);
            ContextShouldBeSingleThread();

            await RandomDelay().ConfigureAwait(false);
            ContextShouldBeLost();

            await RandomDelay();
            ContextShouldBeLost();

            await RandomDelay().ConfigureAwait(true);
            ContextShouldBeLost();
        }

        /// <summary>
        /// Contexts the should not be changed after completed task.
        /// </summary>
        public void ContextShouldNotBeChangedAfterCompletedTask()
        {
            StartOnSingleThreadContext(ContextShouldNotBeChangedAfterCompletedTaskAsync);
        }

        private async Task ContextShouldNotBeChangedAfterCompletedTaskAsync()
        {
            ContextShouldBeSingleThread();
            await Task.CompletedTask;
            ContextShouldBeSingleThread();
            await Task.CompletedTask.ConfigureAwait(false);
            ContextShouldBeSingleThread();
            await Task.CompletedTask.ConfigureAwait(true);
            ContextShouldBeSingleThread();
        }

        /// <summary>
        /// Awaits the operator should work on exception.
        /// </summary>
        public void AwaitOperatorShouldWorkOnException()
        {
            StartOnSingleThreadContext(AwaitOperatorShouldWorkOnExceptionAsync);
        }

        private async Task AwaitOperatorShouldWorkOnExceptionAsync()
        {
            await Task.Delay(1);
            var r = new Random();

            try
            {
                await ThrowInvalidException(r.Next());
                Assert.raiseExceptions = true;
            }
            catch
            {
            }
            ContextShouldBeSingleThread();

            try
            {
                await Task.WhenAll(Enumerable.Range(0, 50).Select(_ => ThrowInvalidException(r.Next())));
                Assert.raiseExceptions = true;
            }
            catch
            {
            }
            ContextShouldBeSingleThread();

            try
            {
                await Task.WhenAny(Enumerable.Range(0, 50).Select(_ => ThrowInvalidException(r.Next())));
                Assert.raiseExceptions = true;
            }
            catch
            {
            }
            ContextShouldBeSingleThread();

            await RandomDelay().ConfigureAwait(false);

            try
            {
                await ThrowInvalidException(r.Next());
                Assert.raiseExceptions = true;
            }
            catch
            {
            }
            ContextShouldBeLost();
        }

        private async Task ThrowInvalidException(int seed)
        {
            var r = new Random(seed);
            await RandomDelay(r).ConfigureAwait(false);
            ContextShouldBeLost();

            throw new InvalidOperationException();
        }
    }
}