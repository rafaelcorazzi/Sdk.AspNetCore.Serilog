using Sdk.AspNetCore.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.TestCorrelator;
using System;
using System.Linq;
using Xunit;

namespace Sdk.AspNetCore.Serilog.Test
{
    public class CorrelationIdMiddlewareTests
    {
        [Fact]
        public async void TraceIdentifier_Should_Be_CorrelationId()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var options = new CorrelationIdOptions();
                var middleware = new CorrelationIdMiddleware(async (innerHttpContext) =>
                {
                    Log.Information("Test");

                    await innerHttpContext.Response.WriteAsync("Test Response Body");

                }, Options.Create(options));

                var context = new DefaultHttpContext();

                var correlationId = Guid.NewGuid().ToString();

                context.Request.Headers.Add(options.Header, correlationId);

                await middleware.Invoke(context);

                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();

                var sequenceValue = item.Properties["CorrelationId"] as global::Serilog.Events.SequenceValue;
                Assert.Equal(correlationId, sequenceValue.Elements[0].ToString().Replace("\"", string.Empty));
            }
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_RequestDelegate_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(null, Options.Create(new CorrelationIdOptions())));
        }

        [Fact]
        public void Constructor_Should_Throw_ArgumentNullException_When_Options_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(async (innerHttpContext) =>
            {
                await innerHttpContext.Response.WriteAsync("Test Response Body");
            }, null));
        }

        [Fact]
        public async void Middleware_Should_Add_CorrelationId_In_Response_Header()
        {
            void ping(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Pong");
                });
            }

            var builder = new WebHostBuilder()
                                .Configure(app =>
                                {
                                    app.UseCorrelationId();
                                    app.Map("/ping", ping);
                                });
            var server = new TestServer(builder);

            var client = server.CreateClient();

            var correlationId = Guid.NewGuid().ToString();

            client.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);

            var response = await client.GetAsync("/ping");

            Assert.Equal(correlationId, response.Headers.GetValues("X-Correlation-Id").Single());
        }

        [Fact]
        public async void Middleware_Should_Add_CorrelationId_In_Request_Header_When_None_Is_In_Request()
        {
            void ping(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    Assert.NotEmpty(context.Request.Headers["X-Correlation-Id"]);
                    await context.Response.WriteAsync("Pong");
                });
            }

            var builder = new WebHostBuilder()
                                .Configure(app =>
                                {
                                    app.UseCorrelationId();
                                    app.Map("/ping", ping);
                                });
            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/ping");

            Assert.NotEmpty(response.Headers.GetValues("X-Correlation-Id"));
        }

        [Fact]
        public async void Middleware_Should__Not_Add_CorrelationId_In_Request_Header_When_Disabled_In_Options()
        {
            void ping(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    Assert.Empty(context.Request.Headers["X-Correlation-Id"]);
                    await context.Response.WriteAsync("Pong");
                });
            }

            var builder = new WebHostBuilder()
                                .Configure(app =>
                                {
                                    app.UseCorrelationId(new CorrelationIdOptions
                                    {
                                        CorrelationIdBuilder = null
                                    });
                                    app.Map("/ping", ping);
                                });
            var server = new TestServer(builder);

            var client = server.CreateClient();

            var response = await client.GetAsync("/ping");

            Assert.False(response.Headers.Contains("X-Correlation-Id"));
        }
    }
}
