using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace EasyDapr.Core.Exceptions
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UserFriendlyException ex)
            {
                await HandleUserFriendlyExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleGenericExceptionAsync(context, ex);
            }
        }

        private Task HandleUserFriendlyExceptionAsync(HttpContext context, UserFriendlyException ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex.StatusCode;

            var response = new
            {
                data = null as object,
                status = ex.StatusCode,
                code = "Failure",
                errorMessage = ex.Message,
                validErrors = null as object
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                data = null as object,
                status = context.Response.StatusCode,
                code = "Failure",
                errorMessage = "An unexpected error occurred. Please try again later（2）.",
                validErrors = null as object
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
