using System.Net;
using Microsoft.AspNetCore.Mvc;
using ShippingRates.Domain.Exceptions;
using ShippingRates.Persistence.Exceptions;

namespace ShippingRates.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "Se produjo un error al procesar la solicitud.");

        var (statusCode, title) = exception switch
        {
            DomainValidationException => ((int)HttpStatusCode.BadRequest, exception.Message),
            ResourceNotFoundException => ((int)HttpStatusCode.NotFound, exception.Message),
            PersistenceConflictException => ((int)HttpStatusCode.Conflict, exception.Message),
            TransientPersistenceException => ((int)HttpStatusCode.ServiceUnavailable, exception.Message),
            PersistenceException => ((int)HttpStatusCode.InternalServerError, exception.Message),
            InvalidOperationException => ((int)HttpStatusCode.InternalServerError, exception.Message),
            _ => ((int)HttpStatusCode.InternalServerError, "No fue posible procesar la solicitud en este momento.")
        };

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
