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

        /////////////////////////// LicenseRecord ////////////////////////////

        /// <summary>
        /// Get a cursor/enumerator over all records.
        /// </summary>
        public abstract Dictionary<Guid, LicenseRecord>.Enumerator LicencesEnumerator { get; }

        /// <summary>
        /// Find record with a specific ID
        /// </summary>
        /// <param name="id">The Id</param>
        /// <returns>list containing the LicenseRecord, possibly empty</returns>
        public abstract List<LicenseRecord> LicensesById(Guid id);

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<LicenseRecord> LicensesByDescription(string descRegex);

        /// <summary>
        /// Find all records with a contact name that matches a specific regex.
        /// </summary>
        /// <param name="nameRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<LicenseRecord> LicensesByContactName(string nameRegex);

        /// <summary>
        /// Find all records with a contact city that matches a specific regex.
        /// </summary>
        /// <param name="cityRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<LicenseRecord> LicensesByContactCity(string cityRegex);

        /// <summary>
        /// Find all records with a contact email that matches a specific regex.
        /// </summary>
        /// <param name="emailRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<LicenseRecord> LicensesByContactEmail(string emailRegex);

        /// <summary>
        /// Find all records with a contact phone that fully or paritally matches.
        /// </summary>
        /// <param name="contactPhone">The number to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<LicenseRecord> LicensesByContactPhone(string contactPhone);

        /// <summary>
        /// Find all records with a site code that matches a specific regex.
        /// </summary>
        /// <param name="siteCode">The value to match</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<LicenseRecord> LicensesBySiteCode(string siteCode);

        /// <summary>
        /// Find all records with recent interactivity.
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<LicenseRecord> LicensesByRecentInteractivity(DateTime startDate);

        /// <summary>
        /// Find all records created within a date range.
        /// </summary>
        /// <param name="startDate">First date to find.</param>
        /// <param name="endDate">Last date to find.</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<LicenseRecord> LicensesByOriginalDate(DateTime startDate, DateTime endDate);

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
        /// Find all records with a description that matches an FK Id.
        /// </summary>
        /// <param name="fkId">The FK Id</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<DeviceRecord> DevicesByFkLicense(Guid fkId);

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
        /// Find all records with a given fk Id.
        /// </summary>
        /// <param name="fkId">The desired fk</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<PurchaseRecord> PurchasesByFkLicense(Guid fkId);

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
        /// Find all records with a given fk Id.
        /// </summary>
        /// <param name="descRegex">The desired fk</param>
        /// <returns>List of matching records, possibly empty</returns>
        public abstract List<InteractivityRecord> InteractivitiesByFkLicense(Guid fkId);

        /// <summary>
        /// Update the DB with a record, per the editFlag
        /// </summary>
        /// <param name="record">Affected record, with the EditFlag set for the desired action</param>
        /// <returns>Success</returns>
        public abstract bool UpdateDb(InteractivityRecord record);

    }
}
