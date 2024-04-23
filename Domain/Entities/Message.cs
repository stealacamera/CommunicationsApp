using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Entities;

public class Message : BaseSoftDeleteEntity<int>
{
    public int OwnerId { get; set; }
    public int ChannelId { get; set; }
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}


// TODO multimedia