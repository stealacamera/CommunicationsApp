namespace CommunicationsApp.Domain.Common;

public class Result
{
    public bool Succeded => Error == null;
    public bool Failed => !Succeded;
    public Error Error { get; } = null;

    public static Result Success() => new Result();
    public static Result Fail(Error error) => new Result(error);

    protected Result() {}

    protected Result(Error error)
        => Error = error;

    public static implicit operator Result(Error error) => Fail(error);
}

public class Result<T> : Result
{
    public T Value { get; } = default;

    public static Result Success(T value) => new Result<T>(value);

    public static implicit operator Result<T>(Error error) => new Result<T>(error);
    public static implicit operator Result<T>(T value) => (Result<T>)Success(value);

    protected Result(Error error) : base(error) {}

    private Result(T value) : base() {
        if (typeof(T) == typeof(Error))
            throw new ArgumentException();

        Value = value;
    }
}