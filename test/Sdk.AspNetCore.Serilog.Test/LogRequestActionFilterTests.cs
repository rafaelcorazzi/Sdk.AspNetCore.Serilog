using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NSubstitute;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sdk.AspNetCore.Serilog.Test
{
    public class LogRequestActionFilterTests
    {
        [Fact(DisplayName = "Quando 'OnActionExecutionAsync' | Dado que http status code é menor que 500 e result é 'StatusCodeResult' | Então log é em nível de informação")]
        public async Task WhenOnActionExecutionAsync_GivenStatusCodeIsLessThan500AndResultIsStatusCodeResult_ThanLogInformation()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var filters = Substitute.For<IList<IFilterMetadata>>();
                var actionArguments = Substitute.For<IDictionary<string, object>>();
                var context = new ActionExecutingContext(GetActionContext(), filters, actionArguments, new object());
                var actionFilter = new LogRequestActionFilter();
                await actionFilter.OnActionExecutionAsync(context, ActionExecutionDelegateStatusCodeResultOkFake);
                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();
                Assert.Equal(LogEventLevel.Information, item.Level);
            }
        }

        [Fact(DisplayName = "Quando 'OnActionExecutionAsync' | Dado que http status code é maior que 500 e result é 'StatusCodeResult' | Então log é em nível de erro")]
        public async Task WhenOnActionExecutionAsync_GivenStatusCodeIsBiggerThan499AndResultIsStatusCodeResult_ThanLogError()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var filters = Substitute.For<IList<IFilterMetadata>>();
                var actionArguments = Substitute.For<IDictionary<string, object>>();
                var context = new ActionExecutingContext(GetActionContext(), filters, actionArguments, new object());
                var actionFilter = new LogRequestActionFilter();
                await actionFilter.OnActionExecutionAsync(context, ActionExecutionDelegateStatusCodeResultInternalServerErrorFake);
                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();
                Assert.Equal(LogEventLevel.Error, item.Level);
            }
        }

        [Fact(DisplayName = "Quando 'OnActionExecutionAsync' | Dado que http status code é menor que 500 e result é 'ObjectResult' | Então log é em nível de informação")]
        public async Task WhenOnActionExecutionAsync_GivenStatusCodeIsLessThan500AndResultIsObjectResult_ThanLogInformation()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var filters = Substitute.For<IList<IFilterMetadata>>();
                var actionArguments = Substitute.For<IDictionary<string, object>>();
                var context = new ActionExecutingContext(GetActionContext(), filters, actionArguments, new object());
                var actionFilter = new LogRequestActionFilter();
                await actionFilter.OnActionExecutionAsync(context, ActionExecutionDelegateObjectResultOkFake);
                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();
                Assert.Equal(LogEventLevel.Information, item.Level);
            }
        }

        [Fact(DisplayName = "Quando 'OnActionExecutionAsync' | Dado que http status code é maior que 499 e result é 'ObjectResult' | Então log é em nível de erro")]
        public async Task WhenOnActionExecutionAsync_GivenStatusCodeIsBiggerThan499AndResultIsObjectResult_ThanLogError()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var filters = Substitute.For<IList<IFilterMetadata>>();
                var actionArguments = Substitute.For<IDictionary<string, object>>();
                var context = new ActionExecutingContext(GetActionContext(), filters, actionArguments, new object());
                var actionFilter = new LogRequestActionFilter();
                await actionFilter.OnActionExecutionAsync(context, ActionExecutionDelegateObjectResultInternalServerErrorFake);
                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();
                Assert.Equal(LogEventLevel.Error, item.Level);
            }
        }

        [Fact(DisplayName = "Quando 'OnActionExecutionAsync' | Dado que execução obteve exceção sem 'handleException' | Então log é em nível de erro")]
        public async Task WhenOnActionExecutionAsync_GivenThrowExceptionWithoutHandleException_ThanLogError()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var filters = Substitute.For<IList<IFilterMetadata>>();
                var actionArguments = Substitute.For<IDictionary<string, object>>();
                var context = new ActionExecutingContext(GetActionContext(), filters, actionArguments, new object());
                var actionFilter = new LogRequestActionFilter();
                await actionFilter.OnActionExecutionAsync(context, ActionExecutionDelegateExceptiontFake);
                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();
                Assert.Equal(LogEventLevel.Error, item.Level);
            }
        }

        [Fact(DisplayName = "Quando 'OnActionExecutionAsync' | Dado que execução obteve exceção com 'handleException' com result Ok com statuscoderesult | Então log é em nível de erro")]
        public async Task WhenOnActionExecutionAsync_GivenThrowExceptionWithHandleExceptionWithOkResultAndStatusCodeResult_ThanLogError()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var filters = Substitute.For<IList<IFilterMetadata>>();
                var actionArguments = Substitute.For<IDictionary<string, object>>();
                var context = new ActionExecutingContext(GetActionContext(), filters, actionArguments, new object());
                var actionFilter = new LogRequestActionFilter(DefaultHandleException);
                await actionFilter.OnActionExecutionAsync(context, ActionExecutionDelegateExceptiontFake);
                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();
                Assert.Equal(LogEventLevel.Error, item.Level);
            }
        }

        [Fact(DisplayName = "Quando 'OnActionExecutionAsync' | Dado que execução obteve exceção com 'handleException' com result 500 e StatusCodeResult | Então log é em nível de erro")]
        public async Task WhenOnActionExecutionAsync_GivenThrowExceptionWithHandleExceptionWithInternalServerErrorAndStatusCodeResult_ThanLogError()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var filters = Substitute.For<IList<IFilterMetadata>>();
                var actionArguments = Substitute.For<IDictionary<string, object>>();
                var context = new ActionExecutingContext(GetActionContext(), filters, actionArguments, new object());
                var actionFilter = new LogRequestActionFilter(InternalServerErrorHandleException);
                await actionFilter.OnActionExecutionAsync(context, ActionExecutionDelegateExceptiontFake);
                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();
                Assert.Equal(LogEventLevel.Error, item.Level);
            }
        }

        [Fact(DisplayName = "Quando 'OnActionExecutionAsync' | Dado que execução obteve exceção com 'handleException' com result Ok com ObjectResult | Então log é em nível de erro")]
        public async Task WhenOnActionExecutionAsync_GivenThrowExceptionWithHandleExceptionWithOkResultAndobjectResult_ThanLogError()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var filters = Substitute.For<IList<IFilterMetadata>>();
                var actionArguments = Substitute.For<IDictionary<string, object>>();
                var context = new ActionExecutingContext(GetActionContext(), filters, actionArguments, new object());
                var actionFilter = new LogRequestActionFilter(DefaultHandleExceptionObjectResult);
                await actionFilter.OnActionExecutionAsync(context, ActionExecutionDelegateExceptiontFake);
                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();
                Assert.Equal(LogEventLevel.Error, item.Level);
            }
        }

        [Fact(DisplayName = "Quando 'OnActionExecutionAsync' | Dado que execução obteve exceção com 'handleException' com result 500 e ObjectResult | Então log é em nível de erro")]
        public async Task WhenOnActionExecutionAsync_GivenThrowExceptionWithHandleExceptionWithInternalServerErrorAndObjectResult_ThanLogError()
        {
            Log.Logger = new LoggerConfiguration()
                                .Enrich.FromLogContext()
                                .WriteTo.TestCorrelator().CreateLogger();

            using (TestCorrelator.CreateContext())
            {
                var filters = Substitute.For<IList<IFilterMetadata>>();
                var actionArguments = Substitute.For<IDictionary<string, object>>();
                var context = new ActionExecutingContext(GetActionContext(), filters, actionArguments, new object());
                var actionFilter = new LogRequestActionFilter(InternalServerErrorHandleExceptionObjectResult);
                await actionFilter.OnActionExecutionAsync(context, ActionExecutionDelegateExceptiontFake);
                var item = TestCorrelator.GetLogEventsFromCurrentContext().FirstOrDefault();
                Assert.Equal(LogEventLevel.Error, item.Level);
            }
        }

        private IActionResult DefaultHandleException(ActionExecutedContext actionExecutingContext) => new OkResult();
        private IActionResult InternalServerErrorHandleException(ActionExecutedContext actionExecutingContext) => new StatusCodeResult(500);
        private IActionResult DefaultHandleExceptionObjectResult(ActionExecutedContext actionExecutingContext) => new ObjectResult(Guid.NewGuid()) { StatusCode = 200 };
        private IActionResult InternalServerErrorHandleExceptionObjectResult(ActionExecutedContext actionExecutingContext) => new ObjectResult(500M) { StatusCode = 500 };

        private async Task<ActionExecutedContext> ActionExecutionDelegateObjectResultOkFake() => await ActionExecutionDelegateObjectResultFake(200);
        private async Task<ActionExecutedContext> ActionExecutionDelegateObjectResultInternalServerErrorFake() => await ActionExecutionDelegateObjectResultFake(500);
        private async Task<ActionExecutedContext> ActionExecutionDelegateStatusCodeResultOkFake() => await ActionExecutionDelegateFake(200);
        private async Task<ActionExecutedContext> ActionExecutionDelegateStatusCodeResultInternalServerErrorFake() => await ActionExecutionDelegateFake(500);

        private Task<ActionExecutedContext> ActionExecutionDelegateObjectResultFake(int statusCode = 200)
        {
            var actionExecutedContext = new ActionExecutedContext(GetActionContext(), Substitute.For<IList<IFilterMetadata>>(), Substitute.For<IDictionary<string, object>>())
            {
                Result = new ObjectResult("teste") { StatusCode = statusCode }
            };

            return Task.FromResult(actionExecutedContext);
        }

        private Task<ActionExecutedContext> ActionExecutionDelegateFake(int statusCode = 200)
        {
            var actionExecutedContext = new ActionExecutedContext(GetActionContext(), Substitute.For<IList<IFilterMetadata>>(), Substitute.For<IDictionary<string, object>>())
            {
                Result = new StatusCodeResult(statusCode)
            };

            return Task.FromResult(actionExecutedContext);
        }

        private Task<ActionExecutedContext> ActionExecutionDelegateExceptiontFake()
        {
            var actionExecutedContext = new ActionExecutedContext(GetActionContext(), Substitute.For<IList<IFilterMetadata>>(), Substitute.For<IDictionary<string, object>>())
            {
                Exception = new Exception("teste")
            };

            return Task.FromResult(actionExecutedContext);
        }

        private ActionContext GetActionContext() => new ActionContext(new DefaultHttpContext(), new Microsoft.AspNetCore.Routing.RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
    }
}
