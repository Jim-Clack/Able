using AbleCheckbook.Db;
using AbleCheckbook.Logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Gui
{

    /// <summary>
    /// Encapsulates and decorates a potential backup file listing
    /// </summary>
    public class RowOfBackup : IComparable
    {

        private string _fileName = "";

        private double _score = 0.0;

        private string _looksOkay = "";

        private DateTime _modifDate = DateTime.Now.AddYears(-5);

        private DateTime _saveDate = DateTime.Now.AddYears(-5);

        private string _lastModPayee = "";

        private string _lastModAmount = "";

        private int _entriesTotal = 0;

        private int _entriesThisYear = 0;

        private int _entriesLast30Days = 0;

        private int _entriesLast90Days = 0;

        private int _scheduledEvents = 0;

        private long _fileSizeBytes = 0;

        private string _path = null;

        private FileInfo _fileInfo = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="fileInfo">Populated FileInfo</param>
        public RowOfBackup(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
            _path = fileInfo.FullName;
            _fileName = fileInfo.Name;
            _saveDate = fileInfo.LastWriteTime;
            _fileSizeBytes = fileInfo.Length;
            _lastModAmount = "";
            _lastModPayee = "";
            _entriesTotal = 0;
            _entriesThisYear = 0;
            _entriesLast30Days = 0;
            _entriesLast90Days = 0;
            _scheduledEvents = 0;
            IDbAccess db = null;
            try
            {
                db = new JsonDbAccess(_path, null);
            }
            catch (Exception)
            {
                _looksOkay = "";
                _score = 0.0;
                return;
            }
            PopulateThis(db);
            db.CloseWithoutSync();
            CalculateScore();
        }

        /// <summary>
        /// Read the db and populate this object accordingly.
        /// </summary>
        /// <param name="db">Source of data</param>
        private void PopulateThis(IDbAccess db)
        {
            DateTime ago30Days = DateTime.Now.AddDays(-30);
            DateTime ago90Days = DateTime.Now.AddDays(-90);
            DateTime agoJan1 = new DateTime(DateTime.Now.Year, 1, 1);
            ScheduledEventIterator events = db.ScheduledEventIterator;
            while (events.HasNextEntry())
            {
                _scheduledEvents++;
            }
            CheckbookEntryIterator entries = db.CheckbookEntryIterator;
            while (entries.HasNextEntry())
            {
                _entriesTotal++;
                CheckbookEntry entry = entries.GetNextEntry();
                if (entry.DateModified.CompareTo(_modifDate) > 0)
                {
                    _modifDate = entry.DateModified;
                    _lastModPayee = entry.Payee;
                    _lastModAmount = UtilityMethods.FormatCurrency(entry.Amount, 3);
                }
                if (entry.DateOfTransaction.CompareTo(agoJan1) > 0)
                {
                    _entriesThisYear++;
                }
                if (entry.DateOfTransaction.CompareTo(ago30Days) > 0)
                {
                    _entriesLast30Days++;
                }
                if (entry.DateOfTransaction.CompareTo(ago90Days) > 0)
                {
                    _entriesLast90Days++;
                }
            }
            _looksOkay = "OK";
        }

        /// <summary>
        /// Set the _score.
        /// </summary>
        private void CalculateScore()
        {
            _score = 0.11 * Math.Max(0, 500.0 - Math.Sqrt(Math.Abs(_modifDate.Subtract(DateTime.Now).TotalHours)));
            _score += 0.10 * Math.Max(0, 500.0 - Math.Sqrt(Math.Abs(_saveDate.Subtract(DateTime.Now).TotalHours)));
            _score += Math.Sqrt(_entriesLast30Days + _entriesLast90Days);
            _score += Math.Sqrt(_entriesThisYear * 4 + _scheduledEvents);
            _score = Math.Round(_score, 4);
        }

        /// <summary>
        /// Compare for sorting purposes.
        /// </summary>
        /// <param name="row">To be compared</param>
        /// <returns>-1, 0, or 1, per CompareTo standard practices, but in descending order</returns>
        public int CompareTo(Object row)
        {
            return ((RowOfBackup)row)._score.CompareTo(_score);
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
        }

        public double Score
        {
            get
            {
                return _score;
            }
        }

        public string LooksOkay
        {
            get
            {
                return _looksOkay;
            }
        }

        public DateTime ModifDate
        {
            get
            {
                return _modifDate;
            }
        }

        public DateTime SaveDate
        {
            get
            {
                return _saveDate;
            }
        }

        public string LastModPayee
        {
            get
            {
                return _lastModPayee;
            }
        }

        public string LastModAmount
        {
            get
            {
                return _lastModAmount;
            }
        }

        public int EntriesTotal
        {
            get
            {
                return _entriesTotal;
            }
        }

        public int EntriesThisYear
        {
            get
            {
                return _entriesThisYear;
            }
        }

        public int EntriesLast30Days
        {
            get
            {
                return _entriesLast30Days;
            }
        }

        public int EntriesLast90Days
        {
            get
            {
                return _entriesLast90Days;
            }
        }

        public int ScheduledEvents
        {
            get
            {
                return _scheduledEvents;
            }
        }

        public long FileSizeBytes
        {
            get
            {
                return _fileSizeBytes;
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }
        }

    }

}
