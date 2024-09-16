using Microsoft.AspNetCore.Mvc;

namespace Memorizz.Host.Domain.Models;

public class RequestResult<T>
{
    private readonly T? data;
    private readonly string? message;
    private readonly bool success;
    private readonly int statusCode;
    
    private RequestResult(T? data, string? message, bool success, int statusCode)
    {
        this.data = data;
        this.message = message;
        this.success = success;
        this.statusCode = statusCode;
    }

    public bool IsSuccess(out T? result) => (result = data) != null && success;
    
    public bool IsFailure(out string? result) => (result = message) != null && !success;

    public ActionResult<TResult> ToActionResult<TResult>(Func<T, TResult>? mapping = null) => success switch
    {
        true => new ObjectResult(mapping == null || data == null ? data : mapping.Invoke(data)) { StatusCode = statusCode },
        false => new ObjectResult(new { errorDetail = message }) { StatusCode = statusCode }
    };
    
    public static RequestResult<T> Success(T data, int statusCode = StatusCodes.Status200OK)
        => new RequestResult<T>(data, null, true, statusCode);
    
    public static RequestResult<T> NotFound(string message = "Not found")
        => new RequestResult<T>(default, message, false, StatusCodes.Status404NotFound);
    
    public static RequestResult<T> Forbidden(string message = "Access denied")
        => new RequestResult<T>(default, message, false, StatusCodes.Status403Forbidden);

    public static RequestResult<T> Error(string message, int statusCode = StatusCodes.Status500InternalServerError)
        => new RequestResult<T>(default, message, false, statusCode);
}