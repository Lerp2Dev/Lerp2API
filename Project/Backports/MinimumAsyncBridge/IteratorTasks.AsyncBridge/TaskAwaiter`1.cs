using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IteratorTasks.AsyncBridge
{
    /// <summary>
    /// Struct TaskAwaiter
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <seealso cref="System.Runtime.CompilerServices.INotifyCompletion" />
    public struct TaskAwaiter<TResult> : INotifyCompletion
    {
        private readonly Task<TResult> _t;
        private Task<TResult> task;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskAwaiter{TResult}"/> struct.
        /// </summary>
        /// <param name="task">The task.</param>
        public TaskAwaiter(Task<TResult> task) : this()
        {
            this.task = task;
        }

        //internal TaskAwaiter(Task<TResult> t) { _t = t; }

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
        /// <returns>TResult.</returns>
        public TResult GetResult() => _t.Result;
    }
}