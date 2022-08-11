using System;
using System.Collections.Generic;
using System.Text;

namespace AbleStrategies.CheckbookWsApi
{
    public interface LicenseRecord
    {
        /// <summary>
        /// Globally unique Id for this record.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// When was this record created?
        /// </summary>
        DateTime DateCreated { get; set; }

        /// <summary>
        /// When was this record last modified?
        /// </summary>
        DateTime DateModified { get; set; }

        /// <summary>
        /// Unique record discriminator.
        /// </summary>
        string LicenseCode { get; set; }

        /// <summary>
        /// Contact name.
        /// </summary>
        string ContactName { get; set; }

        /// <summary>
        /// Street address.
        /// </summary>
        string ContactAddress { get; set; }

        /// <summary>
        /// City and state
        /// </summary>
        string ContactCity { get; set; }

        /// <summary>
        /// Postal code
        /// </summary>
        string ContactZip { get; set; }

        /// <summary>
        /// Contact phone.
        /// </summary>
        string ContactPhone { get; set; }

        /// <summary>
        /// Contact email.
        /// </summary>
        string ContactEMail { get; set; }

        /// <summary>
        /// License features bitmap.
        /// </summary>
        string LicenseFeatures { get; set; }

    }
}
