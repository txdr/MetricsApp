using MetricsApp.Metrics;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace MetricsApp.Middleware
{
    public class RequestTimingMiddleware
    {

        // Class variables
        private readonly RequestDelegate _next;
        private static readonly ConcurrentQueue<RequestMetric> _lastMetrics = new();
        private const int MaxItems = 20;

        // Constructor
        public RequestTimingMiddleware(RequestDelegate next) => _next = next;

        // Asynchronous Task
        public async Task InvokeAsync(HttpContext context)
        {
            // Make a stopwatch
            var sw = Stopwatch.StartNew();
            try
            {
                await _next(context);
            } finally
            {
                sw.Stop();
                var metric = new RequestMetric(
                    Path: context.Request.Path.ToString(),
                    Method: context.Request.Method,
                    StatusCode: context.Response?.StatusCode,
                    ElapsedMilliseconds: sw.ElapsedMilliseconds,
                    Utc: DateTime.UtcNow);

                Console.WriteLine($"[METRIC] {metric.Method} {metric.Path} -> {metric.StatusCode} in {metric.ElapsedMilliseconds}");
                _lastMetrics.Enqueue(metric);
                while (_lastMetrics.Count > MaxItems && _lastMetrics.TryDequeue(out _))
                {

                }
            }
        }

        // Helper Method for Program.cs
        public static IReadOnlyList<RequestMetric> Snapshot()
            => _lastMetrics.Reverse().ToList();

    }
}
