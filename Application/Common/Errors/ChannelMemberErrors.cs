using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Application.Common.Errors;

public static class ChannelMemberErrors
{
    public static Error NotFound = new("Member could not be found");
    public static Error MemberIsOwner = new("The channel's owner cannot also be a member");
    public static Error UserIsNotMemberOfChannel = new("User is not a member of this group");
}
