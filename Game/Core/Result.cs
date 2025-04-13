namespace Game.Core;

public class Result<T> : ResultWithoutValue
{
    private T? value;

    public T Value
    {
        get => IsSuccess ? value! : throw new InvalidOperationException("Result is not successful");
        private set => this.value = value;
    }

    private Result(bool isSuccess,T? value,CustomError error) : base(isSuccess,error) => this.value = value;
    
#pragma warning disable CA1000
    public static Result<T> Success(T result) => new(true, result,Core.CustomError.None);
    public static Result<T> NotFound(string message) => new(false, default, new CustomError("404",message));
    public static Result<T> Invalid(string message) => new(false, default, new CustomError("400",message));
    public static Result<T> Failure(string message) => new(false, default, new CustomError("500",message));
    public static Result<T> CustomError(CustomError customError) => new(false, default, customError);
#pragma warning restore CA1000

    public Result<TA> AsError<TA>()
    {
        if (IsSuccess)
        {
            throw new InvalidOperationException("Cannot rethrow successful result");
        }

        return Result<TA>.CustomError(base.Error);
    }

};

public sealed record CustomError(string Code, string Description)
{
    public static readonly CustomError None = new(string.Empty, string.Empty);
}
