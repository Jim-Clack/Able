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
        /// Only allow this IP address to access DB generally
        /// </summary>
#if DEBUG
        private string SuperSuperUserIp = "::1";
#else
        private string SuperSuperUserIp = "192.2.2.2";
#endif

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
        public bool IsSuperSuperUser(System.Net.IPAddress ipAddress)
        {
            return ipAddress.ToString().ToUpper().Contains(SuperSuperUserIp);
        }

    }

}
