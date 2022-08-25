using System;
using System.Collections.Generic;
using System.Text;

namespace AbleLicensing.WsApi
{
    public class UserInfoResponse
    {
        /// <summary>
        /// API State as an int for API usage, typically a Response or Purchase value. Not persisted.
        /// </summary>
        public int ApiState;

        /// <summary>
        /// Descriptive or diagnostic or error message. Not persisted.
        /// </summary>
        public string Message;

        /// <summary>
        /// PIN number, if specifically requested. Not persisted.
        /// </summary>
        public string PinNumber;

        /// <summary>
        /// List of returned user info objects.
        /// </summary>
        public List<UserInfo> UserInfos;

        /// <summary>
        /// Format man-readable string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result = "UiResp{" + ApiState + ", " + PinNumber + ", " + Message + "}";
            foreach(UserInfo userInfo in UserInfos)
            {
                result = result + "\n " + userInfo.ToString();
            }
            return result;
        }
    }
}
