using Lerp2API.DebugHandler;
using System;
using System.IO;
using System.Text;

namespace Lerp2API.SafeECalls
{
    public enum LoggerType
    {
        INFO,
        WARN,
        ERROR
    }
    public class Logger : SafeECall
    {
        public string path;
        public bool saveOnGoing;
        private StringBuilder sb;

        public Logger(string p, bool sog = true)
        {
            path = p;
            saveOnGoing = sog;
        }

        public void Log()
        {
            Log("");
        }
        
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
            catch (Exception ex)
            { //Si hay problemas mostramos el por defecto.
                Console.WriteLine(msg);
            }
            AppendLine(message);
        }

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
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
            AppendLine(message);
        }

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
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
            AppendLine(message);
        }

        public void Save()
        {

        }

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
