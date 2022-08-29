using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    /// <summary>
    /// This is a JSON record, never a DB resord - not persisted.
    /// </summary>
    public class UserInfoResponse

    {

        /// <summary>
        /// API State as an int for API usage, typically a Response or Purchase value. Not persisted.
        /// </summary>
        private int apiState = 0;

        /// <summary>
        /// Descriptive or diagnostic or error message. Not persisted.
        /// </summary>
        private string message = "";

        /// <summary>
        /// PIN number, if specifically requested. Not persisted.
        /// </summary>
        private string pinNumber = "";

        /// <summary>
        /// List of returned user info objects.
        /// </summary>
        private List<UserInfo> userInfos = new List<UserInfo>();

        /// <summary>
        /// Used for pushing reconfiguration changes.
        /// </summary>
        private List<ReconfigurationRecord> reconfigurationRecords = new List<ReconfigurationRecord>();

        /// <summary>
        /// API State, typically a Response or Purchase value. Not persisted.
        /// </summary>
        public int ApiState { get => apiState; set => apiState = value; }

        /// <summary>
        /// Descriptive or diagnostic or error message. Not persisted.
        /// </summary>
        public string Message { get => message; set => message = value; }

        /// <summary>
        /// PIN number, if specifically requested. Not persisted.
        /// </summary>
        public string PinNumber { get => pinNumber; set => pinNumber = value; }

        /// <summary>
        /// Used for pushing reconfiguration changes.
        /// </summary>
        public List<ReconfigurationRecord> ReconfigurationRecords { get => reconfigurationRecords; set => reconfigurationRecords = value; }

        /// <summary>
        /// List of returned user info objects.
        /// </summary>
        public List<UserInfo> UserInfos { get => userInfos; set => userInfos = value; }

        /// <summary>
        /// Default ctor.
        /// </summary>
        public UserInfoResponse()
        {
        }

        /// <summary>
        /// Initialized ctor.
        /// </summary>
        /// <param name="apiState">Next API State from this response</param>
        /// <param name="userInfos">list of user infos, typically one entry, null becomes an empty list</param>
        /// <param name="message">explanatory message, possibly ""</param>
        /// <param name="pinNumber">pin number for certain responses, else ""</param>
        public UserInfoResponse(int apiState, List<UserInfo> userInfos, string message = "", string pinNumber = "")
        {
            ApiState = apiState;
            if (userInfos != null)
            {
                UserInfos = userInfos;
            }
            if (message != null)
            {
                Message = message;
            }
            if (pinNumber != null)
            {
                PinNumber = pinNumber;
            }
        }

    }
}
