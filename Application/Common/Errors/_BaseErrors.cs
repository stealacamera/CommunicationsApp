using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;

public static class BaseErrors
{
    public static Error NotFound(string entityName) 
        => new($"{entityName} could not be found", ErrorType.NotFound);
}