using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Entities;

public class Message : BaseSoftDeleteEntity
{
    public int OwnerId { get; set; }
    public int ChannelId { get; set; }

    public string? Text { get; set; }
    public DateTime CreatedAt { get; set; }
}