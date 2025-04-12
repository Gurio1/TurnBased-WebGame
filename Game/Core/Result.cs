namespace Game.Core;

public class Result<T> : ResultWithoutValue
{
    private T? _value;
    
    public T Value
    {
        get => IsSuccess ? _value! : throw new InvalidOperationException("Result is not successful");
        private set => _value = value;
    }

    private Result(bool isSuccess,T? value,Error error) : base(isSuccess,error)
    {
        _value = value;
    }
    public static Result<T> Success(T result) => new(true, result,Error.None);
    public static Result<T> NotFound(string message) => new(false, default, new Error("404",message));
    public static Result<T> Invalid(string message) => new(false, default, new Error("400",message));
    public static Result<T> Failure(string message) => new(false, default, new Error("500",message));
    public static Result<T> CustomError(Error error) => new(false, default, error);

    public Result<TA> AsError<TA>()
    {
        if (IsSuccess)
        {
            throw new InvalidOperationException("Cannot rethrow successful result");
        }

        return Result<TA>.CustomError(Error);
    }

};

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(String.Empty, String.Empty);
}
