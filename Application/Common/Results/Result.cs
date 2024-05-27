using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace CommunicationsApp.Domain.Common;

public class Result
{
    public IList<Error> Errors { get; } = new List<Error>();
    public bool Succeded => Errors == null || Errors.Count == 0;
    public bool Failed => !Succeded;

    public bool ContainsErrorType(ErrorType type)
    {
        if (Succeded)
            throw new InvalidOperationException("Cannot check successful result");

        return Errors.Any(error => error.Type == type);
    }

    public static Result Success() => new Result();
    public static Result Fail(params Error[] errors) => new Result(errors);
    public static Result Fail(IList<Error> errors) => new Result(errors);
    public static Result Fail(ValidationResult validationResult) => new Result(validationResult);
    public static Result Fail(IdentityResult result) => new Result(result);

    public static implicit operator Result(Error error) => Fail(error);
    public static implicit operator Result(Error[] errors) => Fail(errors);
    public static implicit operator Result(ValidationResult validationResult) => Fail(validationResult);
    public static implicit operator Result(IdentityResult result) => Fail(result);

    protected Result() { }

    protected Result(params Error[] errors)
        => Errors = errors;

    protected Result(IList<Error> errors)
        => Errors = errors;

    protected Result(ValidationResult validationResult)
    {
        if (validationResult.IsValid)
            throw new ArgumentException("Cannot create a failed result from a successful validation");

        foreach (var error in validationResult.Errors)
            Errors.Add(new Error(error.PropertyName, error.ErrorMessage, ErrorType.General));
    }

    protected Result(IdentityResult errors)
    {
        if (errors.Succeeded)
            throw new ArgumentException("Cannot create a failed result from a successful result");

        var labelledErrors = new Dictionary<string, Error>();

        foreach (var error in errors.Errors)
        {
            string errorProperty = GetErrorProperty(error.Code);

            if (labelledErrors.ContainsKey(errorProperty))
                labelledErrors[errorProperty].Reasons.Add(error.Description);
            else
                labelledErrors.Add(
                    errorProperty,
                    new Error(errorProperty, error.Description, ErrorType.General));
        }

        Errors = labelledErrors.Values.ToList();
    }

    private static string GetErrorProperty(string code)
    {
        if (code.Contains("email", StringComparison.OrdinalIgnoreCase))
            return "email";
        else if (code.Contains("username", StringComparison.OrdinalIgnoreCase))
            return "username";
        else if (code.Contains("password", StringComparison.OrdinalIgnoreCase))
            return "password";
        else
            return "";
    }
}

public class Result<T> : Result
{
    public T Value { get; } = default;

    public static Result<T> Success(T value) => new Result<T>(value);
    public static new Result<T> Fail(IList<Error> errors) => new Result<T>(errors);

    public static implicit operator Result<T>(Error error) => new Result<T>(error);
    public static implicit operator Result<T>(Error[] errors) => new Result<T>(errors);
    public static implicit operator Result<T>(ValidationResult validationResult) => new Result<T>(validationResult);
    public static implicit operator Result<T>(IdentityResult result) => new Result<T>(result);
    public static implicit operator Result<T>(T value) => Success(value);

    protected Result(Error error) : base(error) { }
    protected Result(params Error[] errors) : base(errors) { }
    protected Result(IList<Error> errors) : base(errors) { }
    protected Result(ValidationResult validationResult) : base(validationResult) { }
    protected Result(IdentityResult result) : base(result) { }

    private Result(T value) : base() {
        if (typeof(T) == typeof(Error))
            throw new ArgumentException();

        Value = value;
    }
}