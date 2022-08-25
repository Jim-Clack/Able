using AbleLicensing;
using AbleLicensing.WsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    public class Poller
    {

        /// <summary>
        /// Try to call the server, handle remote configuration, deactivation, etc.
        /// </summary>
        /// <param name="licenseCode">test license code, null to use configured license code</param>
        /// <param name="siteId">device siteIdentificaiton, null if licenseCode is null</param>
        /// <returns>populated UserInfoResponse, null on error</returns>
        public UserInfoResponse Poll(string licenseCode = null, string siteId = null)
        {
            if (licenseCode == null)
            {
                licenseCode = Activation.Instance.LicenseCode.Trim();
                siteId = Activation.Instance.SiteIdentification.Trim();
            }
            UserInfoResponse userInfoResponse =
                AbleLicensing.OnlineActivation.Instance.Poll(licenseCode, siteId, Logic.Version.AppMajor, Logic.Version.AppMinor);
            if (userInfoResponse == null || userInfoResponse.UserInfos.Count < 1)
            {
                return null;
            }
            if (userInfoResponse.ApiState == (int)ApiState.ReturnTimeout || userInfoResponse.UserInfos == null)
            {
                OnlineActivation.Instance.AdjustTimeout(true);
                return null;
            }
            if (userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode.Contains("" + UserLevelPunct.Deactivated) ||
               userInfoResponse.ApiState == (int)ApiState.ReturnDeactivate) // deactivate, if necessary
            {
                userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode += "XXXXXXXXXX";
                userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode =
                    userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode.Substring(0, 6) +
                    UserLevelPunct.Deactivated +
                    userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode.Substring(7, 5);
                Activation.Instance.LicenseCode = userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode;
            }
            if(userInfoResponse.ApiState == (int)ApiState.ReturnOkReconfigure && string.IsNullOrEmpty(userInfoResponse.Message.Trim()))
            {
                string msg = userInfoResponse.Message.Trim();
                string[] args = msg.Split('|');
                Logger.Info("Reconfiguring for " + msg);
                switch (msg[0])
                {
                    case 'E':
                        if (args.Length > 1)
                        {
                            Configuration.Instance.SmtpServer = args[0].Trim();
                            Configuration.Instance.SupportEmail = args[1].Trim();
                        }
                        break;
                    case 'H':
                        if (args.Length > 1)
                        {
                            Configuration.Instance.HelpPageUrl = args[0].Trim();
                            Configuration.Instance.HelpSearchUrl = args[1].Trim();
                        }
                        break;
                    case 'W':
                        Configuration.Instance.WsUrlOverride = args[0].Trim();
                        break;
                    default:
                        Logger.Warn("Bad case for ReturnOkReconfigure: " + msg);
                        break;
                }
                userInfoResponse.Message = Strings.Get("Configuration updated for") + " " + msg[0];
            }
            Configuration.Instance.Save();
            return userInfoResponse;
        }
    }
}
