using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Cookwi.Common.Exceptions;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Cookwi.Common.Models;

namespace Cookwi.Api.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<ErrorHandlerMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (HttpException e)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)e.StatusCode;
                await response.WriteAsync(JsonConvert.SerializeObject(e.HttpError));
            }
            catch (Exception e)
            {
                logger.LogCritical(e, "Internal server error returned");
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var result = JsonConvert.SerializeObject(new HttpError("An unexpected error has occured, please contact our support"));
                await response.WriteAsync(result);
            }
        }
    }
}