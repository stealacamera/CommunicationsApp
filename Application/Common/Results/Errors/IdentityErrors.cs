using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Results.Errors;

public static class IdentityErrors
{
    public static Error UnverifiedEmail(string propertyName) => new(propertyName, "User's email is unverified", ErrorType.General);
    public static Error ExternalLoginError(string propertyName) => new(propertyName, "There was an error from external login", ErrorType.UnverifiedUser);
}
