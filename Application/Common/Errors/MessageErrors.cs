using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;

public static class MessageErrors
{
    public static Error NotFound => new("Message could not be found");
}
