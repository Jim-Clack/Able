﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing.WsApi
{
    public class InteractivityRecord
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
        /// Foreign key to license data.
        /// </summary>
        public Guid FkLicenseId = Guid.Empty;

        /// <summary>
        /// Interactivity by phone, web service, or what?
        /// </summary>
        public int InteractivityKind = 0;

        /// <summary>
        /// Client name, email, and/or IP address.
        /// </summary>
        public string ClientInfo = "";

        /// <summary>
        /// Content - what occurred during interactivity.
        /// </summary>
        public string Conversation = "";

        /// <summary>
        /// Keep track of changes.
        /// </summary>
        public string History = "";

        /// <summary>
        /// Return a man-readable representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "IntRec{" + InteractivityKind + "," + ClientInfo + ", " + Conversation + "}";
        }

    }

}
