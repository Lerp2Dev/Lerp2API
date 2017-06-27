using Lerp2API.SafeECalls;
using System;
using System.Threading;

namespace Lerp2API.Hepers.Debug_Utils
{
    /// <summary>
    /// Class DebugHelpers.
    /// </summary>
    public static class DebugHelpers
    {
        /// <summary>
        /// Detaileds the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="t">The t.</param>
        /// <returns>System.String.</returns>
        public static string DetailedMessage(this string message, LoggerType t)
        {
            return string.Format("[{0}] [{1}/{2}: {3}]", DateTime.Now.ToString("hh:mm:ss"), Thread.CurrentThread.Name, t.ToString(), message);
        }
    }
}