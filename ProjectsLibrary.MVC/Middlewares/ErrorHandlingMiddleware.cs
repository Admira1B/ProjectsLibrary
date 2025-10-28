using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ProjectsLibrary.API.Middlewares
{
    public class ErrorHandlingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            string message;
            string? detail = null;

            switch (exception)
            {
                case JwtOptionsNotConfiguratedException optionsNotConfiguratedException:
                    status = HttpStatusCode.InternalServerError;
                    message = optionsNotConfiguratedException.Message;
                    detail = optionsNotConfiguratedException.Details;
                    break;
                case EntityNotFoundException notFoundException:
                    status = HttpStatusCode.NotFound;
                    message = notFoundException.Message;
                    detail = notFoundException.Details;
                    break;
                case EntityCollectionModificationException cannotAddEntityToCollectionException:
                    status = HttpStatusCode.BadRequest;
                    message = cannotAddEntityToCollectionException.Message;
                    detail = cannotAddEntityToCollectionException.Details;
                    break;
                case PropertyNotFoundException propertyNotFoundException:
                    status = HttpStatusCode.BadRequest;
                    message = propertyNotFoundException.Message;
                    detail = propertyNotFoundException.Details;
                    break;
                case NullOrEmptyFieldNameExсeption nullOrEmptyFieldNameException:
                    status = HttpStatusCode.BadRequest;
                    message = nullOrEmptyFieldNameException.Message;
                    detail = nullOrEmptyFieldNameException.Details;
                    break;
                default:
                    status = HttpStatusCode.InternalServerError;
                    message = exception.Message;
                    break;
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)status;

            var problemDetails = new ProblemDetails
            {
                Status = context.Response.StatusCode,
                Title = message,
                Detail = detail ?? "No detail info",
                Instance = context.Request.Path
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(problemDetails, options)
            );
        }
    }
}
