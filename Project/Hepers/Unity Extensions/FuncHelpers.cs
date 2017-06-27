using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Debug = Lerp2API._Debug.Debug;

namespace Lerp2API.Hepers.Unity_Extensions
{
    /// <summary>
    /// Class FuncHepers.
    /// </summary>
    public static class FuncHepers
    {
        /// <summary>
        /// Tries the action.
        /// </summary>
        /// <typeparam name="TResult">The type of the t result.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>TResult.</returns>
        public static TResult TryAction<TResult>(this Func<TResult> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return default(TResult);
            }
        }

        /// <summary>
        /// Gets the current method.
        /// </summary>
        /// <returns>System.String.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }
    }
}