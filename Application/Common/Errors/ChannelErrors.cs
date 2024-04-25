using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;

public static class ChannelErrors
{
    public static Error NotFound => new("Channel entity could not be found");
}
