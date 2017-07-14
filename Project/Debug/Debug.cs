using Lerp2API.Game;
using System;
using System.Diagnostics;
using UnityEngine;
using Logger = Lerp2API.SafeECalls.Logger;
using Object = UnityEngine.Object;

namespace Lerp2API._Debug
{
    /// <summary>
    /// Class Debug.
    /// </summary>
    /// <seealso cref="Lerp2API.Game.GameConsole" />
    public class Debug : GameConsole
    {
        internal static ConsoleSender _sender;

        private static ConsoleSender sender
        {
            get
            {
                if (_sender == null)
                    _sender = ConsoleSender.instance;
                return _sender;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        public static bool isEnabled
        {
            get
            { //Tengo que comprobar si esto da true, porque por alguna razon no es asi, o si poniendo esto en true no va...
                if (ExistsKey(enabledDebug))
                    return GetBool(enabledDebug);
                else
                    return true;
            }
        }

        /*private static string _logPath;
        private static string logPath
        {
            get
            {
                if (string.IsNullOrEmpty(_logPath))
                    _logPath = GetString(loggerPath);
                if(string.IsNullOrEmpty(_logPath))
                {
                    _logPath = Path.Combine(Application.dataPath, defaultLogFilePath);
                    SetString(loggerPath, _logPath);
                }
                return _logPath;
            }
        }*/

        /// <summary>
        /// Hooks the log.
        /// </summary>
        public static void HookLog()
        {
            //UnityEngine.Debug.Log("Hooking debug!");
            Application.logMessageReceived += LogToFile;
        }

        /// <summary>
        /// Unhooks the log.
        /// </summary>
        public static void UnhookLog()
        {
            Application.logMessageReceived -= LogToFile;
        }

        /*private static bool _isGameVersionEnabled;
        public static bool isGameVersionEnabled
        {
            get
            {
                return _isGameVersionEnabled;
            }
            set
            {
                _isGameVersionEnabled = value;
            }
        }*/

        //Is this a builded version or not? Idk, now what to say.. But if the debug is disabled this also does.
        //Realmente, esto comprueba si el debug esta habilitado se deberia llamar de otra forma, simplemente, esto sirve para enviar los mensajes a al logger, asi que no se porque al RAY le tengo puesto esto.
        /// <summary>
        /// Gets or sets a value indicating whether this instance is game version enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is game version enabled; otherwise, <c>false</c>.</value>
        public static bool isGameVersionEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [developer console visible].
        /// </summary>
        /// <value><c>true</c> if [developer console visible]; otherwise, <c>false</c>.</value>
        public static bool developerConsoleVisible
        {
            get { return UnityEngine.Debug.developerConsoleVisible; }
            set { UnityEngine.Debug.developerConsoleVisible = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is debug build.
        /// </summary>
        /// <value><c>true</c> if this instance is debug build; otherwise, <c>false</c>.</value>
        public static bool isDebugBuild
        {
            get { return UnityEngine.Debug.isDebugBuild; }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public static new ILogger logger
        {
            get { return UnityEngine.Debug.unityLogger; }
        }

        /// <summary>
        /// Asserts the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        public static void Assert(bool condition)
        {
            Assert(condition, "", null, false);
        }

        /// <summary>
        /// Asserts the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="message">The message.</param>
        public static void Assert(bool condition, string message)
        {
            Assert(condition, message, null, true);
        }

        /// <summary>
        /// Asserts the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="message">The message.</param>
        public static void Assert(bool condition, object message)
        {
            Assert(condition, message, null, false);
        }

        /// <summary>
        /// Asserts the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        public static void Assert(bool condition, string message, Object context)
        {
            Assert(condition, message, context, true);
        }

        /// <summary>
        /// Asserts the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <param name="str">if set to <c>true</c> [string].</param>
        public static void Assert(bool condition, object message, Object context, bool str = false)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.Assert(condition, str ? message : (string)message, context);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(message.ToUString(), DebugColor.assert);
            }
        }

        /// <summary>
        /// Asserts the format.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void AssertFormat(bool condition, string format, params object[] args)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.AssertFormat(condition, format, args);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.assert);
            }
        }

        /// <summary>
        /// Breaks this instance.
        /// </summary>
        public static void Break()
        {
            if (isEnabled)
                UnityEngine.Debug.Break();
        }

        /// <summary>
        /// Clears the developer console.
        /// </summary>
        public static void ClearDeveloperConsole()
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.ClearDeveloperConsole();
                if (IsPlaying() && isGameVersionEnabled)
                    Clear();
            }
        }

        /// <summary>
        /// Draws the line.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="color">The color.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="width">The width.</param>
        /// <param name="depthTest">if set to <c>true</c> [depth test].</param>
        public static void DrawLine(Vector3 start, Vector3 end, UnityEngine.Color color = default(UnityEngine.Color), float duration = 0.0f, float width = 1.0f, bool depthTest = true)
        {
            if (color.Equals(default(UnityEngine.Color)))
                color = DebugColor.normal;
            if (isEnabled)
            {
                if (IsEditor() && IsPlaying())
                    UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
                if (IsPlaying())
                    DebugLine.DrawLine(start, end, color, duration, width);
            }
        }

        /// <summary>
        /// Draws the ray.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="color">The color.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="width">The width.</param>
        /// <param name="depthTest">if set to <c>true</c> [depth test].</param>
        public static void DrawRay(Vector3 start, Vector3 dir, UnityEngine.Color color = default(UnityEngine.Color), float duration = 0.0f, float width = 1.0f, bool depthTest = true)
        {
            if (color.Equals(default(UnityEngine.Color)))
                color = DebugColor.normal;
            if (isEnabled)
            {
                if (IsEditor() && IsPlaying())
                    UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
                if (IsPlaying())
                    DebugLine.DrawRay(start, dir, color, duration, width);
            }
        }

        /// <summary>
        /// Logs the assertion.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        public static void LogAssertion(object message, Object context = null)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    if (context != null)
                        UnityEngine.Debug.LogAssertion(message, context);
                    else
                        UnityEngine.Debug.LogAssertion(message);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(message.ToUString(), DebugColor.assertion);
            }
        }

        /// <summary>
        /// Logs the assertion format.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void LogAssertionFormat(string message)
        {
            LogAssertionFormat(message, null);
        }

        /// <summary>
        /// Logs the assertion format.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public static void LogAssertionFormat(string message, params object[] args)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.LogAssertionFormat(message, args);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(message, DebugColor.assertion);
            }
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Log(string message)
        {
            Log((object)message);
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Log(object message)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.Log(message);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(message.ToUString(), DebugColor.normal);
            }
        }

        /// <summary>
        /// Logs the format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogFormat(string format, params object[] args)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.LogFormat(format, args);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.normal);
            }
        }

        /// <summary>
        /// Logs the format.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogFormat(Object context, string format, params object[] args)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.LogFormat(context, format, args);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.normal);
            }
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void LogWarning(string message)
        {
            LogWarning((object)message);
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void LogWarning(object message)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.LogWarning(message);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(message.ToUString(), DebugColor.warning);
            }
        }

        /// <summary>
        /// Logs the warning format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogWarningFormat(string format, params object[] args)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.LogWarningFormat(format, args);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.warning);
            }
        }

        /// <summary>
        /// Logs the warning format.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogWarningFormat(Object context, string format, params object[] args)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.LogWarningFormat(context, format, args);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.warning);
            }
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void LogError(string message)
        {
            LogError((object)message);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void LogError(object message)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.LogError(message);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(message.ToUString(), DebugColor.error);
            }
        }

        /// <summary>
        /// Logs the error format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogErrorFormat(string format, params object[] args)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.LogErrorFormat(format, args);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.error);
            }
        }

        /// <summary>
        /// Logs the error format.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void LogErrorFormat(Object context, string format, params object[] args)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.LogErrorFormat(context, format, args);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(string.Format(format, args), DebugColor.error);
            }
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="context">The context.</param>
        public static void LogException(Exception exception, Object context = null)
        {
            if (isEnabled)
            {
                if (IsEditor())
                    UnityEngine.Debug.LogException(exception, context);
                if (IsPlaying() && isGameVersionEnabled)
                    AddFormattedMessage(exception.Message, DebugColor.exception);
            }
        }

        internal static void LogToFile(string logString, string stackTrace, LogType type)
        {
            if (sender != null)
                sender.SendMessage(type, logString, stackTrace);
            else
                UnityEngine.Debug.LogError("Trying to send a message to the Console when it was closed, please start it before you do anything like this!"); //Guardar los mensajes a enviar y cuando se abra enviarlos todos de golpe
        }

        /// <summary>
        /// Draws the cube.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="col">The col.</param>
        /// <param name="scale">The scale.</param>
        public static void DrawCube(Vector3 pos, UnityEngine.Color col, Vector3 scale)
        {
            if (isEnabled)
            {
                Vector3 halfScale = scale * 0.5f;

                Vector3[] points = new Vector3[]
                {
                    pos + new Vector3(halfScale.x,      halfScale.y,    halfScale.z),
                    pos + new Vector3(-halfScale.x,     halfScale.y,    halfScale.z),
                    pos + new Vector3(-halfScale.x,     -halfScale.y,   halfScale.z),
                    pos + new Vector3(halfScale.x,      -halfScale.y,   halfScale.z),
                    pos + new Vector3(halfScale.x,      halfScale.y,    -halfScale.z),
                    pos + new Vector3(-halfScale.x,     halfScale.y,    -halfScale.z),
                    pos + new Vector3(-halfScale.x,     -halfScale.y,   -halfScale.z),
                    pos + new Vector3(halfScale.x,      -halfScale.y,   -halfScale.z)
                };

                DrawLine(points[0], points[1], col);
                DrawLine(points[1], points[2], col);
                DrawLine(points[2], points[3], col);
                DrawLine(points[3], points[0], col);
            }
        }

        /// <summary>
        /// Draws the rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="col">The col.</param>
        public static void DrawRect(Rect rect, UnityEngine.Color col)
        {
            if (isEnabled)
            {
                Vector3 pos = new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 0.0f);
                Vector3 scale = new Vector3(rect.width, rect.height, 0.0f);

                DrawRect(pos, col, scale);
            }
        }

        /// <summary>
        /// Draws the rect.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="col">The col.</param>
        /// <param name="scale">The scale.</param>
        public static void DrawRect(Vector3 pos, UnityEngine.Color col, Vector3 scale)
        {
            if (isEnabled)
            {
                Vector3 halfScale = scale * 0.5f;

                Vector3[] points = new Vector3[]
                {
                    pos + new Vector3(halfScale.x,      halfScale.y,    halfScale.z),
                    pos + new Vector3(-halfScale.x,     halfScale.y,    halfScale.z),
                    pos + new Vector3(-halfScale.x,     -halfScale.y,   halfScale.z),
                    pos + new Vector3(halfScale.x,      -halfScale.y,   halfScale.z)
                };

                DrawLine(points[0], points[1], col);
                DrawLine(points[1], points[2], col);
                DrawLine(points[2], points[3], col);
                DrawLine(points[3], points[0], col);
            }
        }

        /// <summary>
        /// Draws the point.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="col">The col.</param>
        /// <param name="scale">The scale.</param>
        public static void DrawPoint(Vector3 pos, UnityEngine.Color col, float scale)
        {
            if (isEnabled)
            {
                Vector3[] points = new Vector3[]
                {
                    pos + (Vector3.up * scale),
                    pos - (Vector3.up * scale),
                    pos + (Vector3.right * scale),
                    pos - (Vector3.right * scale),
                    pos + (Vector3.forward * scale),
                    pos - (Vector3.forward * scale)
                };

                DrawLine(points[0], points[1], col);
                DrawLine(points[2], points[3], col);
                DrawLine(points[4], points[5], col);

                DrawLine(points[0], points[2], col);
                DrawLine(points[0], points[3], col);
                DrawLine(points[0], points[4], col);
                DrawLine(points[0], points[5], col);

                DrawLine(points[1], points[2], col);
                DrawLine(points[1], points[3], col);
                DrawLine(points[1], points[4], col);
                DrawLine(points[1], points[5], col);

                DrawLine(points[4], points[2], col);
                DrawLine(points[4], points[3], col);
                DrawLine(points[5], points[2], col);
                DrawLine(points[5], points[3], col);
            }
        }

        /// <summary>
        /// Writes the safe stacktrace.
        /// </summary>
        public static void WriteSafeStacktrace()
        {
            WriteSafeStacktrace(LerpedCore.logger);
        }

        /// <summary>
        /// Writes the safe stacktrace.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public static void WriteSafeStacktrace(Logger logger)
        {
            StackTrace st = new StackTrace(true);
            for (int i = 0; i < st.FrameCount; ++i)
            {
                StackFrame sf = st.GetFrame(i);
                string filename = sf.GetFileName();
                logger.Log("{0}{1}", sf.GetMethod(),
                    string.IsNullOrEmpty(filename) ? "" : string.Format(" (at {0}:{1},{2})",
                                                                        filename,
                                                                        sf.GetFileLineNumber(),
                                                                        sf.GetFileColumnNumber()));
            }
        }
    }
}