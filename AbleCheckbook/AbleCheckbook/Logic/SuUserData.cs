using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{

    /// <summary>
    /// Super-User functionality - User Data record.
    /// </summary>
    public class SuUserData
    {
        private Guid _id = Guid.NewGuid();
        private string _companyName = "";
        private string _contactName = "";
        private string _siteDescription = "";
        private string _phoneNumber = "";
        private string _emailAddress = "";
        private string _zipCode = "";
        private string _otherContactInfo = "";
        private string _importantNotice = "";
        private UserLevel _userLevel = UserLevel.Standard;
        private string _activatedBy = "";
        private string _siteIdentification = "";
        private string _ipAddress = "";
        private DateTime _dateActivated = DateTime.Now;
        private DateTime _dateEntered = DateTime.Now;
        private DateTime _dateLastAccess = DateTime.Now;
        private DateTime _dateLastWebService = DateTime.Now;
        private string _notes = "";
        private string _hiddenInfo = "";

        public Guid Id { get => _id; set => _id = value; }
        public string Company { get => _companyName; set => _companyName = value; }
        public string Contact { get => _contactName; set => _contactName = value; }
        public string SiteId { get => _siteIdentification; set => _siteIdentification = value; }
        public string SiteDesc { get => _siteDescription; set => _siteDescription = value; }
        public string PhoneNum { get => _phoneNumber; set => _phoneNumber = value; }
        public string EmailAddr { get => _emailAddress; set => _emailAddress = value; }
        public string OtherInfo { get => _otherContactInfo; set => _otherContactInfo = value; }
        public string Important { get => _importantNotice; set => _importantNotice = value; }
        public string ActivBy { get => _activatedBy; set => _activatedBy = value; }
        public DateTime DateEnter { get => _dateEntered; set => _dateEntered = value; }
        public DateTime DateActiv { get => _dateActivated; set => _dateActivated = value; }
        public DateTime DateLastAcc { get => _dateLastAccess; set => _dateLastAccess = value; }
        public string Notes { get => _notes; set => _notes = value; }
        public string ZipCode { get => _zipCode; set => _zipCode = value; }
        public string HiddenInfo { get => _hiddenInfo; set => _hiddenInfo = value; }
        public DateTime DateLastWebService { get => _dateLastWebService; set => _dateLastWebService = value; }
        public string IpAddress { get => _ipAddress; set => _ipAddress = value; }
        public UserLevel UserLev { get => _userLevel; set => _userLevel = value; }
    }

}
