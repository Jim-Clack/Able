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
    /// Note: For performance reasons, this ought to be upgraded to maintain suplicate record
    /// maps where there is an additional map for Fk as well as Id. But for now, it works.
    /// </remarks>
    public class JsonUsersDb : BaseUsersDb
    {

        /// <summary>
        /// Until we do exhaustive testing, leave this set to true.
        /// </summary>
        private static bool PerformUnecessaryUpdates = true;

        /// <summary>
        /// Are DB operations suspended?
        /// </summary>
        private static bool suspended = false;

        /// <summary>
        /// Our one-and-only instance.
        /// </summary>
        private static JsonUsersDb instance = null;

        /// <summary>
        /// Here's the in-memory db.
        /// </summary>
        private DbContent dbContent = null;

        /// <summary>
        /// Path and name of JSON file with all data.
        /// </summary>
#if DEBUG
        private static string connection = "./debug.json";
#else
        private static string connection = "./users.json";
#endif

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
        /// Fetch the singleton. Note: Do not hold onto the instance, say to store in in a variable. Always call this.
        /// </summary>
        public static JsonUsersDb Instance
        {
            get
            { 
                long loops = 0;
                while (suspended)
                {
                    Thread.Sleep(500);
                    if (++loops > 30)
                    {
                        Logger.Warn(null, "JsonUsersDb has been suspended for 15 seconds!");
                    }
                }
                EnsureInstanceExists();
                return instance;
            }
        }

        /// <summary>
        /// Create the instance if necessary.
        /// </summary>
        private static void EnsureInstanceExists()
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
        }

        /// <summary>
        /// Pause DB operations, hold back requests.
        /// </summary>
        public static void Suspend()
        {
            suspended = true;
            Thread.Sleep(2000); // allow current operations to complete
            if (instance != null)
            {
                instance.SyncAndClose();
            }
            instance = null;
        }

        /// <summary>
        /// Resume DB operations after a prior Suspend() call.
        /// </summary>
        public static void Resume()
        {
            suspended = false;
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
            fullPath = Path.GetFullPath(connection);
            errorMessage = "";
            isDirty = false;
            if (File.Exists(fullPath))
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
                    UnModDbContent();
                    Logger.Info(null, "Opened DB " + fullPath);
                }
                catch (Exception ex)
                {
                    errorMessage = "Error Opening DB " + fullPath + " " + ex.Message;
                    Logger.Error(null, "Error Opening DB " + fullPath, ex);
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
                Logger.Info(null, "Creating new DB " + fullPath);
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
        /// Return the full absolute path to the DB file.
        /// </summary>
        public string FullPath
        {
            get
            {
                return fullPath;
            }
        }

        /// <summary>
        /// Write the DB to the persistent store.
        /// </summary>
        /// <returns>success</returns>
        public override bool Sync()
        {
            errorMessage = "";
            Logger.Info(null, "Sync DB " + fullPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            using (FileStream stream = File.Create(fullPath))
            {
                //_dbContent.LastSaved = DateTime.Now;
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                options.IgnoreReadOnlyFields = true;
                options.IgnoreReadOnlyProperties = true;
                options.IncludeFields = false;
                options.WriteIndented = true;
                JsonUsersDb.OuterMutex.WaitOne();
                try
                {
                    JsonSerializer.SerializeAsync<DbContent>(stream, dbContent, options).GetAwaiter().GetResult();
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
        private bool UpdateDbPerEditFlag<T>(T record, Dictionary<Guid, T> table) where T : BaseDbRecord
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

        /// <summary>
        /// Since EditFlag is not persisted, set it in all records after reading in the DB anew.
        /// </summary>
        private void UnModDbContent()
        {
            Dictionary<Guid, LicenseRecord>.Enumerator licenseEnum = dbContent.LicenseRecords.GetEnumerator();
            while (licenseEnum.MoveNext())
            {
                ((LicenseRecord)licenseEnum.Current.Value).UnMod();
            }
            Dictionary<Guid, DeviceRecord>.Enumerator deviceEnum = dbContent.DeviceRecords.GetEnumerator();
            while (deviceEnum.MoveNext())
            {
                ((DeviceRecord)deviceEnum.Current.Value).UnMod();
            }
            Dictionary<Guid, PurchaseRecord>.Enumerator purchaseEnum = dbContent.PurchaseRecords.GetEnumerator();
            while (purchaseEnum.MoveNext())
            {
                ((PurchaseRecord)purchaseEnum.Current.Value).UnMod();
            }
            Dictionary<Guid, InteractivityRecord>.Enumerator interactivityEnum = dbContent.InteractivityRecords.GetEnumerator();
            while (interactivityEnum.MoveNext())
            {
                ((InteractivityRecord)interactivityEnum.Current.Value).UnMod();
            }
        }

        /////////////////////////// LicenseRecord ////////////////////////////

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public override Dictionary<Guid, LicenseRecord>.Enumerator LicensesEnumerator
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
        /// Find record with a specific ID
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>list containing the LicenseRecord, possibly empty</returns>
        public override List<LicenseRecord> LicensesById(Guid id)
        {
            errorMessage = "";
            List<LicenseRecord> results = new List<LicenseRecord>();
            LicenseRecord record = null;
            JsonUsersDb.InnerMutex.WaitOne();
            if (dbContent.LicenseRecords.ContainsKey(id))
            {
                try
                {
                    record = dbContent.LicenseRecords[id];
                    Logger.Diag(null, "Got ID " + id + " -> " + record.ToString());
                    results.Add(record);
                }
                finally
                {
                    JsonUsersDb.InnerMutex.ReleaseMutex();
                }
            }
            return results;
        }

        /// <summary>
        /// Find all records with a license code that matches a specific regex.
        /// </summary>
        /// <param name="licRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<LicenseRecord> LicensesByLicenseCode(string licRegex)
        {
            errorMessage = "";
            Regex regex = null;
            List<LicenseRecord> results = new List<LicenseRecord>();
            try
            {
                regex = new Regex(licRegex, RegexOptions.IgnoreCase);
            }
            catch(ArgumentException ex)
            {
                errorMessage = "Bad regex " + licRegex;
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
                    if (regex.Match(record.LicenseCode).Success)
                    {
                        Logger.Diag(null, "Matches License Code " + licRegex + " -> " + record.ToString());
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
        /// Find all records with a contact name that matches a specific regex.
        /// </summary>
        /// <param name="nameRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<LicenseRecord> LicensesByContactName(string nameRegex)
        {
            errorMessage = "";
            Regex regex = null;
            List<LicenseRecord> results = new List<LicenseRecord>();
            try
            {
                regex = new Regex(nameRegex, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException ex)
            {
                errorMessage = "Bad regex " + nameRegex;
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
                    if (regex.Match(record.ContactName).Success)
                    {
                        Logger.Diag(null, "Matches name " + nameRegex + " -> " + record.ToString());
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
        /// Find all records with a contact address that matches a specific regex.
        /// </summary>
        /// <param name="addressRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<LicenseRecord> LicensesByContactAddress(string addressRegex)
        {
            errorMessage = "";
            Regex regex = null;
            List<LicenseRecord> results = new List<LicenseRecord>();
            try
            {
                regex = new Regex(addressRegex, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException ex)
            {
                errorMessage = "Bad regex " + addressRegex;
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
                    if (regex.Match(record.ContactAddress).Success)
                    {
                        Logger.Diag(null, "Matches address " + addressRegex + " -> " + record.ToString());
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
        /// Find all records with a contact city that matches a specific regex.
        /// </summary>
        /// <param name="cityRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<LicenseRecord> LicensesByContactCity(string cityRegex)
        {
            errorMessage = "";
            Regex regex = null;
            List<LicenseRecord> results = new List<LicenseRecord>();
            try
            {
                regex = new Regex(cityRegex, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException ex)
            {
                errorMessage = "Bad regex " + cityRegex;
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
                    if (regex.Match(record.ContactCity).Success)
                    {
                        Logger.Diag(null, "Matches city " + cityRegex + " -> " + record.ToString());
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
        /// Find all records with a contact email that matches a specific regex.
        /// </summary>
        /// <param name="emailRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<LicenseRecord> LicensesByContactEmail(string emailRegex)
        {
            errorMessage = "";
            Regex regex = null;
            List<LicenseRecord> results = new List<LicenseRecord>();
            try
            {
                regex = new Regex(emailRegex, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException ex)
            {
                errorMessage = "Bad regex " + emailRegex;
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
                    if (regex.Match(record.ContactEMail).Success)
                    {
                        Logger.Diag(null, "Matches email " + emailRegex + " -> " + record.ToString());
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
        /// Find all records with a contact phone that matches a specific regex.
        /// </summary>
        /// <param name="contactPhone">The full opr partial phone number to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<LicenseRecord> LicensesByContactPhone(string contactPhone)
        {
            errorMessage = "";
            contactPhone = contactPhone.Replace(" ", "");
            List<LicenseRecord> results = new List<LicenseRecord>();
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, LicenseRecord>.Enumerator enumerator = dbContent.LicenseRecords.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LicenseRecord record = enumerator.Current.Value;
                    string recordPhone = record.ContactPhone;
                    if(recordPhone.Length > contactPhone.Length)
                    {
                        recordPhone = recordPhone.Substring(0, contactPhone.Length);
                    }
                    if (recordPhone.CompareTo(contactPhone) == 0)
                    {
                        Logger.Diag(null, "Matches phone " + contactPhone + " -> " + record.ToString());
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
        /// Find all records with a site code that matches a specific regex.
        /// </summary>
        /// <param name="siteCode">The site code to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<LicenseRecord> LicensesBySiteCode(string siteCode)
        {
            errorMessage = "";
            Regex regex = null;
            List<LicenseRecord> results = new List<LicenseRecord>();
            try
            {
                regex = new Regex(siteCode, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException ex)
            {
                errorMessage = "Bad regex " + siteCode;
                Logger.Error(null, errorMessage, ex);
                return results;
            }
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, LicenseRecord>.Enumerator licensesEnum = dbContent.LicenseRecords.GetEnumerator();
                while (licensesEnum.MoveNext())
                {
                    LicenseRecord license = licensesEnum.Current.Value;
                    List<DeviceRecord> devices = DevicesByFkLicense(license.Id);
                    foreach(DeviceRecord device in devices)
                    {
                        if (regex.Match(device.DeviceSiteId).Success)
                        {
                            Logger.Diag(null, "Matches device site code " + siteCode + " -> " + license.ToString());
                            results.Add(license);
                            break;
                        }
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
        /// Find all records with recent interactivity.
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<LicenseRecord> LicensesByRecentInteractivity(DateTime startDate)
        {
            errorMessage = "";
            List<LicenseRecord> results = new List<LicenseRecord>();
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, LicenseRecord>.Enumerator licensesEnum = dbContent.LicenseRecords.GetEnumerator();
                while (licensesEnum.MoveNext())
                {
                    LicenseRecord license = licensesEnum.Current.Value;
                    List<InteractivityRecord> interactivities = InteractivitiesByFkLicense(license.Id);
                    foreach (InteractivityRecord interactivity in interactivities)
                    {
                        if (interactivity.DateModified >= startDate)
                        {
                            Logger.Diag(null, "Matches interactivity since " + startDate.ToString() + " -> " + license.ToString());
                            results.Add(license);
                            break;
                        }
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
        /// Find all records created within a date range.
        /// </summary>
        /// <param name="startDate">First date to find.</param>
        /// <param name="endDate">Last date to find. (Use the next day if it has no time component)</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<LicenseRecord> LicensesByOriginalDate(DateTime startDate, DateTime endDate)
        {
            errorMessage = "";
            List<LicenseRecord> results = new List<LicenseRecord>();
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, LicenseRecord>.Enumerator enumerator = dbContent.LicenseRecords.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    LicenseRecord record = enumerator.Current.Value;
                    if (record.DateCreated >= startDate && record.DateCreated <= endDate)
                    { 
                        Logger.Diag(null, "Matches date range " + startDate.ToString() + " - " + endDate.ToString() + " -> " + record.ToString());
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
            return UpdateDbPerEditFlag<LicenseRecord>((LicenseRecord)record, dbContent.LicenseRecords);
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
        /// Find all records with a specific FK Id.
        /// </summary>
        /// <param name="fkId">The FK ID</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<DeviceRecord> DevicesByFkLicense(Guid fkId)
        {
            errorMessage = "";
            List<DeviceRecord> results = new List<DeviceRecord>();
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, DeviceRecord>.Enumerator enumerator = dbContent.DeviceRecords.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    DeviceRecord record = enumerator.Current.Value;
                    if (record.FkLicenseId == fkId)
                    {
                        Logger.Diag(null, "Matches fkId " + fkId + " -> " + record.ToString());
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
        /// Find all records with a given fk Id.
        /// </summary>
        /// <param name="fkId">The desired fk</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<PurchaseRecord> PurchasesByFkLicense(Guid fkId)
        {
            errorMessage = "";
            List<PurchaseRecord> results = new List<PurchaseRecord>();
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, PurchaseRecord>.Enumerator enumerator = dbContent.PurchaseRecords.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PurchaseRecord record = enumerator.Current.Value;
                    if (record.FkLicenseId == fkId)
                    {
                        Logger.Diag(null, "Matches fkId " + fkId + " -> " + record.ToString());
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
        /// Find all records with a given fk Id.
        /// </summary>
        /// <param name="fkId">The desired fk</param>
        /// <returns>List of matching records, possibly empty</returns>
        public override List<InteractivityRecord> InteractivitiesByFkLicense(Guid fkId)
        {
            errorMessage = "";
            List<InteractivityRecord> results = new List<InteractivityRecord>();
            JsonUsersDb.InnerMutex.WaitOne();
            try
            {
                Dictionary<Guid, InteractivityRecord>.Enumerator enumerator = dbContent.InteractivityRecords.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    InteractivityRecord record = enumerator.Current.Value;
                    if (record.FkLicenseId == fkId)
                    {
                        Logger.Diag(null, "Matches fkId " + fkId + " -> " + record.ToString());
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
