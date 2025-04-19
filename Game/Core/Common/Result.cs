namespace Game.Core.Common;

public class Result<T> : ResultWithoutValue
{
    private T? value;
    
    private Result(bool isSuccess, T? value, CustomError error) : base(isSuccess, error) => this.value = value;
    
    public T Value
    {
        get => IsSuccess ? value! : throw new InvalidOperationException("Result is not successful");
        private set => this.value = value;
    }
    
    public Result<TA> AsError<TA>() => Result<TA>.CustomError(Error);
    
#pragma warning disable CA1000
    public static Result<T> Success(T result) => new(true, result, Common.CustomError.None);
    public static Result<T> NotFound(string message) => new(false, default, new CustomError("404", message));
    public static Result<T> Invalid(string message) => new(false, default, new CustomError("400", message));
    public static Result<T> Failure(string message) => new(false, default, new CustomError("500", message));
    public static Result<T> CustomError(CustomError customError) => new(false, default, customError);
#pragma warning restore CA1000
}

public sealed record CustomError(string Code, string Description)
{
    public static readonly CustomError None = new(string.Empty, string.Empty);
}
