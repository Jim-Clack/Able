using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{

    public enum InteractivityClient
    {
        Unknown = 0,
        PhoneCall = 1,
        OnlineChat = 2,
        Email = 3,
        InPerson = 4,
        UserAlert = 7,
        ActivationWs = 8,
        OtherWs = 9,
    }

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
        /// [static] Unique record type discriminator.
        /// </summary>
        /// <returns>unique string discriminator</returns>
        public static string GetRecordKind()
        {
            return typeof(InteractivityRecord).Name;
        }

        /// <summary>
        /// Unique record type discriminator (note: implement as a call to a static method)
        /// </summary>
        public override string RecordKind
        {
            get
            {
                return InteractivityRecord.GetRecordKind();
            }
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
