using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.Controllers
{
    [Obsolete("Used only for testing purposes", false)]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IAuthenticationManager authManager;

        public ValuesController(IAuthenticationManager _authManager)
        {
            authManager = _authManager;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public ActionResult<string> Post([FromBody] string value)
        {
            return value;
        }

        [HttpPost("token")]
        public ActionResult Authenticate([FromBody] LoginModel value)
        {
            if(authManager.IsValidUser(value))
            {
                return new OkObjectResult(value);
            }
            else
            {
                return new BadRequestObjectResult(value);
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
