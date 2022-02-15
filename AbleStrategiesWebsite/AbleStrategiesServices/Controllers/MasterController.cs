using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

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
            return new string[] {
                Version.ToString(), 
                now.ToString("o", CultureInfo.GetCultureInfo("en-US")),
                (DateTime.Now.Ticks / (DateTime.Now.Millisecond + 173L)).ToString() // future use
            };
        }

        // GET as/master/user?desc=*
        /// <summary>
        /// Return users by assigned site description.
        /// </summary>
        /// <param name="cmd">literal, "user"</param>
        /// <param name="desc">Assigned site description - may end with wildcard "*"</param>
        /// <returns>List of matching users</returns>
        [HttpGet("{cmd}")]
        public ActionResult<UserData[]> Get([FromRoute] string cmd, [FromQuery]string desc)
        {
            if(!cmd.ToLower().StartsWith("user"))
            {
                return new UserData[] { };
            }
            return UserDb.Instance.UsersByDesc(desc);
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
