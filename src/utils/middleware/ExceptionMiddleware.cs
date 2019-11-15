
using System;
using System.Net;
using System.Threading.Tasks;


using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using stock_dotnet.utils;
using stock_dotnet.utils.Exceptions;
using stock_dotnet.utils.Extensions;

namespace stock_dotnet.utils.middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerManager _logger;
        private IMemoryCache _cache;
        public ExceptionMiddleware(RequestDelegate next, ILoggerManager logger, IMemoryCache cache)
        {
            _logger = logger;
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            ErrorDetails result = exception.HandleException();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.StatusCode;
            return context.Response.WriteAsync(result.ToString());
        }
    }
}