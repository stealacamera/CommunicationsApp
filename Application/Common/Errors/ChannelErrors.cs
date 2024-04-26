using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;

public static class ChannelErrors
{
    public static Error NotFound => new("Channel entity could not be found");
    public static Error IncorrectNumberOfMembers(int maxNrMembers) 
        => new($"A channel can have 2 to {maxNrMembers} members");
}
