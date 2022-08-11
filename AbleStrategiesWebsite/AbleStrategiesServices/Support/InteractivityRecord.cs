using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{

    public class InteractivityRecord : BaseDbRecord
    {

        /// <summary>
        /// Foreign key to license data.
        /// </summary>
        private Guid fkLicenseId = Guid.Empty;

        /// <summary>
        /// Interactivity by phone, web service, or what?
        /// </summary>
        private InteractivityClient interactivityClient = InteractivityClient.Unknown;

        /// <summary>
        /// Client name, email, and/or IP address.
        /// </summary>
        private string clientInfo = "";

        /// <summary>
        /// Content - what occurred during interactivity.
        /// </summary>
        private string conversation = "";

        /// <summary>
        /// Keep track of changes.
        /// </summary>
        private string history = "";

        /// <summary>
        /// Ctor.
        /// </summary>
        public InteractivityRecord() : base()
        {
        }

        /// <summary>
        /// Forieng key - license.
        /// </summary>
        public Guid FkLicenseId
        {
            get
            {
                return fkLicenseId;
            }
            set
            {
                fkLicenseId = value;
                Mod();
            }
        }

        /// <summary>
        /// Interactivity by phone, web service, or what?
        /// </summary>
        public InteractivityClient InteractivityClient
        {
            get
            {
                return interactivityClient;
            }
            set
            {
                interactivityClient = value;
                Mod();
            }
        }

        /// <summary>
        /// Client name, email, and/or IP address.
        /// </summary>
        public string ClientInfo
        {
            get
            {
                return clientInfo;
            }
            set
            {
                clientInfo = value;
                Mod();
            }
        }

        /// <summary>
        /// Content - what occurred during interactivity.
        /// </summary>
        public string Conversation
        {
            get
            {
                return conversation;
            }
            set
            {
                if (!string.IsNullOrEmpty(conversation))
                {
                    history = history + "##### " + DateTime.Now.ToShortDateString() + " #####\n " +
                        conversation.Replace("\n", "|").Replace("\r", "|") + "\n";
                }
                conversation = value;
                Mod();
            }
        }

        /// <summary>
        /// Return a man-readable representation.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "IntRec{" + SupportMethods.Shorten(Id.ToString()) +
                "," + SupportMethods.Shorten(fkLicenseId.ToString()) +
                "," + interactivityClient.ToString() +
                "," + clientInfo + 
                "," + SupportMethods.Shorten(conversation, 20) + "}";
        }

        /// <summary>
        /// Track changes to conversation.
        /// </summary>
        public string History
        {
            get
            {
                return history;
            }
            set
            {
                // cannot be changed directly
            }
        }

        /// <summary>
        /// Update all data fields except for Id - keep this.Id, ignore source.Id (adjusts EditFlag, too)
        /// </summary>
        /// <param name="source">record from which to copy all data except for Id</param>
        public override void PopulateFrom(BaseDbRecord source)
        {
            if (!PopulateBaseFrom(source))
            {
                return;
            }
            this.FkLicenseId = ((InteractivityRecord)source).FkLicenseId;
            this.InteractivityClient = ((InteractivityRecord)source).InteractivityClient;
            this.ClientInfo = ((InteractivityRecord)source).ClientInfo;
            this.Conversation = ((InteractivityRecord)source).Conversation;
        }

    }
}
