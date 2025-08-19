namespace GPS.ApiGateway.Middleware
{
    public class RequestLoggingMiddleware
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
            var startTime = DateTime.UtcNow;
            var requestId = Guid.NewGuid().ToString();

            // Log da requisição de entrada
            _logger.LogInformation(
                "Gateway Request {RequestId}: {Method} {Path} from {RemoteIpAddress} at {StartTime}",
                requestId,
                context.Request.Method,
                context.Request.Path + context.Request.QueryString,
                context.Connection.RemoteIpAddress,
                startTime);

            await _next(context);

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            // Log da resposta
            _logger.LogInformation(
                "Gateway Response {RequestId}: {StatusCode} in {Duration}ms",
                requestId,
                context.Response.StatusCode,
                duration.TotalMilliseconds);
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
