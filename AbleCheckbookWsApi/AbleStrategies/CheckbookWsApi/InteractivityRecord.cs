﻿using System;
using System.Collections.Generic;
using System.Text;

NO - OBSOLETE

namespace AbleStrategies.CheckbookWsApi
{
    public class InteractivityRecord
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
        /// Foreign key to license data.
        /// </summary>
        public Guid FkLicenseId;

        /// <summary>
        /// Interactivity by phone, web service, or what?
        /// </summary>
        public int InteractivityClient;

        /// <summary>
        /// Client name, email, and/or IP address.
        /// </summary>
        public string ClientInfo;

        /// <summary>
        /// Content - what occurred during interactivity.
        /// </summary>
        public string Conversation;

        /// <summary>
        /// Keep track of changes.
        /// </summary>
        public string History;

    }
}
