using Microsoft.AspNetCore.Http;
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
        private static string LOG_FILE_PATH = "./server.log";

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static Logger instance = null;

        /// <summary>
        /// Don't swamp the user with error messages.
        /// </summary>
        private static int loggerFailures = 0;

        /// <summary>
        /// Inner mutex to avoid races.
        /// </summary>
        private static Mutex innerMutex = new Mutex();

        /// <summary>
        /// Outer mutex to avoid races.
        /// </summary>
        private static Mutex outerMutex = new Mutex();

        /// <summary>
        /// Outer mutex to avoid races.
        /// </summary>
        private static Mutex messageMutex = new Mutex();

        /// <summary>
        /// Force an immediate backup conditionally.
        /// </summary>
        private DateTime lastBackupCheck = DateTime.Now.AddHours(-1000);

        /// <summary>
        /// Track number of writes since last backup
        /// </summary>
        private long numWrites = 0L;

        /// <summary>
        /// Here's the file we write logs to.
        /// </summary>
        private StreamWriter streamWriter = null;

        /// <summary>
        /// What level of logging are we keeping track of?
        /// </summary>
        private LogLevel level = LogLevel.Diag;

        // Getters/Setters
        public static Mutex InnerMutex { get => innerMutex; }
        public static Mutex OuterMutex { get => outerMutex; }

        /// <summary>
        /// What level of logging are we keeping track of?
        /// </summary>
        public LogLevel Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
                streamWriter.WriteLine("Level: " + level + " (Log Level Changed)");
            }
        }

        /// <summary>
        /// Close the logger, for unit testing only.
        /// </summary>
        public static void Close()
        {
            innerMutex.WaitOne();
            if (instance != null)
            {
                instance.streamWriter.AutoFlush = false;
                instance.streamWriter.Flush();
                instance.streamWriter.Close();
                instance = null;
            }
            innerMutex.ReleaseMutex();
        }

        /// <summary>
        /// Ctor. (private, as this is a singleton)
        /// </summary>
        private Logger()
        {
            try
            {
                innerMutex.WaitOne();
                BackupAndOpen();
            }
            catch (Exception ex)
            {
                if (loggerFailures++ < 3)
                {
                    throw new Exception("Logger Ctor Failed", ex);
                }
            }
            finally
            {
                innerMutex.ReleaseMutex();
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
                if (instance == null)
                {
                    outerMutex.WaitOne();
                    if (instance == null)
                    {
                        instance = new Logger();
                    }
                    outerMutex.ReleaseMutex();
                }
                return instance;
            }
        }

        /// <summary>
        /// A primary entry point, although the caller will typically call the static methods.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="ipAddress">IP address as a string, null or "" if unknown</param>
        /// <param name="message">To be logged</param>
        public void Log(LogLevel level, string ipAddress, string message)
        {
            if (level < this.level)
            {
                return;
            }
            ipAddress = string.IsNullOrEmpty(ipAddress) ? "(internal)" : ipAddress;
            DateTime now = DateTime.Now;
            int nonLogFrameIndex = 1;
            string className = "";
            string methodName = "";
            string callStack = "";
            if((int)level >= (int)LogLevel.Warn)
            {
                AddWarningMessage(now.ToShortDateString() + "|" + now.ToShortTimeString() + " [" + ipAddress + "] " + level + " " + message);
            }
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
                if (this.level == LogLevel.Detail)
                {
                    for (int i = 1; i < 6; ++i)
                    {
                        if (nonLogFrameIndex + i >= stackTrace.FrameCount - 1)
                        {
                            break;
                        }
                        if (stackTrace.GetFrame(nonLogFrameIndex + i).GetMethod().ReflectedType != typeof(void))
                        {
                            callStack = "\r\n   " +
                                stackTrace.GetFrame(nonLogFrameIndex + i).GetMethod().ReflectedType.Name + "." +
                                stackTrace.GetFrame(nonLogFrameIndex + i).GetMethod().Name + "() -> " + callStack;
                        }
                    }
                    if (callStack.Length > 1)
                    {
                        callStack += "\r\n   " + className + "." + methodName + "()";
                    }
                }
                string timeStr = DateTime.Now.ToString("HH:mm:ss.ffff");
                streamWriter.WriteLine("{0} [{1}] {2}.{3}: {4} {5}{6}",
                    timeStr, level, className, methodName, message, ipAddress, callStack);
                ++numWrites;
            }
            catch (Exception ex)
            {
                if (loggerFailures++ < 2)
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
            BackupIfOkay();
            streamWriter = new StreamWriter(LOG_FILE_PATH, true);
            streamWriter.WriteLine("\r\n###### " + DateTime.Now.ToLocalTime() + " ######\r\n");
            streamWriter.WriteLine("App v: " + Environment.Version.ToString());
            streamWriter.WriteLine("User: " + Environment.UserName + "\r\n");
            streamWriter.WriteLine("Level: " + level);
            streamWriter.AutoFlush = true;
            numWrites = 0L;
            lastBackupCheck = DateTime.Now;
        }

        private void BackupIfOkay()
        {
            bool logfileExists = File.Exists(LOG_FILE_PATH);
            if (!logfileExists) // only check this if the file exists
            {
                return;
            }
            bool logfileIsOpen = streamWriter != null;
            if (logfileIsOpen)
            {
                streamWriter.Close();
            }
            FileInfo fileInfo = new FileInfo(LOG_FILE_PATH);
            if (fileInfo.Length < 1000000L) // 1MB
            {
                return;
            }
            // Perform the backup
            if (File.Exists(LOG_FILE_PATH + ".bak"))
            {
                File.Delete(LOG_FILE_PATH + ".bak");
            }
            if (File.Exists(LOG_FILE_PATH))
            {
                File.Move(LOG_FILE_PATH, LOG_FILE_PATH + ".bak");
            }
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

        /// <summary>
        /// Append a line to the warning messages.
        /// </summary>
        /// <param name="message">To be appended</param>
        private void AddWarningMessage(string message)
        {
            string filePath = Configuration.Instance.MessagesPath + "warnings.msg";
            try
            {
                messageMutex.WaitOne();
                System.IO.StreamWriter writer = new StreamWriter(filePath, true);
                writer.WriteLine(message);
                writer.Close();
            }
            finally
            {
                messageMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Return a multi-line string containing log warnings and errors since last call to this method.
        /// </summary>
        /// <returns></returns>
        public string GetWarningMessages()
        {
            string message = "";
            string filePath = Configuration.Instance.MessagesPath + "warnings.msg";
            try
            {
                messageMutex.WaitOne();
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(filePath);
                    message = reader.ReadToEnd();
                    reader.Close();
                    System.IO.File.Delete(filePath);
                }
            }
            finally
            {
                messageMutex.ReleaseMutex();
            }
            return message;
        }
    }

}

