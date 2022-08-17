using System;
using System.Collections.Generic;
using System.Text;

namespace AbleCheckbook.WsApi
{
    public class LicenseRecord
    {
        /// <summary>
        /// Globally unique Id for this record.
        /// </summary>
        public Guid Id;

        /// <summary>
        /// When was this record created?
        /// </summary>
        public DateTime DateCreated;

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        public DateTime DateModified;

        /// <summary>
        /// Unique record discriminator.
        /// </summary>
        public string LicenseCode;

        /// <summary>
        /// Contact name.
        /// </summary>
        public string ContactName;

        /// <summary>
        /// Street address.
        /// </summary>
        public string ContactAddress;

        /// <summary>
        /// City and state
        /// </summary>
        public string ContactCity;

        /// <summary>
        /// Postal code
        /// </summary>
        public string ContactZip;

        /// <summary>
        /// Contact phone.
        /// </summary>
        public string ContactPhone;

        /// <summary>
        /// Contact email.
        /// </summary>
        public string ContactEMail;

        /// <summary>
        /// License features bitmap.
        /// </summary>
        public string LicenseFeatures;

    }
}
