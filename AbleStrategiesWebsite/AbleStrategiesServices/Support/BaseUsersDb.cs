using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    public abstract class BaseUsersDb
    {

        /// <summary>
        /// Close the DB without modifiying it. (if it was open too long it may have already been modified)
        /// </summary>
        public abstract void CloseWithoutSync();

        /// <summary>
        /// Close the DB
        /// </summary>
        public abstract void SyncAndClose();

        /// <summary>
        /// This must be closed to release the potential mutex (does not save DB!)
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Write the DB to the persistent store.
        /// </summary>
        /// <returns>success</returns>
        public abstract bool Sync();

        /// <summary>
        /// If there has been a DB change (isDirty) and we've been idle too long, save the DB.
        /// </summary>
        public abstract void IdleTimeSync();

        /// <summary>
        /// Has the data been changed since the last save/sync was done?
        /// </summary>
        public abstract bool IsDirty { get; set; }

        /// <summary>
        /// True if the last operation was successful. 
        /// </summary>
        public abstract bool Successful { get; }

        /// <summary>
        /// Get the last error message.
        /// </summary>
        public abstract string ErrorMessage { get; }

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <typeparam name="T">Record class derived from BaseDbRecord</typeparam>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <param name="table">DB table to update</param>
        /// <returns>Success</returns>
        public abstract bool UpdateDbPerEditFlag<T>(T record, Dictionary<Guid, T> table) where T : BaseDbRecord;

        /////////////////////////// LicenseRecord ////////////////////////////

        /// NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO NO 
        public abstract LicenseRecord[] LicenseByDesc(string desc);

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public abstract Dictionary<Guid, LicenseRecord>.Enumerator LicencesEnumerator { get; }

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<LicenseRecord> LicensesByDescription(string descRegex);

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <returns>Success</returns>
        public abstract bool UpdateDb(LicenseRecord record);

        //////////////////////////// DeviceRecord ////////////////////////////

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public abstract Dictionary<Guid, DeviceRecord>.Enumerator DevicesEnumerator { get; }

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<DeviceRecord> DevicesByDescription(string descRegex);

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <returns>Success</returns>
        public abstract bool UpdateDb(DeviceRecord record);

        /////////////////////////// PurchaseRecord ///////////////////////////

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public abstract Dictionary<Guid, PurchaseRecord>.Enumerator PurchasesEnumerator { get; }

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<PurchaseRecord> PurchasesByDescription(string descRegex);

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <returns>Success</returns>
        public abstract bool UpdateDb(PurchaseRecord record);

        ///////////////////////// InteractivityRecord ////////////////////////

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public abstract Dictionary<Guid, InteractivityRecord>.Enumerator InteractivitiesEnumerator { get; }

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<InteractivityRecord> InteractivitiesByDescription(string descRegex);

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <returns>Success</returns>
        public abstract bool UpdateDb(InteractivityRecord record);

    }
}
