using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Struct TaskAwaiter
    /// </summary>
    /// <seealso cref="System.Runtime.CompilerServices.ICriticalNotifyCompletion" />
    public struct TaskAwaiter : ICriticalNotifyCompletion
    {
        private Task _t;
        private SynchronizationContext _capturedContext;

        internal TaskAwaiter(Task t)
        {
            _t = t;
            _capturedContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Schedules the continuation action to be invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param>
        public void OnCompleted(Action continuation) => OnCompletedInternal(_t, continuation, _capturedContext);

        /// <summary>
        /// Schedules the continuation action to be invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param>
        /// Unlike <see cref="INotifyCompletion.OnCompleted" />, <see cref="UnsafeOnCompleted" /> need not propagate
        /// <see cref="ExecutionContext" /> information.
        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        /// <summary>
        /// Gets a value indicating whether this instance is completed.
        /// </summary>
        /// <value><c>true</c> if this instance is completed; otherwise, <c>false</c>.</value>
        public bool IsCompleted => _t.IsCompleted;

        /// <summary>
        /// Gets the result.
        /// </summary>
        public void GetResult() => _t.GetResult();

        internal static void OnCompletedInternal(Task t, Action continuation, SynchronizationContext capturedContext)
        {
            t.OnCompleted(() =>
            {
                if (capturedContext == null)
                {
                    continuation();
                }
                else
                {
                    capturedContext.Post(state => ((Action)state)(), continuation);
                }
            });
        }
    }
}