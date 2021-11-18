using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Sdk.AspNetCore.Serilog
{
    [ExcludeFromCodeCoverage]
    public static class LogRequestExtensions
    {
        public static IApplicationBuilder UseSimpleLogRequest(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            return app.UseMiddleware<LogSimpleRequestMiddleware>();
        }

        public static MvcOptions AddLogRequestFilter(this MvcOptions op)
        {
            op.Filters.Add<LogRequestActionFilter>();
            return op;
        }
    }
}
