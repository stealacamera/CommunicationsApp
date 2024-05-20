using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;

public static class ChannelMemberErrors
{
    public static Error NotFound = BaseErrors.NotFound("Member");
    public static Error MemberIsOwner = new("The channel's owner cannot also be a member", ErrorType.General);

    public static Error NotMemberOfChannel = new("User is not a member of this group", ErrorType.Unauthorized);
    public static Error AlreadyMemberOfChannel = new("User is already a member of this group", ErrorType.General);

    public static Error CannotRemoveAllMembers = new("Cannot remove all members from group", ErrorType.General);
}
