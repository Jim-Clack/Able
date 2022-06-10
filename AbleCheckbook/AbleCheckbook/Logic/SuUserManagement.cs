using AbleCheckbook.Logic;
using AbleCheckbook.Gui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AbleLicensing;

namespace AbleCheckbook.Logic
{

    public class SuUserManagement
    {

        /// <summary>
        /// Here's where we keep track of users.
        /// </summary>
        private List<SuUserData> _users = new List<SuUserData>();

        /// <summary>
        /// Flag as dirty when _userData is changed so we know to save it.
        /// </summary>
        private bool _isDirty = false;

        /// <summary>
        /// User Data is stored here.
        /// </summary>
        private string _userDataFilename = null;

        /// <summary>
        /// Users list.
        /// </summary>
        public List<SuUserData> Users { get => _users; set => _users = value; }

        /// <summary>
        /// Ctor.
        /// </summary>
        public SuUserManagement()
        {
            _userDataFilename = Path.Combine(Configuration.Instance.DirectoryConfiguration, "userdata.acb");
            LoadUserData();
        }

        /// <summary>
        /// Update the user database.
        /// </summary>
        public void Sync()
        {
            Backups.BackupNow(_userDataFilename, 8, ".ud", true, false);
            SaveUserData(_userDataFilename);
            Backups.PeriodicBackup(_userDataFilename, 7, 8, ".uw", false, true);
            _isDirty = false;
        }

        /// <summary>
        /// Get an activation PIN.
        /// </summary>
        /// <param name="siteIdentification">As displayed in the customer's About box</param>
        /// <param name="licenseCode">Unique 12 letter customer name, space, then zip or other location onfo</param>
        /// <returns>The activation PIN, or an error beginning with an "#" symbol</returns>
        public static string GetActivationPin(string siteIdentification, string licenseCode)
        {
            string expectedPin = "";
#if DEBUG
#if SUPERUSER
            if (Configuration.Instance.GetUserLevel() != UserLevel.SuperUser)
            {
                return Strings.Get("#Requires Super-User Permission");
            }
            licenseCode = licenseCode.Trim();
            siteIdentification = siteIdentification.Trim();
            if (siteIdentification.Length < 4)
            {
                return Strings.Get("#Invalid Site Identification Code.");
            }
            if (licenseCode.Contains(" ") || licenseCode.Length != 12 || !Char.IsPunctuation(licenseCode[6]))
            {
                return Strings.Get("#Invalid License Code - Expected 6-char Name, a Hyphen, then 5-char Postal Code.");
            }
            Activation act = Activation.Instance;
            expectedPin = act.CalculatePin(act.ChecksumOfString(act.SiteIdentification), siteIdentification, licenseCode);
#endif
#endif
            return expectedPin;
        }

        public void SaveUser(SuUserData userData)
        {
            SaveUserData(_userDataFilename);
        }

        public bool DeleteUser(SuUserData userData)
        {
            for (int index = 0; index < _users.Count; ++index)
            {
                SuUserData candidate = _users[index];
                if(candidate.Id == userData.Id)
                {
                    _users.RemoveAt(index);
                    SaveUserData(_userDataFilename);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Search for a user based on a substring found in the name, email, or other header field.
        /// </summary>
        /// <param name="pattern">case-insensitive substring</param>
        /// <returns>list of matches</returns>
        public List<SuUserData> FindInHeader(string pattern)
        {
            List<SuUserData> matches = new List<SuUserData>();
            pattern = pattern.Trim().ToUpper();
            if (pattern.Length < 1)
            {
                return matches;
            }
            foreach (SuUserData userData in _users)
            {
                if (userData.Contact.ToUpper().Contains(pattern) ||
                    userData.Company.ToUpper().Contains(pattern) ||
                    userData.ZipCode.ToUpper().Contains(pattern) ||
                    userData.EmailAddr.ToUpper().Contains(pattern) ||
                    userData.Important.ToUpper().Contains(pattern) ||
                    userData.PhoneNum.ToUpper().Contains(pattern) ||
                    userData.LicenseCode.ToUpper().Contains(pattern) ||
                    userData.SiteId.ToUpper().Contains(pattern) ||
                    userData.OtherInfo.ToUpper().Contains(pattern))
                {
                    matches.Add(userData);
                }
            }
            return matches;
        }

        /// <summary>
        /// Search for a user based on a substring found in the notes.
        /// </summary>
        /// <param name="pattern">case-insensitive substring.</param>
        /// <returns>list of matches</returns>
        public List<SuUserData> FindInNotes(string pattern)
        {
            List<SuUserData> matches = new List<SuUserData>();
            pattern = pattern.Trim().ToUpper();
            if (pattern.Length < 1)
            {
                return matches;
            }
            foreach (SuUserData userData in _users)
            {
                if (userData.Notes.ToUpper().Contains(pattern))
                {
                    matches.Add(userData);
                }
            }
            return matches;
        }

        /// <summary>
        /// Search for a user based on a match to the license code.
        /// </summary>
        /// <param name="pattern">case-insensitive string</param>
        /// <returns>list of matches</returns>
        public List<SuUserData> FindInLicenseCode(string pattern)
        {
            List<SuUserData> matches = new List<SuUserData>();
            pattern = pattern.Trim().ToUpper();
            if (pattern.Length < 1)
            {
                return matches;
            }
            foreach (SuUserData userData in _users)
            {
                if (userData.LicenseCode.ToUpper().Contains(pattern))
                {
                    matches.Add(userData);
                }
            }
            return matches;
        }

        /// <summary>
        /// Save user data to the database.
        /// </summary>
        /// <param name="filename">Name and path of file to be written.</param>
        private void SaveUserData(string filename)
        {
            try
            {
                using (FileStream stream = File.Create(filename))
                {
                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.WriteIndented = true;
                    JsonSerializer.SerializeAsync<List<SuUserData>>(stream, _users, options).GetAwaiter().GetResult();
                }
                _isDirty = false;
            }
            catch (Exception ex)
            {
                NotificationForm alert = new NotificationForm(false, "Cannot Save " + filename, ex.Message, false);
                alert.Show();
                Logger.Warn("Cannot Save " + filename, ex);
            }
        }

        /// <summary>
        /// Load _users from disk file. (De sure to save it first if it's dirty.)
        /// </summary>
        private void LoadUserData()
        {
            string filename = Path.Combine(Configuration.Instance.DirectoryConfiguration, "userdata.acb");
            try
            {
                if (File.Exists(filename))
                {
                    using (FileStream stream = File.OpenRead(filename))
                    {
                        _users = JsonSerializer.DeserializeAsync<List<SuUserData>>(stream).GetAwaiter().GetResult();
                    }
                    _isDirty = false;
                }
            }
            catch (Exception ex)
            {
                NotificationForm alert = new NotificationForm(false, "Cannot Load " + filename, ex.Message, false);
                alert.Show();
                Logger.Warn("Cannot Load " + filename, ex);
            }
        }

    }

}
