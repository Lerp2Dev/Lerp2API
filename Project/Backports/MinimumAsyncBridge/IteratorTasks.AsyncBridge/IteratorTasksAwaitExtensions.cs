using IteratorTasks.AsyncBridge;
using System;
using System.Threading.Tasks;

/// <summary>
/// Provides an awaiter for awaiting a <see cref="Task"/>.
/// </summary>
public static class IteratorTasksAwaitExtensions
{
    /// <summary>
    /// Gets the awaiter.
    /// </summary>
    /// <param name="task">The task.</param>
    /// <returns>TaskAwaiter.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static TaskAwaiter GetAwaiter(this Task task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        return new TaskAwaiter(task);
    }

    /// <summary>
    /// Gets the awaiter.
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="task">The task.</param>
    /// <returns>TaskAwaiter&lt;TResult&gt;.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static TaskAwaiter<TResult> GetAwaiter<TResult>(this Task<TResult> task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        return new TaskAwaiter<TResult>(task);
    }
}