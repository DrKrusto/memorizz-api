using Memorizz.Host.Domain.Behaviors;
using Microsoft.AspNetCore.Diagnostics;

namespace Memorizz.Host.Domain.Services;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService problemDetailsService;
    
    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        this.problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";
        
        var errorDetails = exception switch
        {
            ValidationException => (StatusCode: StatusCodes.Status400BadRequest, Detail: exception.Message),
            _ => (StatusCode: StatusCodes.Status500InternalServerError, Detail: exception.Message)
        };
        
        httpContext.Response.StatusCode = errorDetails.StatusCode;

        if (exception is not ValidationException validationException)
            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails =
                {
                    Title = "An error occurred",
                    Status = errorDetails.StatusCode,
                    Detail = errorDetails.Detail,
                    Type = exception.GetType().Name
                },
                Exception = exception
            });
        
        await httpContext.Response.WriteAsJsonAsync(new { validationException.Errors }, cancellationToken);
        return true;
    }
}