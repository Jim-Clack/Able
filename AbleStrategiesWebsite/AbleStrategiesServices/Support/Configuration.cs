using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    /// <summary>
    /// Placeholder for future configurability.
    /// </summary>
    public class Configuration
    {

        /// <summary>
        /// Singleton, hence static...
        /// </summary>
        private static Configuration instance = null;

        /// <summary>
        /// How many hosts can be activated per license
        /// </summary>
        public static readonly int MaxDevicesPerLicense = 3;

        /// <summary>
        /// Only allow these IP addresses to access "as/master" APIs
        /// </summary>
        private string[] HyperUserIp = { "::1", "127.0.0.1", "192.2.2.2" };

        /// <summary>
        /// Ctor.
        /// </summary>
        private Configuration()
        {

        }

        /// <summary>
        /// Ths singleton.
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configuration();
                }
                return instance;
            }
        }

        /// <summary>
        /// Is this a super super user?
        /// </summary>
        /// <param name="ipAddress">The client IP address</param>
        /// <returns>true if a super super user</returns>
        public bool IsHyperUser(System.Net.IPAddress ipAddress)
        {
            return HyperUserIp.Contains(ipAddress.ToString().Trim().ToUpper());
        }

    }

}
