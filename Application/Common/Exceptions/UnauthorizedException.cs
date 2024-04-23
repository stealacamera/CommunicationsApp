namespace CommunicationsApp.Application.Common.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException() : base("User is unauthorized to perform this action")
    {
    }
}
