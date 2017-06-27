using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Struct ConfiguredTaskAwaitable
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    public struct ConfiguredTaskAwaitable<TResult>
    {
        private readonly ConfiguredTaskAwaiter _configuredTaskAwaiter;

        internal ConfiguredTaskAwaitable(Task<TResult> t, bool continueOnCapturedContext)
        {
            _configuredTaskAwaiter = new ConfiguredTaskAwaiter(t, continueOnCapturedContext);
        }

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns>ConfiguredTaskAwaiter.</returns>
        public ConfiguredTaskAwaiter GetAwaiter() => _configuredTaskAwaiter;

        /// <summary>
        /// Struct ConfiguredTaskAwaiter
        /// </summary>
        /// <seealso cref="System.Runtime.CompilerServices.ICriticalNotifyCompletion" />
        /// <seealso cref="System.Runtime.CompilerServices.INotifyCompletion" />
        public struct ConfiguredTaskAwaiter : ICriticalNotifyCompletion, INotifyCompletion
        {
            private Task<TResult> _t;
            private SynchronizationContext _capturedContext;

            internal ConfiguredTaskAwaiter(Task<TResult> t, bool continueOnCapturedContext)
            {
                _t = t;
                _capturedContext = continueOnCapturedContext ? SynchronizationContext.Current : null;
            }

            /// <summary>
            /// Schedules the continuation action to be invoked when the instance completes.
            /// </summary>
            /// <param name="continuation">The action to invoke when the operation completes.</param>
            public void OnCompleted(Action continuation) => TaskAwaiter.OnCompletedInternal(_t, continuation, _capturedContext);

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
            /// <returns>TResult.</returns>
            public TResult GetResult() => _t.GetResult();
        }
    }
}