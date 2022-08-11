using System;
using System.Collections.Generic;
using System.Text;

namespace AbleStrategies.CheckbookWsApi
{
    public interface UserInfoResponse
    {
        /// <summary>
        /// API State as an int for API usage, typically a Response or Purchase value. Not persisted.
        /// </summary>
        int ApiState { get; set; }

        /// <summary>
        /// Descriptive or diagnostic or error message. Not persisted.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// PIN number, if specifically requested. Not persisted.
        /// </summary>
        string PinNumber { get; set; }

        /// <summary>
        /// List of returned user info objects.
        /// </summary>
        List<UserInfo> UserInfos { get; set; }

    }
}
