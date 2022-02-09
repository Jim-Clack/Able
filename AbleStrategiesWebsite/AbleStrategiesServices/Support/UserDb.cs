using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices
{
    public class UserDb
    {

        /// <summary>
        /// Our one-and-only instance.
        /// </summary>
        private static UserDb _instance = null;

        /// <summary>
        /// Private because this is a singleton.
        /// </summary>
        private UserDb()
        {
        }

        /// <summary>
        /// This must be a singleton as it is shared by web services in real time.
        /// </summary>
        public static UserDb Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserDb();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc">Assigned site description - may end with wildcard "*"</param>
        /// <returns></returns>
        public UserData[] UsersByDesc(string desc)
        {
            return new UserData[] { new UserData("test user") };
        }
    }
}
