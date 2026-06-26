using System.Text.Json;
using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.API.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString();

            logger.LogError(ex,
                "Unhandled exception: {Message}. CorrelationId: {CorrelationId}",
                ex.Message,
                correlationId);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString();
    
        var problemDetails = exception switch
        {
            ValidationException ex => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Error",
                Detail = "One or more validation errors occurred.",
                Extensions =
                {
                    ["errors"] = ex.Errors,
                    ["correlationId"] = correlationId
                }
            },
    
            NotFoundException ex => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = ex.Message,
                Extensions = { ["correlationId"] = correlationId }
            },
    
            DomainException ex => new ProblemDetails
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Business Rule Violation",
                Detail = ex.Message,
                Extensions = { ["correlationId"] = correlationId }
            },
    
            ConcurrencyException ex => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = ex.Message,
                Extensions = { ["correlationId"] = correlationId }
            },
    
            DbUpdateConcurrencyException => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = "The resource was modified by another user. Please refresh and try again.",
                Extensions = { ["correlationId"] = correlationId }
            },
    
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred.",
                Extensions = { ["correlationId"] = correlationId }
            }
        };
    
        context.Response.StatusCode = problemDetails.Status!.Value;
        context.Response.ContentType = "application/problem+json";
    
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}