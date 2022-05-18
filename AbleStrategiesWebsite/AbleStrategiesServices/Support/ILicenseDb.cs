using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    public interface ILicenseDb
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desc">Assigned site description - may end with wildcard "*"</param>
        /// <returns></returns>
        LicenseRecord[] LicenseByDesc(string desc);

    }
}
