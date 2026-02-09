using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MetaHammer.Domain.Exceptions;
using System.Net;

namespace MetaHammer.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        if (exception is DomainException)
        {
            problemDetails.Title = "Domain Error";
            problemDetails.Status = (int)HttpStatusCode.BadRequest;
            problemDetails.Detail = exception.Message;
            problemDetails.Type = "https://metahammer.com/errors/domain-error";
        }
        else
        {
            problemDetails.Title = "Server Error";
            problemDetails.Status = (int)HttpStatusCode.InternalServerError;
            problemDetails.Detail = "An unexpected error occurred. Please try again later.";
            problemDetails.Type = "https://metahammer.com/errors/server-error";
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
