using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using AbleStrategiesServices.Support;

/// <summary>                                      MASTER
/// https://domain:port/as/master
/// </summary>
namespace AbleStrategiesServices.Controllers
{
    [Route("as/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {

        // GET as/master
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            DateTime now = DateTime.Now.ToUniversalTime();
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            return new string[] {
                AbleStrategiesServices.Support.Version.ToString(), 
                now.ToString("o", CultureInfo.GetCultureInfo("en-US")),
                ipAddress,
                (DateTime.Now.Ticks / (DateTime.Now.Millisecond + 173L)).ToString() // future use
            };
        }

        // GET as/master/license?desc=*
        /// <summary>
        /// Return licenses by assigned site description.
        /// </summary>
        /// <param name="cmd">literal, "license"</param>
        /// <param name="desc">Assigned site description - may end with wildcard "*"</param>
        /// <returns>List of matching licenses</returns>
        [HttpGet("{cmd}")]
        public ActionResult<LicenseRecord[]> Get([FromRoute] string cmd, [FromQuery]string desc)
        {
            if(!cmd.ToLower().StartsWith("license"))
            {
                return new LicenseRecord[] { };
            }
            return JsonLicenseDb.Instance.LicenseByDesc(desc);
        }

        // POST as/master
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT as/master/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE as/master/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
