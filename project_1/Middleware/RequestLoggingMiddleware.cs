using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace project_1.Middlewares
{
    /// <summary>
    /// Logs HttpMethod, Path, StatusCode and elapsed time for every request.
    /// </summary>
    public sealed class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();

            await _next(context);

            sw.Stop();
            var status = context.Response?.StatusCode;
            _logger.LogInformation("HTTP {Method} {Path} -> {StatusCode} in {Elapsed}ms",
                method, path, status, sw.ElapsedMilliseconds);
        }
    }
}