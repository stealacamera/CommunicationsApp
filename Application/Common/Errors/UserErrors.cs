using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;



public static class UserErrors
{
    public static Error NotFound => new("User entity could not be found");
    public static Error Unauthorized => new("User is not authorized to perform this action");
}
