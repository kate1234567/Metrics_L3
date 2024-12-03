using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IO.Swagger.Services;

namespace IO.Swagger.Middlewares
{
    public class MetricsTracking
    {
        private readonly RequestDelegate _next;
        private readonly MetricsService _metricsService;

        public MetricsTracking(RequestDelegate next, MetricsService metricsService)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _metricsService = metricsService ?? throw new ArgumentNullException(nameof(metricsService));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value;

            if (path == "/metrics")
            {
                await _next(httpContext);
                return;
            }

            _metricsService.BeginConnection();
            _metricsService.TrackRequest(httpContext.Request.Path, httpContext.Request.Method);
            _metricsService.TrackCpuUsage();

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                throw; 
            }
            finally
            {
                stopwatch.Stop();

                _metricsService.TrackResponse(httpContext.Response.StatusCode);
                _metricsService.TrackResponseTime(stopwatch.Elapsed);

                _metricsService.EndConnection();
            }
        }
    }
}
