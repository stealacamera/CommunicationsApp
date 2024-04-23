using Microsoft.AspNetCore.Identity;

namespace CommunicationsApp.Application.Common.Exceptions;

public class UserValidationException : AppException
{
    public readonly IEnumerable<IdentityError> Errors;

    public UserValidationException(IEnumerable<IdentityError> errors) 
        : base("Validation errors occurred") 
        => Errors = errors;
}
