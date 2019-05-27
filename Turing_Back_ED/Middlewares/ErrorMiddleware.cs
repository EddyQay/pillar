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
    public class ErrorMiddleware
    {
        private readonly RequestDelegate next;
        readonly ILogger<ErrorMiddleware> logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> _logger)
        {
            this.next = next;
            logger = _logger;
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

            //if (ex is NotFoundException) code = HttpStatusCode.NotFound;
            //else if (ex is MyUnauthorizedException) code = HttpStatusCode.Unauthorized;
            //else if (ex is MyException) code = HttpStatusCode.BadRequest;


            var result = JsonConvert.SerializeObject(new ErrorRequestModel {
                Status = (int)HttpStatusCode.InternalServerError,
                Code = ex.HResult.ToString(),
                Message = ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace //Constants.InternalServerErrorMessage
            });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
