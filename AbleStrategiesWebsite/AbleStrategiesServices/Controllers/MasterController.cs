using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using AbleStrategiesServices.Support;
using Newtonsoft.Json;

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
        /// <summary>
        /// Simple APi to verify the connection.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            DateTime now = DateTime.Now.ToUniversalTime();
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            return new string[] {
                AbleStrategiesServices.Support.Version.ToString(), 
                now.ToString("o", CultureInfo.GetCultureInfo("en-US")),
                ipAddress,
                (DateTime.Now.Ticks / (DateTime.Now.Millisecond + 173L)).ToString(), // future
                "X" // future use
            };
        }

        // GET as/master/license?lic=.*
        /// <summary>
        /// Return licenses by assigned license code.
        /// </summary>
        /// <param name="cmd">literal, "licenses"</param>
        /// <param name="lic">Assigned license code - regular expression</param>
        /// <returns>List of matching licenses, null if not authorized</returns>
        [HttpGet("{cmd}")]
        public JsonResult Get([FromRoute] string cmd, [FromQuery]string lic)
        {
            if(!cmd.ToLower().StartsWith("license"))
            {
                return new JsonResult("");
            }
            string ip = HttpContext.Connection.RemoteIpAddress.ToString();
            if (SupportMethods.HasWildcard(lic) && !Configuration.Instance.IsSuperSuperUser(HttpContext.Connection.RemoteIpAddress))
            {
                Logger.Warn(HttpContext.Connection.RemoteIpAddress.ToString(), "Attempted unauthorized access [" + lic + "]");
                return null;
            }
            try
            {
                UserInfo[] userInfo = UserInfoDbo.Instance.GetByDescription(lic).ToArray();
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Formatting = Formatting.Indented;
                settings.MaxDepth = 20;
                settings.NullValueHandling = NullValueHandling.Include;
                settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
                settings.CheckAdditionalContent = true;
                settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
                settings.Culture = new CultureInfo("en-US");
                return new JsonResult(UserInfoDbo.Instance.GetByDescription(lic).ToArray(), settings);
            }
            catch(Exception ex)
            {
                Logger.Warn(null, "Failed to serialize to JSON");
            }
            return new JsonResult("");
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
