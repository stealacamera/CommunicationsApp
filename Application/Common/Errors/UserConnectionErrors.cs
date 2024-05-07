using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;

public class UserConnectionErrors
{
    public static Error AlreadyExists = new Error("This user's connection is already saved");
}
