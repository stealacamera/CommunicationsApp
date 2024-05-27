using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Results.Errors;

public static class BaseErrors
{
    public static Error NotFound(string propertyName, string entityName)
        => new(propertyName, $"{entityName} could not be found", ErrorType.NotFound);
}