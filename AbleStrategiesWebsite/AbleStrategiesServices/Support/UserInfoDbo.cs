using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    /// <summary>
    /// This is the main API for database access.
    /// </summary>
    public class UserInfoDbo
    {

        /// <summary>
        /// Track errors.
        /// </summary>
        private string errorMessage = "";

        /// <summary>
        /// Singleton.
        /// </summary>
        private static UserInfoDbo instance = null;

        /// <summary>
        /// Ctor.
        /// </summary>
        private UserInfoDbo()
        {
        }

        /// <summary>
        /// Return our one-and-onnly instance.
        /// </summary>
        public static UserInfoDbo Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new UserInfoDbo();
                }
                return instance;
            }
        }

        /// <summary>
        /// Track errors.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
        }

        /// <summary>
        /// Fetch user objects by Id. (should return only one or zero matching records)
        /// </summary>
        /// <param name="id">unique license ID.</param>
        /// <returns>List of matching records null on error (see ErrorMessage)</returns>
        public List<UserInfo> GetById(Guid id)
        {
            List<LicenseRecord> licenseRecords = JsonUsersDb.Instance.LicensesById(id);
            string errorMessage = JsonUsersDb.Instance.ErrorMessage;
            if ((licenseRecords == null || licenseRecords.Count < 1) && !string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "License Id " + id + " FAILURE: " + errorMessage;
                return null;
            }
            return PopulateLists(licenseRecords);
        }

        /// <summary>
        /// Find all records with a description that matches a specific regex.
        /// </summary>
        /// <param name="descRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty, null on error</returns>
        public List<UserInfo> GetByDescription(string descRegex)
        {
            List<LicenseRecord> licenseRecords = JsonUsersDb.Instance.LicensesByDescription(descRegex);
            string errorMessage = JsonUsersDb.Instance.ErrorMessage;
            if ((licenseRecords == null || licenseRecords.Count < 1) && !string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "Description " + descRegex + " FAILURE: " + errorMessage;
                return null;
            }
            return PopulateLists(licenseRecords);
        }

        /// <summary>
        /// Find all records with a contact name that matches a specific regex.
        /// </summary>
        /// <param name="nameRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty, null on error</returns>
        public List<UserInfo> GetByContactName(string nameRegex)
        {
            List<LicenseRecord> licenseRecords = JsonUsersDb.Instance.LicensesByContactName(nameRegex);
            string errorMessage = JsonUsersDb.Instance.ErrorMessage;
            if ((licenseRecords == null || licenseRecords.Count < 1) && !string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "Name " + nameRegex + " FAILURE: " + errorMessage;
                return null;
            }
            return PopulateLists(licenseRecords);
        }

        /// <summary>
        /// Find all records with a contact address that matches a specific regex.
        /// </summary>
        /// <param name="addressRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty, null on error</returns>
        public List<UserInfo> GetByContactAddress(string addressRegex)
        {
            List<LicenseRecord> licenseRecords = JsonUsersDb.Instance.LicensesByContactAddress(addressRegex);
            string errorMessage = JsonUsersDb.Instance.ErrorMessage;
            if ((licenseRecords == null || licenseRecords.Count < 1) && !string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "Address " + addressRegex + " FAILURE: " + errorMessage;
                return null;
            }
            return PopulateLists(licenseRecords);
        }

        /// <summary>
        /// Find all records with a contact city that matches a specific regex.
        /// </summary>
        /// <param name="cityRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty, null on error</returns>
        public List<UserInfo> GetByContactCity(string cityRegex)
        {
            List<LicenseRecord> licenseRecords = JsonUsersDb.Instance.LicensesByContactCity(cityRegex);
            string errorMessage = JsonUsersDb.Instance.ErrorMessage;
            if ((licenseRecords == null || licenseRecords.Count < 1) && !string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "City " + cityRegex + " FAILURE: " + errorMessage;
                return null;
            }
            return PopulateLists(licenseRecords);
        }

        /// <summary>
        /// Find all records with a contact email that matches a specific regex.
        /// </summary>
        /// <param name="emailRegex">The regular expression to match</param>
        /// <returns>List of matching records, possibly empty, null on error</returns>
        public List<UserInfo> GetByContactEmail(string emailRegex)
        {
            List<LicenseRecord> licenseRecords = JsonUsersDb.Instance.LicensesByContactEmail(emailRegex);
            string errorMessage = JsonUsersDb.Instance.ErrorMessage;
            if ((licenseRecords == null || licenseRecords.Count < 1) && !string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "EMail " + emailRegex + " FAILURE: " + errorMessage;
                return null;
            }
            return PopulateLists(licenseRecords);
        }

        /// <summary>
        /// Find all records with a contact phone that fully or paritally matches.
        /// </summary>
        /// <param name="contactPhone">The number to match</param>
        /// <returns>List of matching records, possibly empty, null on error</returns>
        public List<UserInfo> GetByContactPhone(string contactPhone)
        {
            List<LicenseRecord> licenseRecords = JsonUsersDb.Instance.LicensesByContactPhone(contactPhone);
            string errorMessage = JsonUsersDb.Instance.ErrorMessage;
            if ((licenseRecords == null || licenseRecords.Count < 1) && !string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "Phone " + contactPhone + " FAILURE: " + errorMessage;
                return null;
            }
            return PopulateLists(licenseRecords);
        }

        /// <summary>
        /// Find all records with a site code that matches a specific regex.
        /// </summary>
        /// <param name="siteCode">The value to match</param>
        /// <returns>List of matching records, possibly empty, null on error</returns>
        public List<UserInfo> GetBySiteCode(string siteCode)
        {
            List<LicenseRecord> licenseRecords = JsonUsersDb.Instance.LicensesBySiteCode(siteCode);
            string errorMessage = JsonUsersDb.Instance.ErrorMessage;
            if ((licenseRecords == null || licenseRecords.Count < 1) && !string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "Site Code " + siteCode + " FAILURE: " + errorMessage;
                return null;
            }
            return PopulateLists(licenseRecords);
        }

        /// <summary>
        /// Find all records with recent interactivity.
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <returns>List of matching records, possibly empty, null on error</returns>
        public List<UserInfo> GetByRecentInteractivity(DateTime startDate)
        {
            List<LicenseRecord> licenseRecords = JsonUsersDb.Instance.LicensesByRecentInteractivity(startDate);
            string errorMessage = JsonUsersDb.Instance.ErrorMessage;
            if ((licenseRecords == null || licenseRecords.Count < 1) && !string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "Interactivity Since " + startDate.ToString() + " FAILURE: " + errorMessage;
                return null;
            }
            return PopulateLists(licenseRecords);
        }

        /// <summary>
        /// Find all records created within a date range.
        /// </summary>
        /// <param name="startDate">First date to find.</param>
        /// <param name="endDate">Last date to find.</param>
        /// <returns>List of matching records, possibly empty, null on error</returns>
        public List<UserInfo> GetByOriginalDate(DateTime startDate, DateTime endDate)
        {
            List<LicenseRecord> licenseRecords = JsonUsersDb.Instance.LicensesByOriginalDate(startDate, endDate);
            string errorMessage = JsonUsersDb.Instance.ErrorMessage;
            if ((licenseRecords == null || licenseRecords.Count < 1) && !string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "Description " + startDate.ToString() + " - " + endDate.ToString() + " FAILURE: " + errorMessage;
                return null;
            }
            return PopulateLists(licenseRecords);
        }

        /// <summary>
        /// Update the DB with any changes to the data.
        /// </summary>
        /// <param name="userInfo">possibly modified data to be written to the DB</param>
        /// <returns>success, see ErrorMessage</returns>
        public bool Update(UserInfo userInfo)
        {
            bool ok = true;
            errorMessage = "";
            if(userInfo == null)
            {
                return true;
            }
            if(userInfo.LicenseRecord == null)
            {
                errorMessage = "Null License Record passed into Update()";
                return false;
            }
            if(!JsonUsersDb.Instance.UpdateDb(userInfo.LicenseRecord))
            {
                ok = false;
                errorMessage = JsonUsersDb.Instance.ErrorMessage;
            }
            List<DeviceRecord>.Enumerator deviceEnum = userInfo.DeviceRecords.GetEnumerator();
            while (deviceEnum.MoveNext())
            {
                deviceEnum.Current.fkLicenseId = userInfo.LicenseRecord.Id; // avoid Setter, so as to not affect EditFlag
                if (!JsonUsersDb.Instance.UpdateDb(deviceEnum.Current))
                {
                    ok = false;
                    errorMessage = JsonUsersDb.Instance.ErrorMessage;
                }
            }
            List<PurchaseRecord>.Enumerator purchaseEnum = userInfo.PurchaseRecords.GetEnumerator();
            while (purchaseEnum.MoveNext())
            {
                purchaseEnum.Current.fkLicenseId = userInfo.LicenseRecord.Id; // avoid Setter, so as to not affect EditFlag
                if (!JsonUsersDb.Instance.UpdateDb(purchaseEnum.Current))
                {
                    ok = false;
                    errorMessage = JsonUsersDb.Instance.ErrorMessage;
                }
            }
            List<InteractivityRecord>.Enumerator interactivityEnum = userInfo.InteractivityRecords.GetEnumerator();
            while (interactivityEnum.MoveNext())
            {
                interactivityEnum.Current.fkLicenseId = userInfo.LicenseRecord.Id; // avoid Setter, so as to not affect EditFlag
                if (!JsonUsersDb.Instance.UpdateDb(interactivityEnum.Current))
                {
                    ok = false;
                    errorMessage = JsonUsersDb.Instance.ErrorMessage;
                }
            }
            return ok;
        }

        /// <summary>
        /// Populate a list of userInfo objects.
        /// </summary>
        /// <param name="licenseRecords">License records</param>
        /// <returns>The list of userInfo if all is well; otherwise null</returns>
        private List<UserInfo> PopulateLists(List<LicenseRecord> licenseRecords)
        {
            List<UserInfo> results = new List<UserInfo>();
            foreach (LicenseRecord record in licenseRecords)
            {
                UserInfo userInfo = new UserInfo();
                userInfo.LicenseRecord = record;
                userInfo.PurchaseRecords = JsonUsersDb.Instance.PurchasesByFkLicense(userInfo.LicenseRecord.Id);
                userInfo.DeviceRecords = JsonUsersDb.Instance.DevicesByFkLicense(userInfo.LicenseRecord.Id);
                userInfo.InteractivityRecords = JsonUsersDb.Instance.InteractivitiesByFkLicense(userInfo.LicenseRecord.Id);
                results.Add(userInfo);
            }
            return results;
        }

    }

}
