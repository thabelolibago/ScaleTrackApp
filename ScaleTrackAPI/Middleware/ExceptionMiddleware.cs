using System.Net;
using System.Text.Json;
using ScaleTrackAPI.Errors;

namespace ScaleTrackAPI.Middleware
{
    /// <summary>
    /// Middleware for global exception handling.
    /// Converts exceptions into consistent API error responses.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                var errorResponse = new
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = ErrorMessages.Get("UnexpectedError")
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
        }
    }
}
