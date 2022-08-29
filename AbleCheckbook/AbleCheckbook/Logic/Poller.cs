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
        /// <param name="briefTimeout">true to reset the OnlineActivation timeout to a minimum</param>
        /// <param name="licenseCode">test license code, null to use configured license code</param>
        /// <param name="siteId">device siteIdentificaiton, null if licenseCode is null</param>
        /// <returns>populated UserInfoResponse - esp ApiState, Message, and License; poss null on error</returns>
        public UserInfoResponse Poll(bool briefTimeout, string licenseCode = null, string siteId = null)
        {
            if (licenseCode == null)
            {
                licenseCode = Activation.Instance.LicenseCode.Trim();
                siteId = Activation.Instance.SiteIdentification.Trim();
            }
            if (briefTimeout)
            {
                OnlineActivation.Instance.AdjustTimeout(false); // never wait too long for incidental polls
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
                DeactivateSite(userInfoResponse);
            }
            Reconfigure(userInfoResponse.ReconfigurationRecords);
            Configuration.Instance.Save();
            return userInfoResponse;
        }

        /// <summary>
        /// Deactivate this host device per site ID.
        /// </summary>
        /// <param name="userInfoResponse">To be deactivated. Also updates local configuration</param>
        private static void DeactivateSite(UserInfoResponse userInfoResponse)
        {
            userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode += "XXXXXXXXXXXX"; // ensure it's long enough
            userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode =
                userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode.Substring(0, 6) +
                UserLevelPunct.Deactivated +
                userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode.Substring(7, 5);
            Activation.Instance.LicenseCode = userInfoResponse.UserInfos[0].LicenseRecord.LicenseCode;
        }

        /// <summary>
        /// Perform reconfiguration per userInfoResponse
        /// </summary>
        /// <param name="reconfigurationRecords">list of reconfiguration records</param>
        private static void Reconfigure(List<ReconfigurationRecord> reconfigurationRecords)
        {
            if(reconfigurationRecords == null || reconfigurationRecords.Count < 1)
            {
                return;
            }
            foreach(ReconfigurationRecord reconfig in reconfigurationRecords)
            {
                Logger.Info("Reconfiguring " + reconfig.ToString());
                switch ((int)reconfig.ReconfigureSelector)
                {
                    case (int)ReconfigurationSelection.Email:
                        if (reconfig.NewValues.Count > 1)
                        {
                            Configuration.Instance.SmtpServer = reconfig.NewValues[0].Trim();
                            Configuration.Instance.SupportEmail = reconfig.NewValues[1].Trim();
                        }
                        break;
                    case (int)ReconfigurationSelection.Help:
                        if (reconfig.NewValues.Count > 1)
                        {
                            Configuration.Instance.HelpPageUrl = reconfig.NewValues[0].Trim();
                            Configuration.Instance.HelpSearchUrl = reconfig.NewValues[1].Trim();
                        }
                        break;
                    case (int)ReconfigurationSelection.PayPal:
                        if (reconfig.NewValues.Count > 1)
                        {
                            Configuration.Instance.PayPalUrl = reconfig.NewValues[0].Trim();
                            Configuration.Instance.PayPalConfiguration = reconfig.NewValues[1].Trim();
                        }
                        break;
                    case (int)ReconfigurationSelection.WebService:
                        Configuration.Instance.WebServiceUrl = reconfig.NewValues[0].Trim();
                        break;
                    case (int)ReconfigurationSelection.Alert:
                        Configuration.Instance.AlertNotification = reconfig.NewValues[0].Trim();
                        break;
                    default:
                        Logger.Warn("Bad case for ReturnOkReconfigure: " + reconfig.ToString());
                        break;
                }
            }
        }

    }

}
