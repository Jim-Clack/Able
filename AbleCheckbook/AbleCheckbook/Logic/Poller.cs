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
            if (userInfoResponse.ApiState == (int)ApiState.ReturnOkReconfigure)
            {
                Reconfigure(userInfoResponse);
            }
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
        /// Perform reconfiguration per the message returned in userInfoResponse
        /// </summary>
        /// <param name="userInfoResponse">Contains Message that may contain reconfiguration commands appended to it</param>
        /// <remarks>
        /// userInfoResponse.Message is appended with reconfiguration commands, each preceded by a dollar signs.
        /// Each reconfiguration command begins with a key letter followed by arguments, separated by pipe symbols.
        /// i.e. Successful $ W httpt://ablestrategies.com/as/checkbook/ $ Ehttpt://mail.google.com | support@ablestrategies.com
        ///   W httpt://ablestrategies.com/as/checkbook/             -- reconfigure web service URL
        ///   E httpt://mail.google.com support@ablestrategies.com   -- reconfigure email server and address
        /// Blank spaces are not needed around delimiters but are shown above to make them obvious.
        /// The original message will be updated so that the reconfiguration commands are no longer present in it
        /// </remarks>
        private static void Reconfigure(UserInfoResponse userInfoResponse)
        {
            if(string.IsNullOrEmpty(userInfoResponse.Message.Trim()) || !userInfoResponse.Message.Contains("$"))
            {
                return;
            }
            string keys = "";
            string[] commands = userInfoResponse.Message.Trim().Split('$');
            for(int commandNumber = 1; commandNumber < commands.Length; ++commandNumber)
            {
                string command = commands[commandNumber].Trim();
                if(command.Length < 3)
                {
                    continue;
                }
                Logger.Info("Reconfiguring for " + command);
                char key = command.ToUpper()[0];
                keys += key;
                string[] args = command.Trim().Substring(1).Split('|');
                switch (key)
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
                    case 'P':
                        if (args.Length > 1)
                        {
                            Configuration.Instance.PayPalUrl = args[0].Trim();
                            Configuration.Instance.PayPalConfiguration = args[1].Trim();
                        }
                        break;
                    case 'W':
                        Configuration.Instance.WsUrlOverride = args[0].Trim();
                        break;
                    case 'A':
                        Configuration.Instance.AlertNotification = args[0].Trim();
                        break;
                    default:
                        Logger.Warn("Bad case for ReturnOkReconfigure: " + command);
                        break;
                }
            }
            // truncate to trim off reconfiguration commands
            userInfoResponse.Message = commands[0] + "\n(" + Strings.Get("Configuration updated for") + " " + keys + ")";
        }

    }

}
