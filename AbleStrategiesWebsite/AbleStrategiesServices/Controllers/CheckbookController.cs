using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

/// <summary>                                      CHECKBOOK
/// https://domain:port/as/checkbook
/// </summary>
namespace AbleStrategiesServices.Controllers
{
    [Route("as/[controller]")]
    [ApiController]
    public class CheckbookController : ControllerBase
    {
        // GET as/checkbook
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "cb" };
        }

        // GET as/checkbook/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST as/checkbook
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT as/checkbook/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE as/checkbook/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}