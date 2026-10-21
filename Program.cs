using MetricsApp.Middleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 1. Centralized error handler
app.UseExceptionHandler("/error");

// 2. Timing middleware handler
app.UseMiddleware<RequestTimingMiddleware>();

// 3. Routing and endpoints
app.MapGet("/", () => "Hello from Metrics App!");

// 4. Route that throws error
app.MapGet("/boom", () =>
{
    throw new InvalidOperationException("BOOM! Testing the error handler.");
});

// 5. Metrics endpoint to return last 20 metrics
app.MapGet("/metrics", () =>
{
    var metrics = RequestTimingMiddleware.Snapshot();
    return Results.Json(metrics, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
});

// 6. Error endpoint for exception handler
app.Map("/error", (HttpContext ctx) =>
{
    return Results.Problem(
        title: "An error occured while processing your request.",
        detail: "Something went wrong.",
        statusCode: StatusCodes.Status500InternalServerError);
});

app.Run();
