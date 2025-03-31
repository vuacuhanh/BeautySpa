using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BeautySpa.API.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path}");

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred during request: {ex}");
                throw;  
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation($"Outgoing response: {context.Response.StatusCode} completed in {stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }
}
