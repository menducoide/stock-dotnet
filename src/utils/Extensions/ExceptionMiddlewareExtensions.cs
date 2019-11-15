 
using Microsoft.AspNetCore.Builder;
using stock_dotnet.utils.middleware;
 

namespace stock_dotnet.utils.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}