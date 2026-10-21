namespace MetricsApp.Metrics;

public record RequestMetric(string Path, string Method, int? StatusCode, long ElapsedMilliseconds, DateTime Utc);