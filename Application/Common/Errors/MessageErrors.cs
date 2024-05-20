using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;

public static class MessageErrors
{
    public static Error NotFound = BaseErrors.NotFound(nameof(Message));
}
