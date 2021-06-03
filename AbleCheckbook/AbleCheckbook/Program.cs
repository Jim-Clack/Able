using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbleCheckbook
{
    static class Program
    {

        private const int MaxRestarts = 3;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainScreen mainScreen = new MainScreen();
            int restartAttempts = 0;
            bool tryAgain = true;
            while (tryAgain)
            {
                try
                {
                    Application.Run(mainScreen);
                    tryAgain = false; // normal exit
                }
                catch (Exception ex)
                {
                    tryAgain = false;
                    string message = "";
                    ExceptionHandling exceptionHandling = AppException.HandleTopLevelException(ex, out message);
                    MessageBox.Show(Strings.GetIff(message), Strings.Get("Error"), MessageBoxButtons.OK);
                    if (exceptionHandling == ExceptionHandling.CompleteFailure)
                    {
                        tryAgain = false;
                    }
                    if (exceptionHandling == ExceptionHandling.SaveCleanupContinue)
                    {
                        if (restartAttempts++ < MaxRestarts)
                        {
                            tryAgain = true;
                        }
                    }
                    if (exceptionHandling == ExceptionHandling.SaveThenRestart || exceptionHandling == ExceptionHandling.NoSaveThenRestart)
                    {
                        mainScreen = new MainScreen(); // restart
                        if (restartAttempts++ < MaxRestarts)
                        {
                            tryAgain = true;
                        }
                    }
                }
            }
        }
    }
}
