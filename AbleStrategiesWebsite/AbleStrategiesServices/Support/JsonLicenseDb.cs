using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{

    /// <summary>
    /// [singleton] License Database implemented as flat files.
    /// </summary>
    /// <remarks>
    /// This is NOT currently implemented as a DB but as in-memory collections. That means that
    /// returned records are "live" and that any change to them is immediately changed in the 
    /// in-memory DB. Be careful. To avoid problems, don't replace (update) a record with a new
    /// one unless you first record.Update(source), source is the original as read from the DB.
    /// </remarks>
    public class JsonLicenseDb : ILicenseDb
    {

        /// <summary>
        /// Until we do exhaustive testing, leave this set to true.
        /// </summary>
        private static bool PerformUnecessaryUpdates = true;

        /// <summary>
        /// Here's the in-memory db.
        /// </summary>
        private DbContent _dbContent = null;

        /// <summary>
        /// Our one-and-only instance.
        /// </summary>
        private static JsonLicenseDb _instance = null;

        /// <summary>
        /// Path and name of JSON file with all data.
        /// </summary>
        private static string _connection = "./license.json";

        /// <summary>
        /// How often to autosave if changes were made.
        /// </summary>
        public const long AUTO_SAVE_SECONDS = 19;

        /// <summary>
        /// WHen did we last autosave?
        /// </summary>
        private static DateTime _lastAutoSave = DateTime.Now;

        /// <summary>
        /// This is the full path to the JSON DB file. 
        /// </summary>
        private string _fullPath = null;

        /// <summary>
        /// Description of last error. "" if none.
        /// </summary>
        private string _errorMessage = "";

        /// <summary>
        /// Has the data changed since the last save/sync was done?
        /// </summary>
        private bool _isDirty = false;

        /// <summary>
        /// Inner mutex to avoid races.
        /// </summary>
        private static Mutex _innerMutex = new Mutex();

        /// <summary>
        /// Outer mutex to avoid races.
        /// </summary>
        private static Mutex _outerMutex = new Mutex();

        // Getters/Setters
        public static Mutex InnerMutex { get => _innerMutex; }
        public static Mutex OuterMutex { get => _outerMutex; }

        /// <summary>
        /// Fetch the singleton.
        /// </summary>
        public static JsonLicenseDb Instance
        {
            get
            {
                // This is NOT a redundant "if" but it is here so we don't lock for 
                // initial simultaneous Instance reference, crippling performance.
                if (_instance == null)
                {
                    _outerMutex.WaitOne();
                    try
                    {
                        if (_instance == null)
                        {
                            _instance = new JsonLicenseDb();
                        }
                    }
                    finally
                    {
                        _outerMutex.ReleaseMutex();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// For unit testing only!
        /// </summary>
        public static void PurgeExisting()
        {
            if(_instance != null)
            {
                _instance.CloseWithoutSync();
            }
            _outerMutex.WaitOne();
            try
            {
                File.Delete(_connection);
            }
            finally
            {
                _outerMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        private JsonLicenseDb()
        {
            _fullPath = _connection;
            _errorMessage = "";
            _isDirty = false;
            if (File.Exists(_connection))
            {
                JsonLicenseDb.InnerMutex.WaitOne();
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.AllowTrailingCommas = true;
                try
                {
                    using (FileStream stream = File.OpenRead(_fullPath))
                    {
                        _dbContent = JsonSerializer.DeserializeAsync<DbContent>(stream, options).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                    throw new Exception("Cannot deserialize " + _fullPath, ex);
                }
                finally
                {
                    JsonLicenseDb.InnerMutex.ReleaseMutex();
                }
            }
            else
            {
                _dbContent = new DbContent();
            }
            _dbContent.DbName = Path.GetFileNameWithoutExtension(_fullPath);
        }

        /// <summary>
        /// Close the DB without modifiying it. (if it was open too long it may have already been modified)
        /// </summary>
        public void CloseWithoutSync()
        {
            Logger.Info(null, "Close Without Sync " + _fullPath);
            _fullPath = null;
            _dbContent = null;
            _isDirty = false;
            _errorMessage = "Close Without Sync";
            Dispose();
        }

        /// <summary>
        /// Close the DB
        /// </summary>
        public void SyncAndClose()
        {
            Logger.Info(null, "SyncAndClose " + _fullPath);
            Sync();
            Dispose();
        }

        /// <summary>
        /// This must be closed to release the potential mutex (does not save DB!)
        /// </summary>
        public void Dispose()
        {
            _instance = null;
        }

        /// <summary>
        /// Write the DB to the persistent store.
        /// </summary>
        /// <returns>success</returns>
        public bool Sync()
        {
            _errorMessage = "";
            Directory.CreateDirectory(Path.GetDirectoryName(_connection));
            using (FileStream stream = File.Create(_fullPath))
            {
                //_dbContent.LastSaved = DateTime.Now;
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                JsonLicenseDb.OuterMutex.WaitOne();
                try
                {
                    //JsonSerializer.SerializeAsync<JsonDbHeader>(stream, _dbHeader, options).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                    Logger.Error(null, _errorMessage);
                    throw new Exception("Cannot serialize " + _fullPath, ex);
                }
                finally
                {
                    JsonLicenseDb.OuterMutex.ReleaseMutex();
                }
            }
            _isDirty = false;
            return true;
        }

        /// <summary>
        /// If there has been a DB change (isDirty) and we've been idle too long, save the DB.
        /// </summary>
        public void IdleTimeSync()
        {
            if (!_isDirty || DateTime.Now.Subtract(_lastAutoSave).TotalSeconds < AUTO_SAVE_SECONDS)
            {
                return;
            }
            _lastAutoSave = DateTime.Now;
            Sync();
            _isDirty = false;
        }

        /// <summary>
        /// Has the data been changed since the last save/sync was done?
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                _isDirty = value;
            }
        }

        /// <summary>
        /// True if the last operation was successful. 
        /// </summary>
        public bool Successful
        {
            get
            {
                return _errorMessage.Length > 0;
            }
        }

        /// <summary>
        /// Get the last error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <param name="table">DB table to update</param>
        /// <param name="recordName">man-readable name of record type for logging and errors</param>
        /// <returns>Success</returns>
        public bool UpdateDb(BaseDbRecord record, Dictionary<Guid, BaseDbRecord> table, string recordName)
        {
            _errorMessage = "";
            if(record.EditFlag == EditFlag.Zombie)
            {
                _errorMessage = "Cannot Update DB with Zombie Record " + record;
                Logger.Error(null, _errorMessage);
                return false;
            }
            if (!PerformUnecessaryUpdates && record.EditFlag == EditFlag.Unchanged)
            {
                Logger.Diag(null, "Skipping update to Unchanged record " + record);
                return true;
            }
            _isDirty = true;
            record.DateModified = DateTime.Now;
            JsonLicenseDb.InnerMutex.WaitOne();
            try
            {
                try
                {
                    Logger.Diag(null, "Deleting " + recordName + " " + record.Id);
                    table.Remove(record.Id); // ignore missing record
                }
                catch (ArgumentNullException ex)
                {
                    _errorMessage = "Problem deleting " + recordName;
                    Logger.Error(null, _errorMessage, ex);
                    return false;
                }
                if (record.EditFlag != EditFlag.Deleted)
                {
                    try
                    {
                        Logger.Diag(null, "Adding " + recordName + " " + record.ToString());
                        record.UnMod();
                        table.Add(record.Id, record);
                    }
                    catch (ArgumentException ex)
                    {
                        _errorMessage = "Problem adding " + recordName;
                        Logger.Error(null, _errorMessage, ex);
                        return false;
                    }
                }
            }
            finally
            {
                JsonLicenseDb.InnerMutex.ReleaseMutex();
            }
            return true;
        }

        /////////////////////////// LicenseRecord ////////////////////////////

        /// <summary>
        /// NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO 
        /// </summary>
        /// <param name="desc">Assigned site description - may end with wildcard "*"</param>
        /// <returns></returns>
        public LicenseRecord[] LicenseByDesc(string desc)
        {
            return new LicenseRecord[] { new LicenseRecord(), };
        }

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public Dictionary<Guid, LicenseRecord>.Enumerator LicencesEnumerator
        {
            get
            {
                _errorMessage = "";
                Dictionary<Guid, LicenseRecord>.Enumerator enumerator = 
                    new Dictionary<Guid, LicenseRecord>().GetEnumerator();
                JsonLicenseDb.InnerMutex.WaitOne();
                try
                {
                    enumerator = _dbContent.LicenseRecords.GetEnumerator();
                }
                catch(Exception ex)
                {
                    _errorMessage = "Problem getting enumerator";
                    Logger.Error(null, _errorMessage, ex);
                }
                finally
                {
                    JsonLicenseDb.InnerMutex.ReleaseMutex();
                }
                return enumerator;
            }
        }

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public List<LicenseRecord> LicensesByDescription(string descRegex)
        {
            _errorMessage = "";
            Regex regex = null;
            List<LicenseRecord> results = new List<LicenseRecord>();
            try
            {
                regex = new Regex(descRegex);
            }
            catch(ArgumentException ex)
            {
                _errorMessage = "Bad regex " + descRegex;
                Logger.Error(null, _errorMessage, ex);
                return results;
            }
            JsonLicenseDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, LicenseRecord>.Enumerator enumerator = _dbContent.LicenseRecords.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LicenseRecord record = enumerator.Current.Value;
                    if (regex.Match(record.Desc).Success)
                    {
                        Logger.Diag(null, "Matches description " + descRegex + " -> " + record.ToString());
                        results.Add(record);
                    }
                }
            }
            finally
            {
                JsonLicenseDb.InnerMutex.ReleaseMutex();
            }
            return results;
        }

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <returns>Success</returns>
        public bool UpdateDb(LicenseRecord record)
        {
            return UpdateDb(record, (Dictionary<Guid, BaseDbRecord>)(Object)_dbContent.LicenseRecords, "LicenseRecord");
        }


    }

}
