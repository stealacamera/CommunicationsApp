using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Entities;

public class ChannelRole : BaseStrongEntity
{
    public string Name { get; set; } = null!;
}
