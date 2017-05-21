using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Neutrino.Core.Diagnostics.Exceptions;

namespace Neutrino.Core.Diagnostics
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DiagnosticSource _diagnosticSource;
        private readonly ILogger _logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, DiagnosticSource diagnosticSource, ILoggerFactory loggerFactory)
        {
            _next = next;
            _diagnosticSource = diagnosticSource;
            _logger = loggerFactory.CreateLogger<ExceptionHandlerMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext, IHostingEnvironment environment)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                if (httpContext.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, the error handler will not be executed.");
                    throw;
                }

                var bibliothecaException = exception as NeutrinoException;
                if (bibliothecaException != null)
                {
                    httpContext.Response.StatusCode = (int)bibliothecaException.StatusCode;
                }
                else
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                httpContext.Response.OnStarting(ClearCacheHeaders, httpContext.Response);

                AddExceptionToLogger(exception);

                string result = CreateResult(exception, environment);
                await httpContext.Response.WriteAsync(result);

                if (_diagnosticSource.IsEnabled("Microsoft.AspNetCore.Diagnostics.HandledException"))
                {
                    _diagnosticSource.Write("Microsoft.AspNetCore.Diagnostics.HandledException",
                        new
                        {
                            httpContext = httpContext,
                            exception = exception
                        });
                }

                return;
            }
        }

        private Task ClearCacheHeaders(object state)
        {
            var response = (HttpResponse)state;
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);
            return Task.FromResult(0);
        }

        private void AddExceptionToLogger(Exception exception)
        {
            try
            {
                _logger.LogError($"Exception '{exception.GetType().FullName}' occurs.");
                _logger.LogError($"Exception message: {exception.Message}");
                _logger.LogError($"Exception stack trace {exception.StackTrace}");
                
                if(exception.InnerException != null) 
                {
                    _logger.LogError($"Inner exception '{exception.GetType().FullName}' occurs.");
                    _logger.LogError($"Inner exception message: {exception.Message}");
                    _logger.LogError($"Inner exception stack trace {exception.StackTrace}");
                }
            }
            catch { }
        }

        private string CreateResult(Exception exception, IHostingEnvironment environment)
        {
            if (environment.IsProduction())
            {
                return JsonConvert.SerializeObject(new { type = exception.GetType().Name });
            }

            return JsonConvert.SerializeObject(new { type = exception.GetType(), message = exception.Message, stackTrace = exception.StackTrace });
        }
    }
}