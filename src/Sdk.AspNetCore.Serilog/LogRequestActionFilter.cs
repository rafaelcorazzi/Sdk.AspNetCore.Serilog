using Sdk.AspNetCore.Serilog.LogModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace Sdk.AspNetCore.Serilog
{
    public class LogRequestActionFilter : IAsyncActionFilter
    {
        static readonly ILogger Log = global::Serilog.Log.ForContext<LogRequestActionFilter>();
        const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} {@RequestBody} responded {@ResponseBody} {StatusCode} in {Elapsed:0.0000} ms";

        public delegate IActionResult Del(ActionExecutedContext actionExecutingContext);

        private readonly Del _handleException;

        /// <summary>
        /// Contrutor do log filter. para injeção no startup das API's.
        /// </summary>
        /// <param name="handleException">Método para sobrescrever o retorno da requisião em caso de exceção. Geralmente usado em integrações que sempre retornam httpstatus 200 e especificam o erro dentro do response</param>
        public LogRequestActionFilter(Del handleException = null) => _handleException = handleException;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var sw = Stopwatch.StartNew();
            var request = context.ActionArguments;
            var controllerResult = await next();
            sw.Stop();
            if (controllerResult.Exception != null)
                LogException(controllerResult, request, sw.ElapsedMilliseconds);
            else
                LogResult(controllerResult, request, sw.ElapsedMilliseconds);
        }

        private void LogResult(ActionExecutedContext controllerResult, object request, double elapsedMs)
        {
            if (controllerResult.Result is ObjectResult)
            {
                var result = controllerResult.Result as ObjectResult;
                var statusCode = result?.StatusCode;
                var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;
                var log = level == LogEventLevel.Error ? LogForErrorContext(controllerResult.HttpContext) : Log;
                log.Write(level, MessageTemplate, controllerResult.HttpContext.Request.Method, controllerResult.HttpContext.Request.Path, GetLog(request), GetLog(result?.Value), statusCode, elapsedMs);
            }
            else
            {
                var result = controllerResult.Result as StatusCodeResult;
                var statusCode = result?.StatusCode;
                var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;
                var log = level == LogEventLevel.Error ? LogForErrorContext(controllerResult.HttpContext) : Log;
                log.Write(level, MessageTemplate, controllerResult.HttpContext.Request.Method, controllerResult.HttpContext.Request.Path, GetLog(request), null, statusCode, elapsedMs);
            }
        }

        private void LogException(ActionExecutedContext controllerResult, dynamic requestBody, double elapsedMs)
        {
            if (_handleException != null)
            {
                controllerResult.Result = _handleException(controllerResult);

                if (controllerResult.Result is ObjectResult)
                {
                    var result = controllerResult.Result as ObjectResult;
                    LogForErrorContext(controllerResult.HttpContext).Error(controllerResult.Exception, MessageTemplate, controllerResult.HttpContext.Request.Method, controllerResult.HttpContext.Request.Path, requestBody, result, result.StatusCode, elapsedMs);
                }
                else
                {
                    var result = controllerResult.Result as StatusCodeResult;
                    LogForErrorContext(controllerResult.HttpContext).Error(controllerResult.Exception, MessageTemplate, controllerResult.HttpContext.Request.Method, controllerResult.HttpContext.Request.Path, requestBody, null, result.StatusCode, elapsedMs);
                }

                controllerResult.Exception = null;
            }
            else
                LogForErrorContext(controllerResult.HttpContext).Error(controllerResult.Exception, MessageTemplate, controllerResult.HttpContext.Request.Method, controllerResult.HttpContext.Request.Path, requestBody, null, StatusCodes.Status500InternalServerError, elapsedMs);
        }

        static ILogger LogForErrorContext(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = Log
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            if (request.HasFormContentType)
                result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

            return result;
        }

        private object GetLog(object value)
        {
            switch (value)
            {
                case int intValue:
                    return NumberLog.GetLog(intValue);
                case decimal decValue:
                    return NumberLog.GetLog(decValue);
                case string strValue:
                    return StringLog.GetLog(strValue);
                case bool booValue:
                    return BoolLog.GetLog(booValue);
                case Guid guidValue:
                    return GuidLog.GetLog(guidValue);
                default:
                    return value;
            }
        }
    }
}
