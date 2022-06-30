using AbleCheckbook.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook.Logic
{

    /// <summary>
    /// How should the exception be handled at the top level?
    /// </summary>
    public enum ExceptionHandling
    {
        SaveCleanupContinue =    0,   // Save the DB, cleanup, then continue
        SaveThenRestart =        1,   // Save the DB then restart
        NoSaveCleanupContinue =  2,   // Don't save, cleanup, then continue
        NoSaveThenRestart =      3,   // Don't save, just restart
        CompleteFailure =        4,   // Give up, all is lost!
    }

    [System.Serializable]
    public class AppException : Exception
    {

        /// <summary>
        /// DB to be sync'd on a major failure.
        /// </summary>
        private static IDbAccess _db = null;

        /// <summary>
        /// Can the app continue after cleaning up the exception?
        /// </summary>
        private ExceptionHandling _exceptionHandling = ExceptionHandling.SaveCleanupContinue;

        /// <summary>
        /// Default Ctor.
        /// </summary>
        public AppException() : base("Unknown App Exception")
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="message">Description of failure.</param>
        public AppException(string message) : base(message)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="message">Description of failure.</param>
        /// <param name="inner">nested exception</param>
        public AppException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="message">Description of failure.</param>
        /// <param name="exceptionHandling">How to deal with it at the top level.</param>
        public AppException(string message, ExceptionHandling exceptionHandling) : base(message)
        {
            _exceptionHandling = exceptionHandling;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="message">Description of failure.</param>
        /// <param name="inner">nested exception</param>
        /// <param name="exceptionHandling">How to deal with it at the top level.</param>
        public AppException(string message, Exception inner, ExceptionHandling exceptionHandling) : base(message, inner)
        {
            _exceptionHandling = exceptionHandling;
        }

        /// <summary>
        /// Ctor for serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected AppException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
        
        /// <summary>
        /// Can the App continue after cleaning up the exception?
        /// </summary>
        public ExceptionHandling ExceptionHandling { get => _exceptionHandling; }

        /// <summary>
        /// Set the current underlying DB in case it must be sync'd before exit.
        /// </summary>
        /// <param name="db">current low-level DB</param>
        public static void SetDb(IDbAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Handle a top-level exception, saving the DB if appropriate.
        /// </summary>
        /// <param name="ex">the excepttion</param>
        /// <param name="message">Will be populated with a descriptive message.</param>
        /// <returns>Exception handling.</returns>
        public static ExceptionHandling HandleTopLevelException(Exception ex, out string message)
        {
            ExceptionHandling exceptionHandling = ExceptionHandling.SaveThenRestart;
            message = ex.Message;
            if (ex.GetType() == typeof(AppException))
            {
                exceptionHandling = ((AppException)ex).ExceptionHandling;
            }
            try
            {
                Logger.Error("Top Level Exception", ex);
            }
            catch (Exception ex1)
            {
                MessageBox.Show(Strings.Get("Cannot write log ") + ex1.Message);
            }
            if (exceptionHandling == ExceptionHandling.CompleteFailure)
            {
                message = "Unrecoverable Failure. " + ex.Message;
            }
            if (exceptionHandling == ExceptionHandling.SaveCleanupContinue)
            {
                message = "Serious Error. (Work Saved) " + ex.Message;
                if (_db != null)
                {
                    _db.Sync(); // save
                }
            }
            if (exceptionHandling == ExceptionHandling.SaveThenRestart)
            {
                message = "Serious Error. (Work Saved) Restarting. " + ex.Message;
                if (_db != null)
                {
                    _db.Sync(); // save
                }
            }
            if (exceptionHandling == ExceptionHandling.NoSaveCleanupContinue)
            {
                message = "Serious Error. " + ex.Message;
            }
            if (exceptionHandling == ExceptionHandling.NoSaveThenRestart)
            {
                message = "Serious Error. Restarting. " + ex.Message;
            }
            return exceptionHandling;
        }

    }

}
