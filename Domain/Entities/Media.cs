using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Entities;

public class Media : BaseStrongEntity
{
    public string Filename { get; set; } = null!;
    public int MediaTypeId { get; set; }
    public int MessageId { get; set; }
}