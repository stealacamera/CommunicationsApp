using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Results.Errors;



public static class UserErrors
{
    public static Error NotFound(string propertyName) => BaseErrors.NotFound(propertyName, nameof(User));
    public static Error Unauthorized(string propertyName) => new(propertyName, "User is not authorized to perform this action", ErrorType.Unauthorized);
}
