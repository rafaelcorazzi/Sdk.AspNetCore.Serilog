using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sdk.AspNetCore.Serilog.Test
{
    [Trait("LogSimpleRequestMiddleware", "Invoke")]
    public class LogSimpleRequestMiddlewareTest
    {
        [Fact(DisplayName = "Quando 'Invoke' | Dado que http status code é menor que 500 | Então deve logar em nível de informação")]
        public async Task WhenInvoke_GivenStatusCodeIsLessThan500_ThenLogInformation()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var options = new CorrelationIdOptions();
                var middleware = new LogSimpleRequestMiddleware(async (innerHttpContext) =>
                {
                    await innerHttpContext.Response.WriteAsync("Test Response Body");
                });

                var context = new DefaultHttpContext();

                await middleware.Invoke(context);

                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();

                Assert.Equal(LogEventLevel.Information, item.Level);
            }
        }

        [Fact(DisplayName = "Quando 'Invoke' | Dado lançou exceção | Então deve logar em nível de Erro")]
        public async Task WhenInvoke_GivenThrowException_ThenLogError()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var options = new CorrelationIdOptions();
                var middleware = new LogSimpleRequestMiddleware((innerHttpContext) =>
                {
                    throw new Exception("teste");
                });

                var context = new DefaultHttpContext();

                await Assert.ThrowsAsync<Exception>(async () => await middleware.Invoke(context));

                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();

                Assert.Equal(LogEventLevel.Error, item.Level);
            }
        }

        [Fact(DisplayName = "Quando 'Invoke' | Dado que http status code é maior que 499 | Então deve logar em nível de erro")]
        public async Task WhenInvoke_GivenStatusCodeIsBiggerThan499_ThenLogError()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var options = new CorrelationIdOptions();
                var middleware = new LogSimpleRequestMiddleware(async (innerHttpContext) =>
                {
                    innerHttpContext.Response.StatusCode = 500;
                    await innerHttpContext.Response.WriteAsync("Test Response Body");
                });

                var context = new DefaultHttpContext();

                await middleware.Invoke(context);

                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();

                Assert.Equal(LogEventLevel.Error, item.Level);
            }
        }
    }
}

