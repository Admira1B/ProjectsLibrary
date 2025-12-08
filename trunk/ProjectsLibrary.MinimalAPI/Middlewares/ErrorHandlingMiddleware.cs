using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ProjectsLibrary.MinimalAPI.Middlewares {
    public class ErrorHandlingMiddleware(RequestDelegate next) {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context) {
            try {
                await _next(context);
            } catch (Exception ex) {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception) {
            var (status, message, detail) = exception switch {
                JwtOptionsNotConfiguratedException ex =>
                    (HttpStatusCode.InternalServerError, ex.Message, ex.Details),

                EntityNotFoundException ex =>
                    (HttpStatusCode.NotFound, ex.Message, ex.Details),

                EntityCollectionModificationException ex =>
                    (HttpStatusCode.BadRequest, ex.Message, ex.Details),

                PropertyNotFoundException ex =>
                    (HttpStatusCode.BadRequest, ex.Message, ex.Details),

                NullOrEmptyFieldNameExсeption ex =>
                    (HttpStatusCode.BadRequest, ex.Message, ex.Details),

                OperationCanceledException =>
                    (HttpStatusCode.ServiceUnavailable, "Request was cancelled", null),

                UnauthorizedAccessException =>
                    (HttpStatusCode.Unauthorized, "Access denied", null),

                _ => HandleUnknownException(context, exception)
            };

            await WriteProblemDetailsAsync(context, status, message, detail);
        }

        private static (HttpStatusCode status, string message, string? detail) HandleUnknownException(HttpContext context, Exception exception) {
            var isDevelopment = context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true;

            if (!isDevelopment) {
                return (HttpStatusCode.InternalServerError,
                       "An internal server error occurred.",
                       null);
            }

            return (HttpStatusCode.InternalServerError,
                   exception.Message,
                   exception.StackTrace);
        }

        private static async Task WriteProblemDetailsAsync(HttpContext context, HttpStatusCode status, string message, string? detail) {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)status;

            var problemDetails = new ProblemDetails {
                Status = (int)status,
                Title = message,
                Detail = detail ?? "No detail info",
                Instance = context.Request.Path,
                Type = GetProblemType(status)
            };

            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
        }

        private static string GetProblemType(HttpStatusCode status) {
            return status switch {
                HttpStatusCode.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                HttpStatusCode.Unauthorized => "https://tools.ietf.org/html/rfc7235#section-3.1",
                HttpStatusCode.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                HttpStatusCode.InternalServerError => "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };
        }
    }
}