using System;
using System.Collections.Generic;
using System.Text;

namespace AbleStrategies.CheckbookWsApi
{
    public interface InteractivityRecord
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
        /// Foreign key to license data.
        /// </summary>
        Guid FkLicenseId { get; set; }

        /// <summary>
        /// Interactivity by phone, web service, or what?
        /// </summary>
        int InteractivityClient { get; set; }

        /// <summary>
        /// Client name, email, and/or IP address.
        /// </summary>
        string ClientInfo { get; set; }

        /// <summary>
        /// Content - what occurred during interactivity.
        /// </summary>
        string Conversation { get; set; }

        /// <summary>
        /// Keep track of changes.
        /// </summary>
        string History { get; set; }

    }
}
