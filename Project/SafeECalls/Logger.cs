using Lerp2API.DebugHandler;
using System;
using System.IO;
using System.Text;
using System.Threading;

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
            string msg = pars != null ? string.Format(message, pars) : message;
            try
            { //Probamos a hacer un debug...
                if (LerpedCore.safeECallEnabled)
                    Console.WriteLine(msg);
                else
                    Debug.Log(msg);
            }
            catch (Exception ex)
            { //Si hay problemas mostramos el por defecto.
                Console.WriteLine(msg);
            }
            AppendLine(message, LoggerType.INFO);
        }

        public void LogWarning(string message, params object[] pars)
        {
            string msg = pars != null ? string.Format(message, pars) : message;
            try
            {
                if (LerpedCore.safeECallEnabled)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(msg);
                    Console.ResetColor();
                }
                else
                    Debug.LogWarning(msg);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
            AppendLine(message, LoggerType.WARN);
        }

        public void LogError(string message, params object[] pars)
        {
            string msg = pars != null ? string.Format(message, pars) : message;
            try
            {
                if (LerpedCore.safeECallEnabled)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(msg);
                    Console.ResetColor();
                }
                else
                    Debug.LogWarning(msg);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(msg);
                Console.ResetColor();
            }
            AppendLine(message, LoggerType.ERROR);
        }

        public void Save()
        {

        }

        public void SaveToFile()
        {

        }

        internal void AppendLine(string str, LoggerType t)
        {
            string msg = string.Format("[{0}] [{1}/{2}: {3}]", DateTime.Now.ToString("hh:mm:ss"), Thread.CurrentThread.Name, t.ToString(), str);
            if (!string.IsNullOrEmpty(path))
            {
                if (!File.Exists(path))
                    File.Create(path).Dispose();
                File.AppendAllText(path, msg);
            }
        }
    }
}
