using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED.Middlewares
{
    public class ModelValidateAttribute : ActionFilterAttribute
    {
        private readonly bool _allowNull = false;

        public ModelValidateAttribute(bool allowNull = false)
        {
            _allowNull = allowNull;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!_allowNull)
                if (!context.ModelState.IsValid)
                {
                    var errorsRaw = JToken.FromObject(context.ModelState.Values
                        .SelectMany(a => a.Errors), new Newtonsoft.Json.JsonSerializer
                        {
                            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                        });

                    //errorsRaw["ErrorMessage"].ToList().ForEach(delegate (JToken message) {

                    //});

                    context.Result = new BadRequestObjectResult(new BadRequestModel
                    {
                        Code = $"PRM_01",
                        Status = StatusCodes.Status400BadRequest,
                        Message = Constants.BadRequestMessage,
                        Errors = errorsRaw
                    });
                }
        }
    }
}
