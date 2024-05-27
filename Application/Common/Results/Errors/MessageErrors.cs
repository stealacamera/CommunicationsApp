using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Results.Errors;

public static class MessageErrors
{
    public static Error NotFound(string propertyName) => BaseErrors.NotFound(propertyName, nameof(Message));
}
