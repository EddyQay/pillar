using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Turing_Back_ED.DomainModels;
using Turing_Back_ED.Utilities;

namespace Turing_Back_ED
{
    /// <summary>
    /// Handles all uncatched errors fo this API
    /// </summary>
    public class ErrorMiddleware
    {
        private readonly RequestDelegate next;
        readonly ILogger<ErrorMiddleware> logger;
        readonly IHostingEnvironment env;
        static readonly IHostingEnvironment env_s;


        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> _logger, IHostingEnvironment _env)
        {
            this.next = next;
            logger = _logger;
            env = _env;
        }

        static ErrorMiddleware()
        {
            env_s = new HostingEnvironment();
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, ex.Source, ex.TargetSite, ex.StackTrace);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected


            var result = JsonConvert
                .SerializeObject(
                    new ErrorRequestModel
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Code = nameof(Constants.ErrorMessages.ERR_02),
                        Message = env_s.IsProduction()
                                      ? ex.Message//ex.Message + Environment.NewLine + ex.InnerException
                                            //+ Environment.NewLine + ex.StackTrace Constants.ErrorMessages.ERR_02: 
                                      : ex.Message
                    }
                );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
