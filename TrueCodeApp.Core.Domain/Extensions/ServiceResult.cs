using System.Net;

namespace TrueCodeApp.Core.Domain.Extensions;

public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public HttpStatusCode? StatusCode { get; set; }

    public static ServiceResult Success() => new() { IsSuccess = true, StatusCode = HttpStatusCode.OK };

    public static ServiceResult Failure(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new() { IsSuccess = false, ErrorMessage = message, StatusCode = statusCode };
}

public class ServiceResult<T> : ServiceResult
{
    public T? Data { get; set; }

    public static ServiceResult<T> Success(T data) => new() { IsSuccess = true, Data = data, StatusCode = HttpStatusCode.OK };

    public new static ServiceResult<T> Failure(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new() { IsSuccess = false, ErrorMessage = message, StatusCode = statusCode };
}