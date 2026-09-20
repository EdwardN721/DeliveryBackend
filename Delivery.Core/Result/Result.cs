namespace Delivery.Core.Result;

public class Result
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public ErrorResult Error { get; init; } = ErrorResult.None;

    public static Result Success()
    {
        return new Result { IsSuccess = true, Error = ErrorResult.None }; 
    }

    public static Result Failure(ErrorResult error)
    {
        return new Result { IsSuccess = false, Error = error };
    }
}

public class Result<T> : Result
{
    public T? Value {get; init;} 
    
    public static Result<T> Success(T value)
    {
        return new Result<T> { IsSuccess = true, Error = ErrorResult.None, Value = value };
    }

    public static new Result<T> Failure(ErrorResult error)
    {
        return new Result<T> { IsSuccess = false, Error = error, Value = default };
    }
}

public record ErrorResult (string Code, string Description)
{
    public static readonly ErrorResult None = new (string.Empty, string.Empty);
}