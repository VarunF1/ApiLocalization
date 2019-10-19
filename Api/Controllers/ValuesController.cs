using Api.Models;
using System;
using System.Web.Http;

namespace Api.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            var employee = new Employee
            {
                Name = Resources.Language.Name,
                Description = Resources.Language.Description,
                Timestamp = DateTime.UtcNow
            };

            return Ok(employee);
        }

        // POST api/values
        [HttpPost]
        public IHttpActionResult Post([FromBody]Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(employee);
        }

    }
}