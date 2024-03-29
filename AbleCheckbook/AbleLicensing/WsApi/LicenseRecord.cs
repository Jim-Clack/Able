﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing.WsApi
{
    public class LicenseRecord
    {
        /// <summary>
        /// Globally unique Id for this record.
        /// </summary>
        public Guid Id = Guid.Empty;

        /// <summary>
        /// When was this record created?
        /// </summary>
        public DateTime DateCreated = DateTime.Now;

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        public DateTime DateModified = DateTime.Now;

        /// <summary>
        /// Unique record discriminator.
        /// </summary>
        public string LicenseCode = "";

        /// <summary>
        /// Contact name.
        /// </summary>
        public string ContactName = "";

        /// <summary>
        /// Street address.
        /// </summary>
        public string ContactAddress = "";

        /// <summary>
        /// City and state
        /// </summary>
        public string ContactCity = "";

        /// <summary>
        /// Postal code
        /// </summary>
        public string ContactZip = "";

        /// <summary>
        /// Contact phone.
        /// </summary>
        public string ContactPhone = "";

        /// <summary>
        /// Contact email.
        /// </summary>
        public string ContactEMail = "";

        /// <summary>
        /// License features bitmap.
        /// </summary>
        public string LicenseFeatures = "";

        /// <summary>
        /// Return a man-readable representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "LicRec{" + LicenseCode + "," + ContactName + ", " + ContactEMail + ", " + ContactPhone + "}";
        }

    }

}
