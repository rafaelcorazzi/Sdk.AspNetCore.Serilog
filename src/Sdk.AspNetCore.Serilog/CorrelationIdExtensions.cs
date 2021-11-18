using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Sdk.AspNetCore.Serilog
{
    [ExcludeFromCodeCoverage]
    public static class CorrelationIdExtensions
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<CorrelationIdMiddleware>();
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app, string header)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseCorrelationId(new CorrelationIdOptions
            {
                Header = header
            });
        }

        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app, CorrelationIdOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<CorrelationIdMiddleware>(Options.Create(options));
        }
    }
}
