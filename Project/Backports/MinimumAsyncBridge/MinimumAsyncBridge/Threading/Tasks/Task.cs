using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Represents an asynchronous operation.
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public TaskStatus Status { get; protected internal set; }

        /// <summary>
        /// Gets whether this Task has completed.
        /// </summary>
        public bool IsCompleted => Status == TaskStatus.RanToCompletion || IsCanceled || IsFaulted;

        /// <summary>
        /// Gets whether this Task instance has completed execution due to being canceled.
        /// </summary>
        public bool IsCanceled => Status == TaskStatus.Canceled;

        /// <summary>
        /// Gets whether the Task completed due to an unhandled exception.
        /// </summary>
        public bool IsFaulted => Status == TaskStatus.Faulted;

        private object _sync = new object();

        internal bool Cancel()
        {
            lock (_sync)
            {
                if (Status == TaskStatus.Running)
                {
                    Status = TaskStatus.Canceled;
                    _completed?.Invoke();
                    return true;
                }
                return false;
            }
        }

        internal bool SetException(Exception exception)
        {
            lock (_sync)
            {
                if (Status == TaskStatus.Running)
                {
                    Status = TaskStatus.Faulted;
                    MergeException(exception);
                    _completed?.Invoke();
                    return true;
                }
                return false;
            }
        }

        private void MergeException(Exception ex)
        {
            var agex = ex as AggregateException;
            if (Exception == null)
            {
                if (agex != null)
                    Exception = agex;
                else
                    Exception = new AggregateException(ex);
            }
            else
            {
                if (agex != null)
                    Exception = new AggregateException(Exception.InnerExceptions.Concat(agex.InnerExceptions).ToArray());
                else
                    Exception = new AggregateException(Exception.InnerExceptions.Concat(new[] { ex }).ToArray());
            }
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public AggregateException Exception { get; private set; }

        /// <summary>
        /// Completes the specified on complete.
        /// </summary>
        /// <param name="onComplete">The on complete.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected internal bool Complete(Action onComplete)
        {
            lock (_sync)
            {
                if (Status == TaskStatus.Running)
                {
                    Status = TaskStatus.RanToCompletion;
                    onComplete();
                    _completed?.Invoke();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Waits this instance.
        /// </summary>
        public void Wait()
        {
            while (!IsCompleted)
                Thread.Sleep(10);
            if (Exception != null)
                throw Exception;
        }

        internal void OnCompleted(Action continuation)
        {
            lock (_sync)
            {
                if (IsCompleted)
                {
                    continuation();
                    return;
                }

                Action x = null;
                x = () =>
                {
                    continuation();
                    _completed -= x;
                };

                _completed += x;
            }
        }

        private Action _completed;

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns>TaskAwaiter.</returns>
        public TaskAwaiter GetAwaiter() => new TaskAwaiter(this);

        /// <summary>
        /// Configures the await.
        /// </summary>
        /// <param name="continueOnCapturedContext">if set to <c>true</c> [continue on captured context].</param>
        /// <returns>ConfiguredTaskAwaitable.</returns>
        public ConfiguredTaskAwaitable ConfigureAwait(bool continueOnCapturedContext) => new ConfiguredTaskAwaitable(this, continueOnCapturedContext);

        internal void GetResult()
        {
            if (Exception != null)
                throw Exception.InnerExceptions.First();

            if (IsCanceled)
                throw new TaskCanceledException();
        }

        /// <summary>
        /// Froms the result.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>Task&lt;TResult&gt;.</returns>
        public static Task<TResult> FromResult<TResult>(TResult value)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        /// <summary>
        /// Gets the completed task.
        /// </summary>
        /// <value>The completed task.</value>
        public static Task CompletedTask { get; } = FromResult<object>(null);

        /// <summary>
        /// Froms the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>Task.</returns>
        public static Task FromException(Exception exception)
        {
            var tcs = new TaskCompletionSource<object>();
            tcs.SetException(exception);
            return tcs.Task;
        }

        /// <summary>
        /// Froms the exception.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="exception">The exception.</param>
        /// <returns>Task&lt;TResult&gt;.</returns>
        public static Task<TResult> FromException<TResult>(Exception exception)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(exception);
            return tcs.Task;
        }

        /// <summary>
        /// Whens any.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <returns>Task&lt;Task&gt;.</returns>
        public static Task<Task> WhenAny(IEnumerable<Task> tasks) => WhenAny(tasks.ToArray());

        /// <summary>
        /// Whens any.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <returns>Task&lt;Task&gt;.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Task<Task> WhenAny(params Task[] tasks)
        {
            if (tasks == null) throw new ArgumentNullException(nameof(tasks));
            if (tasks.Length == 0) throw new ArgumentException(nameof(tasks) + " empty", nameof(tasks));

            var tcs = new TaskCompletionSource<Task>();

            foreach (var t in tasks)
            {
                if (t.IsCompleted)
                {
                    tcs.TrySetResult(t);
                    break;
                }

                t.OnCompleted(() => tcs.TrySetResult(t));
            }

            return tcs.Task;
        }

        /// <summary>
        /// Whens any.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="tasks">The tasks.</param>
        /// <returns>Task&lt;Task&lt;TResult&gt;&gt;.</returns>
        public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks) => WhenAny(tasks.ToArray());

        /// <summary>
        /// Whens any.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="tasks">The tasks.</param>
        /// <returns>Task&lt;Task&lt;TResult&gt;&gt;.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
        {
            if (tasks == null) throw new ArgumentNullException(nameof(tasks));
            if (tasks.Length == 0) throw new ArgumentException(nameof(tasks) + " empty", nameof(tasks));

            var tcs = new TaskCompletionSource<Task<TResult>>();

            foreach (var t in tasks)
            {
                if (t.IsCompleted)
                {
                    tcs.TrySetResult(t);
                    break;
                }

                t.OnCompleted(() => tcs.TrySetResult(t));
            }

            return tcs.Task;
        }

        /// <summary>
        /// Whens all.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <returns>Task.</returns>
        public static Task WhenAll(IEnumerable<Task> tasks) => WhenAll(tasks.ToArray());

        /// <summary>
        /// Whens all.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task WhenAll(params Task[] tasks)
        {
            if (tasks == null) throw new ArgumentNullException(nameof(tasks));
            if (tasks.Length == 0) return CompletedTask;

            var tcs = new TaskCompletionSource<object>();
            var exceptions = new AggregateException[tasks.Length];
            int count = 0;

            for (int j = 0; j < tasks.Length; j++)
            {
                var index = j;
                var t = tasks[index];

                if (t.IsCompleted)
                {
                    if (t.IsFaulted)
                        lock (exceptions)
                            exceptions[index] = t.Exception;

                    CheckWhenAllCompletetion(tasks, tcs, null, exceptions, ref count);
                }
                else
                {
                    t.OnCompleted(() =>
                    {
                        if (t.IsFaulted)
                            lock (exceptions)
                                exceptions[index] = t.Exception;

                        CheckWhenAllCompletetion(tasks, tcs, null, exceptions, ref count);
                    });
                }
            }

            return tcs.Task;
        }

        private static void CheckWhenAllCompletetion<TResult>(Task[] tasks, TaskCompletionSource<TResult> tcs, TResult result, AggregateException[] exceptions, ref int count)
        {
            Interlocked.Increment(ref count);
            if (count == tasks.Length)
            {
                bool any;
                Exception[] innerExceptions;
                lock (exceptions)
                {
                    innerExceptions = exceptions.Where(x => x != null).SelectMany(x => x.InnerExceptions).ToArray();
                    any = innerExceptions.Any();
                }
                if (any)
                    tcs.TrySetException(new AggregateException(innerExceptions));
                else if (tasks.Any(x => x.IsCanceled))
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(result);
            }
        }

        /// <summary>
        /// Whens all.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="tasks">The tasks.</param>
        /// <returns>Task&lt;TResult[]&gt;.</returns>
        public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks) => WhenAll(tasks.ToArray());

        /// <summary>
        /// Whens all.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="tasks">The tasks.</param>
        /// <returns>Task&lt;TResult[]&gt;.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
        {
            if (tasks == null) throw new ArgumentNullException(nameof(tasks));
            if (tasks.Length == 0) return FromResult(new TResult[0]);

            var tcs = new TaskCompletionSource<TResult[]>();
            var exceptions = new AggregateException[tasks.Length];
            var results = new TResult[tasks.Length];
            int count = 0;

            for (var j = 0; j < tasks.Length; j++)
            {
                var index = j;
                var t = tasks[index];

                if (t.IsCompleted)
                {
                    if (t.IsFaulted)
                        lock (exceptions)
                            exceptions[index] = t.Exception;
                    else if (!t.IsCanceled)
                        results[index] = t.Result;

                    CheckWhenAllCompletetion(tasks, tcs, results, exceptions, ref count);
                }
                else
                {
                    t.OnCompleted(() =>
                    {
                        if (t.IsFaulted)
                            lock (exceptions)
                                exceptions[index] = t.Exception;
                        else if (!t.IsCanceled)
                            results[index] = t.Result;

                        CheckWhenAllCompletetion(tasks, tcs, results, exceptions, ref count);
                    });
                }
            }

            return tcs.Task;
        }

        /// <summary>
        /// Continues the with.
        /// </summary>
        /// <param name="continuationAction">The continuation action.</param>
        /// <returns>Task.</returns>
        public Task ContinueWith(Action<Task> continuationAction)
        {
            var tcs = new TaskCompletionSource<object>();
            OnCompleted(() =>
            {
                try
                {
                    continuationAction(this);
                    tcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });
            return tcs.Task;
        }

        /// <summary>
        /// Continues the with.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="continuationFunction">The continuation function.</param>
        /// <returns>Task&lt;TResult&gt;.</returns>
        public Task<TResult> ContinueWith<TResult>(Func<Task, TResult> continuationFunction)
        {
            var tcs = new TaskCompletionSource<TResult>();
            OnCompleted(() =>
            {
                try
                {
                    var r = continuationFunction(this);
                    tcs.TrySetResult(r);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });
            return tcs.Task;
        }

        private static int CheckedTotalMilliseconds(TimeSpan t)
        {
            var totalMilliseconds = (long)t.TotalMilliseconds;
            if (totalMilliseconds < 0 || totalMilliseconds > int.MaxValue)
                throw new ArgumentOutOfRangeException(t.ToString());
            return (int)totalMilliseconds;
        }

        /// <summary>
        /// Delays the specified delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <returns>Task.</returns>
        public static Task Delay(TimeSpan delay) => Delay(CheckedTotalMilliseconds(delay), CancellationToken.None);

        /// <summary>
        /// Delays the specified delay.
        /// </summary>
        /// <param name="delay">The delay.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public static Task Delay(TimeSpan delay, CancellationToken cancellationToken) => Delay(CheckedTotalMilliseconds(delay), cancellationToken);

        /// <summary>
        /// Delays the specified milliseconds delay.
        /// </summary>
        /// <param name="millisecondsDelay">The milliseconds delay.</param>
        /// <returns>Task.</returns>
        public static Task Delay(int millisecondsDelay) => Delay(millisecondsDelay, CancellationToken.None);

        /// <summary>
        /// Delays the specified milliseconds delay.
        /// </summary>
        /// <param name="millisecondsDelay">The milliseconds delay.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The millisecondsDelay argument is must be 0 or greater. " + millisecondsDelay</exception>
        public static Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            var task = tcs.Task;
            var ctr = default(CancellationTokenRegistration);

            if (cancellationToken.IsCancellationRequested)
            {
                tcs.SetCanceled();
                return task;
            }

            if (millisecondsDelay == 0)
            {
                tcs.SetResult(false);
                return task;
            }
            if (millisecondsDelay < 0)
            {
                throw new ArgumentOutOfRangeException("The millisecondsDelay argument is must be 0 or greater. " + millisecondsDelay);
            }

            Timer t = null;

            Action<bool> stop = (canceled) =>
            {
                var t1 = Interlocked.Exchange(ref t, null);

                if (t1 != null)
                {
                    t1.Dispose();
                    if (canceled) tcs.TrySetCanceled();
                    else tcs.TrySetResult(false);
                    tcs = null;
                    if (ctr != default(CancellationTokenRegistration)) ctr.Dispose();
                    ctr = default(CancellationTokenRegistration);
                }
            };

            t = new Timer(_ => stop(false), null, Timeout.Infinite, Timeout.Infinite);

            if (cancellationToken != CancellationToken.None)
            {
                ctr = cancellationToken.Register(() => stop(true));
            }

            t?.Change(millisecondsDelay, Timeout.Infinite);

            return task;
        }

        /// <summary>
        /// Runs the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Task.</returns>
        public static Task Run(Action action)
        {
            var tcs0 = new TaskCompletionSource<bool>();
            ThreadPool.QueueUserWorkItem(x =>
            {
                var tcs = (TaskCompletionSource<bool>)x;
                try
                {
                    action();
                    tcs.TrySetResult(false);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, tcs0);

            return tcs0.Task;
        }

        /// <summary>
        /// Runs the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="function">The function.</param>
        /// <returns>Task&lt;TResult&gt;.</returns>
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
            var tcs0 = new TaskCompletionSource<TResult>();
            ThreadPool.QueueUserWorkItem(x =>
            {
                var tcs = (TaskCompletionSource<TResult>)x;
                try
                {
                    var result = function();
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, tcs0);

            return tcs0.Task;
        }
    }
}