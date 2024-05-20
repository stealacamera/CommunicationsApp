namespace CommunicationsApp.Domain.Common;

public sealed record Error(string Description, ErrorType Type);

public enum ErrorType : sbyte
{
    General,
    NotFound,
    Unauthorized
}