using Ardalis.SmartEnum;

namespace CommunicationsApp.Application.Common.Enums;

public sealed class ChannelRole : SmartEnum<ChannelRole>
{
    public static readonly ChannelRole Owner = new("Owner", 1);
    public static readonly ChannelRole Member = new("Member", 2);

    private ChannelRole(string name, int value) : base(name, value) { }
}
