using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IteratorTasks.AsyncBridge
{
    /// <summary>
    /// Struct TaskAwaiter
    /// </summary>
    /// <seealso cref="System.Runtime.CompilerServices.INotifyCompletion" />
    public struct TaskAwaiter : INotifyCompletion
    {
        private readonly Task _t;

        internal TaskAwaiter(Task t)
        {
            _t = t;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is completed.
        /// </summary>
        /// <value><c>true</c> if this instance is completed; otherwise, <c>false</c>.</value>
        public bool IsCompleted => _t.IsCompleted;

        /// <summary>
        /// Schedules the continuation action to be invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param>
        public void OnCompleted(Action continuation)
        {
            if (_t.IsCompleted)
                continuation();
            else
                _t.ContinueWith(_ => continuation());
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public void GetResult()
        {
            if (_t.Exception != null)
                throw _t.Exception;
        }
    }
}