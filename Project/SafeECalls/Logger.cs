using Lerp2API._Debug;
using Lerp2API.Hepers.Debug_Utils;
using System;
using System.IO;
using System.Text;

namespace Lerp2API.SafeECalls
{
    /// <summary>
    /// Class Logger.
    /// </summary>
    /// <seealso cref="Lerp2API.SafeECalls.SafeECall" />
    public class Logger : SafeECall
    {
        /// <summary>
        /// The path
        /// </summary>
        public string path;
        /// <summary>
        /// The save on going
        /// </summary>
        public bool saveOnGoing;
        private StringBuilder sb;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="sog">if set to <c>true</c> [sog].</param>
        public Logger(string p, bool sog = true)
        {
            path = p;
            saveOnGoing = sog;
        }

        /// <summary>
        /// Logs this instance.
        /// </summary>
        public void Log()
        {
            Log("");
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="pars">The pars.</param>
        public void Log(string message, params object[] pars)
        {
            string str = pars != null ? string.Format(message, pars) : message,
                   msg = str.DetailedMessage(LoggerType.INFO);
            try
            { //Probamos a hacer un debug...
                if (LerpedCore.safeECallEnabled)
                    Console.WriteLine(msg);
                else
                    Debug.Log(str);
            }
            catch
            { //Si hay problemas mostramos el por defecto.
                Console.WriteLine(msg);
            }
            AppendLine(message);
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="pars">The pars.</param>
        public void LogWarning(string message, params object[] pars)
        {
            string str = pars != null ? string.Format(message, pars) : message,
                   msg = str.DetailedMessage(LoggerType.WARN);
            try
            {
                if (LerpedCore.safeECallEnabled)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(msg);
                    Console.ResetColor();
                }
                else
                    Debug.LogWarning(str);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
            AppendLine(message);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="pars">The pars.</param>
        public void LogError(string message, params object[] pars)
        {
            string str = pars != null ? string.Format(message, pars) : message,
                   msg = str.DetailedMessage(LoggerType.ERROR);
            try
            {
                if (LerpedCore.safeECallEnabled)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(msg);
                    Console.ResetColor();
                }
                else
                    Debug.LogError(str);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
            AppendLine(message);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {

        }

        /// <summary>
        /// Saves to file.
        /// </summary>
        public void SaveToFile()
        {

        }

        internal void AppendLine(string str)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (!File.Exists(path))
                    File.Create(path).Dispose();
                File.AppendAllText(path, str + Environment.NewLine);
            }
        }
    }
}
