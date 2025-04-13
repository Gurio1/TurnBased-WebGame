namespace Game.Core;

public class ResultWithoutValue
{
    private readonly CustomError error;
    public CustomError Error => !IsSuccess ? error : throw new InvalidOperationException("Result is successful");
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    protected ResultWithoutValue(bool isSuccess, CustomError error)
    {
        if (isSuccess && error != CustomError.None ||
            !isSuccess && error != CustomError.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        this.error = error;
    }

    public static ResultWithoutValue Success() => new ResultWithoutValue(true, CustomError.None);
    public static ResultWithoutValue Failure(CustomError customError) => new ResultWithoutValue(false, customError);


}
