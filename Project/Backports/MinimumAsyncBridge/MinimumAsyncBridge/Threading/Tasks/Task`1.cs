using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Threading.Tasks
{
    /// <summary>
    /// Represents an asynchronous operation that can return a value.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class Task<TResult> : Task
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        /// <exception cref="AggregateException"></exception>
        /// <exception cref="TaskCanceledException"></exception>
        public TResult Result
        {
            get
            {
                if (Status == TaskStatus.Running)
                    Wait();

                if (Exception != null)
                    throw Exception;

                if (IsCanceled)
                    throw new AggregateException(new TaskCanceledException());

                return GetResult();
            }
            private set { _result = value; }
        }

        private TResult _result;

        internal bool SetResult(TResult result)
        {
            return Complete(() => Result = result);
        }

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns>TaskAwaiter&lt;TResult&gt;.</returns>
        public new TaskAwaiter<TResult> GetAwaiter() => new TaskAwaiter<TResult>(this);

        /// <summary>
        /// Configures the await.
        /// </summary>
        /// <param name="continueOnCapturedContext">if set to <c>true</c> [continue on captured context].</param>
        /// <returns>ConfiguredTaskAwaitable&lt;TResult&gt;.</returns>
        public new ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext) => new ConfiguredTaskAwaitable<TResult>(this, continueOnCapturedContext);

        internal new TResult GetResult()
        {
            if (Exception != null)
                throw Exception.InnerExceptions.First();

            if (IsCanceled)
                throw new TaskCanceledException();

            return _result;
        }

        /// <summary>
        /// Continues the with.
        /// </summary>
        /// <param name="continuationAction">The continuation action.</param>
        /// <returns>Task.</returns>
        public Task ContinueWith(Action<Task<TResult>> continuationAction)
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
        /// <typeparam name="TNewResult">The type of the t new result.</typeparam>
        /// <param name="continuationFunction">The continuation function.</param>
        /// <returns>Task&lt;TNewResult&gt;.</returns>
        public Task<TNewResult> ContinueWith<TNewResult>(Func<Task<TResult>, TNewResult> continuationFunction)
        {
            var tcs = new TaskCompletionSource<TNewResult>();
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
    }
}