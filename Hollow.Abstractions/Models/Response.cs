namespace Hollow.Abstractions.Models;

public struct Response<T>(bool isSuccess, string message = "")
{
    public T Data { get; set; }
    public readonly bool IsSuccess = isSuccess;
    public readonly string Message = message;
}