using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Results.Errors;

public static class ChannelMemberErrors
{
    public static Error NotFound(string propertyName) => BaseErrors.NotFound(propertyName, "Member");
    public static Error MemberIsOwner(string propertyName) => new(propertyName, "The channel's owner cannot also be a member", ErrorType.General);

    public static Error NotMemberOfChannel(string propertyName) => new(propertyName, "User is not a member of this group", ErrorType.Unauthorized);
    public static Error AlreadyMemberOfChannel(string propertyName) => new(propertyName, "User is already a member of this group", ErrorType.General);

    public static Error CannotRemoveAllMembers(string propertyName) => new(propertyName, "Cannot remove all members from group", ErrorType.General);
}
