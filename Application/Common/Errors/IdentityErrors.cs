using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;

public static class IdentityErrors
{
    public static Error UnverifiedEmail => new("User's email is unverified", ErrorType.General);
    public static Error ExternalLoginError => new("There was an error from external login", ErrorType.General);
}
