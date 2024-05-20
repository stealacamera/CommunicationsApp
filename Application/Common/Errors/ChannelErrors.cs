using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;

public static class ChannelErrors
{
    public static Error NotFound = BaseErrors.NotFound(nameof(Channel));
    public static Error IncorrectNumberOfMembers(int maxNrMembers) 
        => new($"A channel can have 2 to {maxNrMembers} members", ErrorType.General);
}
