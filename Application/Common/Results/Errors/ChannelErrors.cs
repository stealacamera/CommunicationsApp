using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Results.Errors;

public static class ChannelErrors
{
    public static Error NotFound(string propertyName) => BaseErrors.NotFound(propertyName, nameof(Channel));
    public static Error IncorrectNumberOfMembers(string propertyName, int maxNrMembers)
        => new(propertyName, $"A channel can have 2 to {maxNrMembers} members", ErrorType.General);
}
