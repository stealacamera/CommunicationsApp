using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Events;

public record UserAdded : BaseEvent
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
}
