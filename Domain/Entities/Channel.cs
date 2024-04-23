using CommunicationsApp.Domain.Common;
using System.Net.Http.Headers;

namespace CommunicationsApp.Domain.Entities;

public class Channel : BaseSoftDeleteEntity<int>
{
    public int OwnerId { get; set; }
    public IEnumerable<User> Members { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}