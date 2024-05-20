using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;



public static class UserErrors
{
    public static Error NotFound = BaseErrors.NotFound(nameof(User));
    public static Error Unauthorized => new("User is not authorized to perform this action", ErrorType.Unauthorized);
}
