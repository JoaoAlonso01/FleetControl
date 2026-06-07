namespace FleetControl.Domain.Shared;

public sealed class Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public int StatusCode { get; }
    public bool IsSuccess { get; }

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        StatusCode = 200;
    }

    private Result(string error, int statusCode)
    {
        Error = error;
        IsSuccess = false;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error, int statusCode = 400) => new(error, statusCode);
    public static Result<T> NotFound(string error) => new(error, 404);
}

public sealed class Result
{
    public string? Error { get; }
    public int StatusCode { get; }
    public bool IsSuccess { get; }

    private Result()
    {
        IsSuccess = true;
        StatusCode = 200;
    }

    private Result(string error, int statusCode)
    {
        Error = error;
        IsSuccess = false;
        StatusCode = statusCode;
    }

    public static Result Success() => new();
    public static Result Failure(string error, int statusCode = 400) => new(error, statusCode);
    public static Result NotFound(string error) => new(error, 404);
}
