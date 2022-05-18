using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    public class Logger : IDisposable
    {

        /// <summary>
        /// Logging criticalities.
        /// </summary>
        public enum LogLevel
        {
            Detail = 0,
            Trace = 1,
            Diag = 2,
            Info = 3,
            Warn = 4,
            Error = 5,
        }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static Logger _instance = null;

        /// <summary>
        /// Don't swamp the user with error messages.
        /// </summary>
        private static int _loggerFailures = 0;

        /// <summary>
        /// Prevent a Ctor race condition.
        /// </summary>
        private static Mutex _mutex = new Mutex();

        /// <summary>
        /// Here's the file we write logs to.
        /// </summary>
        private StreamWriter _writer = null;

        /// <summary>
        /// What level of logging are we keeping track of?
        /// </summary>
        private LogLevel _level = LogLevel.Diag;

        /// <summary>
        /// What level of logging are we keeping track of?
        /// </summary>
        public LogLevel Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
                _writer.WriteLine("Level: " + _level + " (Log Level Changed)");
            }
        }

        /// <summary>
        /// Close the logger, for unit testing only.
        /// </summary>
        public static void Close()
        {
            _mutex.WaitOne();
            if (_instance != null)
            {
                _instance._writer.AutoFlush = false;
                _instance._writer.Flush();
                _instance._writer.Close();
                _instance = null;
            }
            _mutex.ReleaseMutex();
        }

        /// <summary>
        /// Ctor. (private, as this is a singleton)
        /// </summary>
        private Logger()
        {
            string filename = Path.Combine(Configuration.Instance.DirectoryLogs, "chkbk-diag.log");
            try
            {
                if (File.Exists(filename))
                {
                    FileInfo fileInfo = new FileInfo(filename);
                    if (fileInfo.Length > 2000000L) // 2MB
                    {
                        if (File.Exists(filename + ".bak"))
                        {
                            File.Delete(filename + ".bak");
                        }
                        File.Move(filename, filename + ".bak");
                    }
                }
                _writer = new StreamWriter(filename, true);
                _writer.WriteLine("\r\n###### " + DateTime.Now.ToLocalTime() + " ######\r\n");
                _writer.WriteLine("DB v: " + Version.DbVersion);
                _writer.WriteLine("App v: " + Version.AppVersion);
                _writer.WriteLine("User: " + Environment.UserName + "\r\n");
                _writer.WriteLine("Level: " + _level);
                _writer.AutoFlush = true;
            }
            catch (Exception ex)
            {
                if(_loggerFailures++ < 1)
                {
                    throw new AppException("Logger Ctor Failed", ex, ExceptionHandling.NoSaveCleanupContinue);
                }
            }
        }

        /// <summary>
        /// Create or create our singleton instance.
        /// </summary>
        public static Logger Instance
        {
            get
            {
                // This is NOT a redundant "if" but it is here so we don't lock 
                // the mutex for every Instance reference, crippling performance.
                if (_instance == null)
                {
                    _mutex.WaitOne();
                    if (_instance == null)
                    {
                        _instance = new Logger();
                        if (Configuration.Instance.GetUserLevel() == UserLevel.SuperUser)
                        {
                            _instance.Log(LogLevel.Info, "### Super User Enabled ###");
                        }
                    }
                    _mutex.ReleaseMutex();
                }
                return _instance;
            }
        }

        /// <summary>
        /// A primary entry point, although the caller will typically call the static methods.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogLevel level, string message)
        {
            if (level < _level)
            {
                return;
            }
            DateTime now = DateTime.Now;
            int nonLogFrameIndex = 1;
            string className = "";
            string methodName = "";
            string callStack = "";
            StackTrace stackTrace = new StackTrace();
            for (nonLogFrameIndex = 1; nonLogFrameIndex < 10 && nonLogFrameIndex < stackTrace.GetFrames().Length; nonLogFrameIndex++)
            {
                className = stackTrace.GetFrame(nonLogFrameIndex).GetMethod().ReflectedType.Name;
                methodName = stackTrace.GetFrame(nonLogFrameIndex).GetMethod().Name;
                if (!className.Equals("Logger") && !methodName.Equals("LoggerHook"))
                {
                    break;
                }
            }
            try
            {  
                if(_level == LogLevel.Detail)
                {
                    for(int i = 1; i < 6; ++i)
                    {
                        if(nonLogFrameIndex + i >= stackTrace.FrameCount - 1)
                        {
                            break;
                        }
                        callStack = "\r\n   " +
                            stackTrace.GetFrame(nonLogFrameIndex + i).GetMethod().ReflectedType.Name + "." +
                            stackTrace.GetFrame(nonLogFrameIndex + i).GetMethod().Name + "() -> " + callStack;
                    }
                    if (callStack.Length > 1)
                    {
                        callStack += "\r\n   " + className + "." + methodName + "()";
                    }
                }
                string timeStr = DateTime.Now.ToString("HH:mm:ss.ffff");
                _writer.WriteLine("{0} [{1}] {2}.{3}: {4}{5}",
                    timeStr, level, className, methodName, message, callStack);
            }
            catch (Exception ex)
            {
                if (_loggerFailures++ < 2)
                {
                    throw new AppException("Logger.Log Failed", ex, ExceptionHandling.NoSaveCleanupContinue);
                }
            }
        }

        /// <summary>
        /// A primary entry point, although the caller will typically call the static methods.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public void Log(LogLevel level, string message, Exception ex)
        {
            string indented = "\r\nException: " + ex.Message + "\r\n" + ex.StackTrace;
            indented = indented.Replace("\r\n", "\r\n    ");
            Instance.Log(level, message + indented);
        }

        /// <summary>
        /// Log trace.
        /// </summary>
        /// <param name="message">message to be logged</param>
        public static void Trace(string message)
        {
            Instance.Log(LogLevel.Trace, message);
        }

        /// <summary>
        /// Log trace.
        /// </summary>
        /// <param name="message">message to be logged</param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public static void Trace(string message, Exception ex)
        {
            Instance.Log(LogLevel.Trace, message, ex);
        }

        /// <summary>
        /// Log a diagnostic.
        /// </summary>
        /// <param name="message">message to be logged</param>
        public static void Diag(string message)
        {
            Instance.Log(LogLevel.Diag, message);
        }

        /// <summary>
        /// Log a diagnostic.
        /// </summary>
        /// <param name="message">message to be logged</param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public static void Diag(string message, Exception ex)
        {
            Instance.Log(LogLevel.Diag, message, ex);
        }

        /// <summary>
        /// Log info.
        /// </summary>
        /// <param name="message">message to be logged</param>
        public static void Info(string message)
        {
            Instance.Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Log info.
        /// </summary>
        /// <param name="message">message to be logged</param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public static void Info(string message, Exception ex)
        {
            Instance.Log(LogLevel.Info, message, ex);
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="message">message to be logged</param>
        public static void Warn(string message)
        {
            Instance.Log(LogLevel.Warn, message);
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="message">message to be logged</param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public static void Warn(string message, Exception ex)
        {
            Instance.Log(LogLevel.Warn, message, ex);
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">message to be logged</param>
        public static void Error(string message)
        {
            Instance.Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="message">message to be logged</param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public static void Error(string message, Exception ex)
        {
            Instance.Log(LogLevel.Error, message, ex);
        }

        /// <summary>
        /// IDisposable.
        /// </summary>
        public void Dispose()
        {
            Close();
        }
    }

}
