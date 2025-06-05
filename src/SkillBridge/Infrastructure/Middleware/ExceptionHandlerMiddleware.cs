using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkillBridge.Infrastructure.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Middleware;

/// <summary>
/// Middleware to handle exceptions globally and return appropriate HTTP status codes.
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var problemDetails = exception switch
        {
            EntityNotFoundException ex => new ProblemDetails
            {
                Title = "Entity Not Found",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.NotFound,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },
            BusinessRuleValidationException ex => new ProblemDetails
            {
                Title = "Business Rule Validation Failed",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },
            AuthenticationException ex => new ProblemDetails
            {
                Title = "Authentication Failed",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.Unauthorized,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            },
            ExternalServiceException ex => new ProblemDetails
            {
                Title = "External Service Error",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadGateway,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.3"
            },
            ArgumentException ex => new ProblemDetails
            {
                Title = "Invalid Argument",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },            // ArgumentNullException is a subclass of ArgumentException, so we don't need a separate handler
            InvalidOperationException ex => new ProblemDetails
            {
                Title = "Invalid Operation",
                Detail = ex.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },
            _ => new ProblemDetails
            {
                Title = "An error occurred",
                Detail = "An unexpected error occurred. Please try again later.",
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            }
        };

        response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(problemDetails, jsonOptions);
        await response.WriteAsync(json);
    }
}
