using System.Net;
using System.Text.Json;
using BlogRealtime.Api.Models;
using BlogRealtime.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BlogRealtime.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        switch (exception)
        {
            case InvalidRequestException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case ResourceNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.StatusCode = StatusCodes.Status404NotFound;
                break;

            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.StatusCode = StatusCodes.Status401Unauthorized;
                break;

            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = StatusCodes.Status400BadRequest;
                response.Errors = validationException.Errors.Select(e => new ValidationError
                {
                    PropertyName = e.PropertyName,
                    ErrorMessage = e.ErrorMessage
                }).ToList();
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusCode = StatusCodes.Status500InternalServerError;
                break;
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsJsonAsync(response);
    }
}
