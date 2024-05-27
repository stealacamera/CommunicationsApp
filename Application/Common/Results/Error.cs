namespace CommunicationsApp.Domain.Common;

public sealed record Error
{
    public string PropertyName { get; private set; }
    public IList<string> Reasons { get; private set; } = new List<string>();
    public ErrorType Type { get; private set; }

    public Error(
        string propertyName,
        IList<string> reasons,
        ErrorType type)
    {
        PropertyName = propertyName;
        Reasons = reasons; 
        Type = type;
    }

    public Error(
        string propertyName,
        string reason,
        ErrorType type)
    {
        PropertyName = propertyName;
        Reasons.Add(reason);
        Type = type;
    }
}

public enum ErrorType : sbyte
{
    General,
    NotFound,
    Unauthorized,
    UnverifiedUser
}