using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
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
        /// Location of output file.
        /// </summary>
        private static string _logFilePath = "./server.log";

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static Logger _instance = null;

        /// <summary>
        /// Don't swamp the user with error messages.
        /// </summary>
        private static int _loggerFailures = 0;

        /// <summary>
        /// Inner mutex to avoid races.
        /// </summary>
        private static Mutex _innerMutex = new Mutex();

        /// <summary>
        /// Outer mutex to avoid races.
        /// </summary>
        private static Mutex _outerMutex = new Mutex();

        /// <summary>
        /// Force an immediate backup conditionally.
        /// </summary>
        private DateTime _lastBackupCheck = DateTime.Now.AddHours(-1000);

        /// <summary>
        /// Track number of writes since last backup
        /// </summary>
        private long _numWrites = 0L;

        /// <summary>
        /// Here's the file we write logs to.
        /// </summary>
        private StreamWriter _writer = null;

        /// <summary>
        /// What level of logging are we keeping track of?
        /// </summary>
        private LogLevel _level = LogLevel.Diag;

        // Getters/Setters
        public static Mutex InnerMutex { get => _innerMutex; }
        public static Mutex OuterMutex { get => _outerMutex; }

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
            _innerMutex.WaitOne();
            if (_instance != null)
            {
                _instance._writer.AutoFlush = false;
                _instance._writer.Flush();
                _instance._writer.Close();
                _instance = null;
            }
            _innerMutex.ReleaseMutex();
        }

        /// <summary>
        /// Ctor. (private, as this is a singleton)
        /// </summary>
        private Logger()
        {
            try
            {
                _innerMutex.WaitOne();
                BackupAndOpen();
            }
            catch (Exception ex)
            {
                if (_loggerFailures++ < 3)
                {
                    throw new Exception("Logger Ctor Failed", ex);
                }
            }
            finally
            {
                _innerMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Create or create our singleton instance.
        /// </summary>
        public static Logger Instance
        {
            get
            {
                // This is NOT a redundant "if" but it is here so we don't lock for 
                // initial simultaneous Instance reference, crippling performance.
                if (_instance == null)
                {
                    _outerMutex.WaitOne();
                    if (_instance == null)
                    {
                        _instance = new Logger();
                    }
                    _outerMutex.ReleaseMutex();
                }
                return _instance;
            }
        }

        /// <summary>
        /// A primary entry point, although the caller will typically call the static methods.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message"></param>
        public void Log(LogLevel level, string ipAddress, string message)
        {
            if (level < _level)
            {
                return;
            }
            ipAddress = string.IsNullOrEmpty(ipAddress) ? "(internal)" : ipAddress;
            DateTime now = DateTime.Now;
            int nonLogFrameIndex = 1;
            string className = "";
            string methodName = "";
            string callStack = "";
            StackTrace stackTrace = new StackTrace();
            if(level == LogLevel.Warn) // only check for "time to backup" on a warn level message
            {
                BackupAndOpen();
            }
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
                if (_level == LogLevel.Detail)
                {
                    for (int i = 1; i < 6; ++i)
                    {
                        if (nonLogFrameIndex + i >= stackTrace.FrameCount - 1)
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
                _writer.WriteLine("{0} [{1}] {2}.{3}: {4} {5}{6}",
                    timeStr, level, className, methodName, message, ipAddress, callStack);
                ++_numWrites;
            }
            catch (Exception ex)
            {
                if (_loggerFailures++ < 2)
                {
                    throw new Exception("Logger.Log Failed", ex);
                }
            }
        }

        /// <summary>
        /// Backup the logfile, then opening it again, if necessary. (lock mutex before calling!)
        /// </summary>
        private void BackupAndOpen()
        {
            bool logfileIsOpen = _writer != null;
            if (!logfileIsOpen)
            {
                bool tooManyWrites = _numWrites > 10000;
                bool tooLongOpen = Math.Abs(DateTime.Now.Subtract(_lastBackupCheck).Hours) > 23;
                if (!(tooManyWrites || tooLongOpen)) // only bother to check if one of these is true
                {
                    return;
                }
                bool logfileExists = File.Exists(_logFilePath);
                if (logfileExists) // only check this if the file exists
                {
                    FileInfo fileInfo = new FileInfo(_logFilePath);
                    if (fileInfo.Length < 1000000L) // 1MB
                    {
                        return;
                    }
                }
            }
            if (_writer != null)
            {
                _writer.Close();
                _writer.Dispose();
                _writer = null;
            }
            // Perform the backup
            if (File.Exists(_logFilePath + ".bak"))
            {
                File.Delete(_logFilePath + ".bak");
            }
            if (File.Exists(_logFilePath))
            {
                File.Move(_logFilePath, _logFilePath + ".bak");
            }
            _writer = new StreamWriter(_logFilePath, true);
            _writer.WriteLine("\r\n###### " + DateTime.Now.ToLocalTime() + " ######\r\n");
            _writer.WriteLine("App v: " + Environment.Version.ToString());
            _writer.WriteLine("User: " + Environment.UserName + "\r\n");
            _writer.WriteLine("Level: " + _level);
            _writer.AutoFlush = true;
            _numWrites = 0L;
            _lastBackupCheck = DateTime.Now;
        }

        /// <summary>
        /// A primary entry point, although the caller will typically call the static methods.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message"></param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public void Log(LogLevel level, string ipAddress, string message, Exception ex)
        {
            string indented = "\r\nException: " + ex.Message + "\r\n" + ex.StackTrace;
            indented = indented.Replace("\r\n", "\r\n    ");
            Instance.Log(level, ipAddress, message + indented);
        }

        /// <summary>
        /// Log trace.
        /// </summary>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">message to be logged</param>
        public static void Trace(string ipAddress, string message)
        {
            Instance.Log(LogLevel.Trace, ipAddress, message);
        }

        /// <summary>
        /// Log trace.
        /// </summary>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">message to be logged</param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public static void Trace(string ipAddress, string message, Exception ex)
        {
            Instance.Log(LogLevel.Trace, ipAddress, message, ex);
        }

        /// <summary>
        /// Log a diagnostic.
        /// </summary>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">message to be logged</param>
        public static void Diag(string ipAddress, string message)
        {
            Instance.Log(LogLevel.Diag, ipAddress, message);
        }

        /// <summary>
        /// Log a diagnostic.
        /// </summary>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">message to be logged</param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public static void Diag(string ipAddress, string message, Exception ex)
        {
            Instance.Log(LogLevel.Diag, ipAddress, message, ex);
        }

        /// <summary>
        /// Log info.
        /// </summary>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">message to be logged</param>
        public static void Info(string ipAddress, string message)
        {
            Instance.Log(LogLevel.Info, ipAddress, message);
        }

        /// <summary>
        /// Log info.
        /// </summary>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">message to be logged</param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public static void Info(string ipAddress, string message, Exception ex)
        {
            Instance.Log(LogLevel.Info, ipAddress, message, ex);
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">message to be logged</param>
        public static void Warn(string ipAddress, string message)
        {
            Instance.Log(LogLevel.Warn, ipAddress, message);
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">message to be logged</param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public static void Warn(string ipAddress, string message, Exception ex)
        {
            Instance.Log(LogLevel.Warn, ipAddress, message, ex);
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">message to be logged</param>
        public static void Error(string ipAddress, string message)
        {
            Instance.Log(LogLevel.Error, ipAddress, message);
        }

        /// <summary>
        /// Log an error.
        /// </summary>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">message to be logged</param>
        /// <param name="ex">exception for logging the stacktrace</param>
        public static void Error(string ipAddress, string message, Exception ex)
        {
            Instance.Log(LogLevel.Error, ipAddress, message, ex);
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

