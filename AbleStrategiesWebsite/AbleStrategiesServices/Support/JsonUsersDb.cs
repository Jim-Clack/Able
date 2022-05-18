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
    public class JsonUsersDb : BaseUsersDb
    {

        /// <summary>
        /// Until we do exhaustive testing, leave this set to true.
        /// </summary>
        private static bool PerformUnecessaryUpdates = true;

        /// <summary>
        /// Here's the in-memory db.
        /// </summary>
        private DbContent dbContent = null;

        /// <summary>
        /// Our one-and-only instance.
        /// </summary>
        private static JsonUsersDb instance = null;

        /// <summary>
        /// Path and name of JSON file with all data.
        /// </summary>
        private static string connection = "./license.json";

        /// <summary>
        /// How often to autosave if changes were made.
        /// </summary>
        public const long AUTO_SAVE_SECONDS = 19;

        /// <summary>
        /// WHen did we last autosave?
        /// </summary>
        private static DateTime lastAutoSave = DateTime.Now;

        /// <summary>
        /// This is the full path to the JSON DB file. 
        /// </summary>
        private string fullPath = null;

        /// <summary>
        /// Description of last error. "" if none.
        /// </summary>
        private string errorMessage = "";

        /// <summary>
        /// Has the data changed since the last save/sync was done?
        /// </summary>
        private bool isDirty = false;

        /// <summary>
        /// Inner mutex to avoid races.
        /// </summary>
        private static Mutex innerMutex = new Mutex();

        /// <summary>
        /// Outer mutex to avoid races.
        /// </summary>
        private static Mutex outerMutex = new Mutex();

        // Getters/Setters
        public static Mutex InnerMutex { get => innerMutex; }
        public static Mutex OuterMutex { get => outerMutex; }

        /// <summary>
        /// Fetch the singleton.
        /// </summary>
        public static JsonUsersDb Instance
        {
            get
            {
                // This is NOT a redundant "if" but it is here so we don't lock for 
                // initial simultaneous Instance reference, crippling performance.
                if (instance == null)
                {
                    outerMutex.WaitOne();
                    try
                    {
                        if (instance == null)
                        {
                            instance = new JsonUsersDb();
                        }
                    }
                    finally
                    {
                        outerMutex.ReleaseMutex();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// For unit testing only!
        /// </summary>
        public static void PurgeExisting()
        {
            if(instance != null)
            {
                instance.CloseWithoutSync();
            }
            outerMutex.WaitOne();
            try
            {
                File.Delete(connection);
            }
            finally
            {
                outerMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        private JsonUsersDb()
        {
            fullPath = connection;
            errorMessage = "";
            isDirty = false;
            if (File.Exists(connection))
            {
                JsonUsersDb.InnerMutex.WaitOne();
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.AllowTrailingCommas = true;
                try
                {
                    using (FileStream stream = File.OpenRead(fullPath))
                    {
                        dbContent = JsonSerializer.DeserializeAsync<DbContent>(stream, options).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    throw new Exception("Cannot deserialize " + fullPath, ex);
                }
                finally
                {
                    JsonUsersDb.InnerMutex.ReleaseMutex();
                }
            }
            else
            {
                dbContent = new DbContent();
            }
            dbContent.DbName = Path.GetFileNameWithoutExtension(fullPath);
        }

        /// <summary>
        /// Close the DB without modifiying it. (if it was open too long it may have already been modified)
        /// </summary>
        public override void CloseWithoutSync()
        {
            Logger.Info(null, "Close Without Sync " + fullPath);
            fullPath = null;
            dbContent = null;
            isDirty = false;
            errorMessage = "Close Without Sync";
            Dispose();
        }

        /// <summary>
        /// Close the DB
        /// </summary>
        public override void SyncAndClose()
        {
            Logger.Info(null, "SyncAndClose " + fullPath);
            Sync();
            Dispose();
        }

        /// <summary>
        /// This must be closed to release the potential mutex (does not save DB!)
        /// </summary>
        public override void Dispose()
        {
            instance = null;
        }

        /// <summary>
        /// Write the DB to the persistent store.
        /// </summary>
        /// <returns>success</returns>
        public override bool Sync()
        {
            errorMessage = "";
            Directory.CreateDirectory(Path.GetDirectoryName(connection));
            using (FileStream stream = File.Create(fullPath))
            {
                //_dbContent.LastSaved = DateTime.Now;
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                JsonUsersDb.OuterMutex.WaitOne();
                try
                {
                    //JsonSerializer.SerializeAsync<JsonDbHeader>(stream, _dbHeader, options).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    Logger.Error(null, errorMessage);
                    throw new Exception("Cannot serialize " + fullPath, ex);
                }
                finally
                {
                    JsonUsersDb.OuterMutex.ReleaseMutex();
                }
            }
            isDirty = false;
            return true;
        }

        /// <summary>
        /// If there has been a DB change (isDirty) and we've been idle too long, save the DB.
        /// </summary>
        public override void IdleTimeSync()
        {
            if (!isDirty || DateTime.Now.Subtract(lastAutoSave).TotalSeconds < AUTO_SAVE_SECONDS)
            {
                return;
            }
            lastAutoSave = DateTime.Now;
            Sync();
            isDirty = false;
        }

        /// <summary>
        /// Has the data been changed since the last save/sync was done?
        /// </summary>
        public override bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                isDirty = value;
            }
        }

        /// <summary>
        /// True if the last operation was successful. 
        /// </summary>
        public override bool Successful
        {
            get
            {
                return errorMessage.Length > 0;
            }
        }

        /// <summary>
        /// Get the last error message.
        /// </summary>
        public override string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
        }

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <typeparam name="T">Record class derived from BaseDbRecord</typeparam>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <param name="table">DB table to update</param>
        /// <returns>Success</returns>
        public override bool UpdateDbPerEditFlag<T>(T record, Dictionary<Guid, T> table) // where T : BaseDbRecord
        {
            errorMessage = "";
            if(record.EditFlag == EditFlag.Zombie)
            {
                errorMessage = "Cannot Update DB with Zombie Record " + record;
                Logger.Error(null, errorMessage);
                return false;
            }
            if (!PerformUnecessaryUpdates && record.EditFlag == EditFlag.Unchanged)
            {
                Logger.Diag(null, "Skipping update to Unchanged record " + record);
                return true;
            }
            isDirty = true;
            record.DateModified = DateTime.Now;
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                try
                {
                    Logger.Diag(null, "Deleting " + record.Id);
                    table.Remove(record.Id); // ignore missing record
                }
                catch (ArgumentNullException ex)
                {
                    errorMessage = "Problem deleting " + record.GetType().Name;
                    Logger.Error(null, errorMessage, ex);
                    return false;
                }
                if (record.EditFlag != EditFlag.Deleted)
                {
                    try
                    {
                        Logger.Diag(null, "Adding " + record.ToString());
                        table.Add(record.Id, record);
                        record.UnMod();
                    }
                    catch (ArgumentException ex)
                    {
                        errorMessage = "Problem adding " + record.GetType().Name;
                        Logger.Error(null, errorMessage, ex);
                        return false;
                    }
                }
            }
            finally
            {
                JsonUsersDb.InnerMutex.ReleaseMutex();
            }
            return true;
        }

        /////////////////////////// LicenseRecord ////////////////////////////

        /// <summary>
        /// NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO 
        /// </summary>
        /// <param name="desc">Assigned site description - may end with wildcard "*"</param>
        /// <returns></returns>
        public override LicenseRecord[] LicenseByDesc(string desc)
        {
            return new LicenseRecord[] { new LicenseRecord(), };
        }

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public override Dictionary<Guid, LicenseRecord>.Enumerator LicencesEnumerator
        {
            get
            {
                errorMessage = "";
                Dictionary<Guid, LicenseRecord>.Enumerator enumerator = 
                    new Dictionary<Guid, LicenseRecord>().GetEnumerator();
                JsonUsersDb.InnerMutex.WaitOne();
                try
                {
                    enumerator = dbContent.LicenseRecords.GetEnumerator();
                }
                catch(Exception ex)
                {
                    errorMessage = "Problem getting enumerator";
                    Logger.Error(null, errorMessage, ex);
                }
                finally
                {
                    JsonUsersDb.InnerMutex.ReleaseMutex();
                }
                return enumerator;
            }
        }

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<LicenseRecord> LicensesByDescription(string descRegex)
        {
            errorMessage = "";
            Regex regex = null;
            List<LicenseRecord> results = new List<LicenseRecord>();
            try
            {
                regex = new Regex(descRegex);
            }
            catch(ArgumentException ex)
            {
                errorMessage = "Bad regex " + descRegex;
                Logger.Error(null, errorMessage, ex);
                return results;
            }
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, LicenseRecord>.Enumerator enumerator = dbContent.LicenseRecords.GetEnumerator();
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
                JsonUsersDb.InnerMutex.ReleaseMutex();
            }
            return results;
        }

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <returns>Success</returns>
        public override bool UpdateDb(LicenseRecord record)
        {
            return UpdateDbPerEditFlag<LicenseRecord>(record, dbContent.LicenseRecords);
        }

        //////////////////////////// DeviceRecord ////////////////////////////

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public override Dictionary<Guid, DeviceRecord>.Enumerator DevicesEnumerator
        {
            get
            {
                errorMessage = "";
                Dictionary<Guid, DeviceRecord>.Enumerator enumerator =
                    new Dictionary<Guid, DeviceRecord>().GetEnumerator();
                JsonUsersDb.InnerMutex.WaitOne();
                try
                {
                    enumerator = dbContent.DeviceRecords.GetEnumerator();
                }
                catch (Exception ex)
                {
                    errorMessage = "Problem getting enumerator";
                    Logger.Error(null, errorMessage, ex);
                }
                finally
                {
                    JsonUsersDb.InnerMutex.ReleaseMutex();
                }
                return enumerator;
            }
        }

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<DeviceRecord> DevicesByDescription(string descRegex)
        {
            errorMessage = "";
            Regex regex = null;
            List<DeviceRecord> results = new List<DeviceRecord>();
            try
            {
                regex = new Regex(descRegex);
            }
            catch(ArgumentException ex)
            {
                errorMessage = "Bad regex " + descRegex;
                Logger.Error(null, errorMessage, ex);
                return results;
            }
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, DeviceRecord>.Enumerator enumerator = dbContent.DeviceRecords.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DeviceRecord record = enumerator.Current.Value;
                    if (regex.Match(record.Desc).Success)
                    {
                        Logger.Diag(null, "Matches description " + descRegex + " -> " + record.ToString());
                        results.Add(record);
                    }
                }
            }
            finally
            {
                JsonUsersDb.InnerMutex.ReleaseMutex();
            }
            return results;
        }

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <returns>Success</returns>
        public override bool UpdateDb(DeviceRecord record)
        {
            return UpdateDbPerEditFlag<DeviceRecord>(record, dbContent.DeviceRecords);
        }

        /////////////////////////// PurchaseRecord ///////////////////////////

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public override Dictionary<Guid, PurchaseRecord>.Enumerator PurchasesEnumerator
        {
            get
            {
                errorMessage = "";
                Dictionary<Guid, PurchaseRecord>.Enumerator enumerator =
                    new Dictionary<Guid, PurchaseRecord>().GetEnumerator();
                JsonUsersDb.InnerMutex.WaitOne();
                try
                {
                    enumerator = dbContent.PurchaseRecords.GetEnumerator();
                }
                catch (Exception ex)
                {
                    errorMessage = "Problem getting enumerator";
                    Logger.Error(null, errorMessage, ex);
                }
                finally
                {
                    JsonUsersDb.InnerMutex.ReleaseMutex();
                }
                return enumerator;
            }
        }

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<PurchaseRecord> PurchasesByDescription(string descRegex)
        {
            errorMessage = "";
            Regex regex = null;
            List<PurchaseRecord> results = new List<PurchaseRecord>();
            try
            {
                regex = new Regex(descRegex);
            }
            catch (ArgumentException ex)
            {
                errorMessage = "Bad regex " + descRegex;
                Logger.Error(null, errorMessage, ex);
                return results;
            }
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, PurchaseRecord>.Enumerator enumerator = dbContent.PurchaseRecords.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PurchaseRecord record = enumerator.Current.Value;
                    if (regex.Match(record.Desc).Success)
                    {
                        Logger.Diag(null, "Matches description " + descRegex + " -> " + record.ToString());
                        results.Add(record);
                    }
                }
            }
            finally
            {
                JsonUsersDb.InnerMutex.ReleaseMutex();
            }
            return results;
        }

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <returns>Success</returns>
        public override bool UpdateDb(PurchaseRecord record)
        {
            return UpdateDbPerEditFlag<PurchaseRecord>(record, dbContent.PurchaseRecords);
        }

        ///////////////////////// InteractivityRecord ////////////////////////

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public override Dictionary<Guid, InteractivityRecord>.Enumerator InteractivitiesEnumerator
        {
            get
            {
                errorMessage = "";
                Dictionary<Guid, InteractivityRecord>.Enumerator enumerator =
                    new Dictionary<Guid, InteractivityRecord>().GetEnumerator();
                JsonUsersDb.InnerMutex.WaitOne();
                try
                {
                    enumerator = dbContent.InteractivityRecords.GetEnumerator();
                }
                catch (Exception ex)
                {
                    errorMessage = "Problem getting enumerator";
                    Logger.Error(null, errorMessage, ex);
                }
                finally
                {
                    JsonUsersDb.InnerMutex.ReleaseMutex();
                }
                return enumerator;
            }
        }

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<InteractivityRecord> InteractivitiesByDescription(string descRegex)
        {
            errorMessage = "";
            Regex regex = null;
            List<InteractivityRecord> results = new List<InteractivityRecord>();
            try
            {
                regex = new Regex(descRegex);
            }
            catch (ArgumentException ex)
            {
                errorMessage = "Bad regex " + descRegex;
                Logger.Error(null, errorMessage, ex);
                return results;
            }
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, InteractivityRecord>.Enumerator enumerator = dbContent.InteractivityRecords.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    InteractivityRecord record = enumerator.Current.Value;
                    if (regex.Match(record.Desc).Success)
                    {
                        Logger.Diag(null, "Matches description " + descRegex + " -> " + record.ToString());
                        results.Add(record);
                    }
                }
            }
            finally
            {
                JsonUsersDb.InnerMutex.ReleaseMutex();
            }
            return results;
        }

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <returns>Success</returns>
        public override bool UpdateDb(InteractivityRecord record)
        {
            return UpdateDbPerEditFlag<InteractivityRecord>(record, dbContent.InteractivityRecords);
        }

    }

}
