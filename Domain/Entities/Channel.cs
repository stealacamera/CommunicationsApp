using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Entities;

public class Channel : BaseSoftDeleteEntity
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public List<User> Members { get; set; } = new List<User>();
}