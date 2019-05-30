using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Turing_Back_ED.Middlewares
{
    [Microsoft.AspNetCore.Mvc.Infrastructure.DefaultStatusCode(500)]
    public class InternalServerErrorResultObject : ActionResult
    {
        private readonly object _error;

        /// <summary>
        /// Initializes a new instance of the InternalServerErrorResultObject class
        /// </summary>
        public InternalServerErrorResultObject(object error)
        {
            _error = error;
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(_error));
            return Task.CompletedTask;
        }
    }
}
