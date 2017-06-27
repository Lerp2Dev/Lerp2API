namespace System.Threading
{
    using Tasks;

    /// <summary>
    /// Simplified implementation of SemaphoreSlim.
    ///
    /// </summary>
    public class SemaphoreSlim : IDisposable
    {
        private object _lockObj = new object();
        private int _currentCount;
        private TaskNode _head;
        private TaskNode _tail;

        /// <summary>
        /// Initializes a new instance of the <see cref="SemaphoreSlim"/> class.
        /// </summary>
        /// <param name="initialCount">The initial count.</param>
        public SemaphoreSlim(int initialCount)
        {
            _currentCount = initialCount;
        }

        /// <summary>
        /// Gets the current count.
        /// </summary>
        /// <value>The current count.</value>
        public int CurrentCount => _currentCount;

        /// <summary>
        /// <see cref="TaskCompletionSource{TResult}"/>-derived linked list node so as to have links intrusively。
        /// </summary>
        private class TaskNode : TaskCompletionSource<bool>
        {
            /// <summary>
            /// The next
            /// </summary>
            public TaskNode Next;

            /// <summary>
            /// The cancellation
            /// </summary>
            public CancellationTokenRegistration Cancellation;
        }

        /// <summary>
        /// Waits the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public Task WaitAsync() => WaitAsync(CancellationToken.None);

        /// <summary>
        /// Waits the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public Task WaitAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            lock (_lockObj)
            {
                if (_currentCount > 0)
                {
                    --_currentCount;
                    return Task.CompletedTask;
                }
                else
                {
                    var task = new TaskNode();
                    if (_head == null)
                    {
                        _head = _tail = task;
                    }
                    else
                    {
                        _tail.Next = task;
                        _tail = task;
                    }

                    if (cancellationToken != CancellationToken.None)
                    {
                        task.Cancellation = cancellationToken.Register(() =>
                        {
                            task.TrySetCanceled();
                            if (task.Cancellation != default(CancellationTokenRegistration))
                                task.Cancellation.Dispose();
                        });
                    }

                    return task.Task;
                }
            }
        }

        /// <summary>
        /// Releases this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int Release()
        {
            TaskNode head = null;
            int count;

            do
            {
                lock (_lockObj)
                {
                    count = _currentCount;

                    if (_head == null)
                    {
                        ++_currentCount;
                        head = null;
                    }
                    else
                    {
                        head = _head;

                        if (_head == _tail)
                        {
                            _head = _tail = null;
                        }
                        else
                        {
                            _head = _head.Next;
                        }
                    }
                }
            } while (head != null && head.Task.IsCompleted);

            if (head != null)
            {
                if (head.Cancellation != default(CancellationTokenRegistration))
                    head.Cancellation.Dispose();
                Task.Run(() => head.TrySetResult(false));
            }

            return count;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            // nop, because this simplified implementation does not use WaitHandle
        }
    }
}