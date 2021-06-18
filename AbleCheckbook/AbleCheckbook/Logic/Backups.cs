using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    public static class Backups
    {
        /// <summary>
        /// Update backup files.
        /// </summary>
        /// <param name="filename">Base path and filename to be backed up.</param>
        /// <param name="count">Number of rolling backups to maintain.</param>
        /// <param name="extension">Backup file base extension beginning with a dot.</param>
        /// <param name="inPlace">True to backup in place, else to a backup path.</param>
        /// <param name="buPath2">True to backup to backup path 2 instead of 1. (but only if !inPlace)</param>
        /// <returns>null if okay, else error message.</returns>
        public static string BackupNow(string filename, int count, string extension, bool inPlace, bool buPath2)
        {
            string okay = null;
            string filenameBase = GetBackupFilenameNoSuffix(filename, inPlace, buPath2);
            try
            {
                string filenameBak1 = filenameBase + extension + (count - 1);
                for (int backupNbr = count; backupNbr > 1; --backupNbr)
                {
                    if (File.Exists(filename))
                    {
                        filenameBak1 = filenameBase + extension + (backupNbr - 1);
                        string filenameBak2 = filenameBase + extension + backupNbr;
                        Directory.CreateDirectory(Path.GetDirectoryName(filenameBak1));
                        if (File.Exists(filenameBak2))
                        {
                            File.Delete(filenameBak2);
                        }
                        if (File.Exists(filenameBak1))
                        {
                            File.Move(filenameBak1, filenameBak2);
                        }
                        File.Copy(filename, filenameBak1);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Problem Backing Up " + filename, ex);
                okay = "Problem Backing Up " + filename + " - " + ex.Message;
            }
            return okay;
        }

        /// <summary>
        /// Check if it is time to perform a periodic backup and, if so, do it.
        /// </summary>
        /// <param name="filename">Base path and filename of user data file.</param>
        /// <param name="days">How often to perform backups - number of days.</param>
        /// <param name="count">Number of rolling backups to maintain.</param>
        /// <param name="extension">Backup file base extension beginning with a dot. i.e. ".bu"</param>
        /// <param name="inPlace">True to backup in place, else to a backup path.</param>
        /// <param name="buPath2">True to backup to backup path 2 instead of 1.</param>
        /// <returns>null if okay, else error message.</returns>
        public static string PeriodicBackup(string filename, int days, int count, string extension, bool inPlace, bool buPath2)
        {
            string filenameBak1 = GetBackupFilenameNoSuffix(filename, inPlace, buPath2) + extension + "1";
            DateTime lastDate = DateTime.Now.AddDays(-1000);
            if (File.Exists(filenameBak1))
            {
                lastDate = File.GetLastWriteTime(filenameBak1);
            }
            if (DateTime.Now.Date.Subtract(lastDate.Date).TotalDays < days)
            {
                return null; // not yet time to backup
            }
            return BackupNow(filename, count, extension, inPlace, buPath2);
        }

        /// <summary>
        /// Assemble a backup filepath with no extension.
        /// </summary>
        /// <param name="filename">Base path and filename to be backed up.</param>
        /// <param name="inPlace">True to backup in place, else to a backup path. </param>
        /// <param name="buPath2">True to backup to backup path 2 instead of 1. (but only if !inPlace)</param>
        /// <returns>filepath and name with no extension</returns>
        private static string GetBackupFilenameNoSuffix(string filename, bool inPlace, bool buPath2)
        {
            string buPath = (buPath2 && Configuration.Instance.DirectoryBackup2.Length > 0) ?
                Configuration.Instance.DirectoryBackup2 : 
                Configuration.Instance.DirectoryBackup1;
            buPath = inPlace ? Path.GetDirectoryName(filename) : buPath;
            return Path.Combine(buPath, Path.GetFileNameWithoutExtension(filename));
        }
    }
}
