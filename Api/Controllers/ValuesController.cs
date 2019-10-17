using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        public IEnumerable<MyPayload> Get()
        {
            var myPayLoad = new MyPayload
            {
                Description = Resources.AmazingResource.Description,
                Timestamp = DateTime.UtcNow,
                Name = Resources.AmazingResource.Name
            };
            return new[] { myPayLoad };
        }

        //// GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public IHttpActionResult Post([FromBody]MyPayload valuevalue)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
