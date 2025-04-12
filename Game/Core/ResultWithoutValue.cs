namespace Game.Core;

public class ResultWithoutValue 
{
    private Error _error;
    public Error Error => !IsSuccess ? _error : throw new InvalidOperationException("Result is successful");
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    protected ResultWithoutValue(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || 
            !isSuccess && error != Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        
        IsSuccess = isSuccess;
        _error = error;
    }

    public static ResultWithoutValue Success() => new ResultWithoutValue(true, Error.None);
    public static ResultWithoutValue Failure(Error error) => new ResultWithoutValue(false, error);
    

}