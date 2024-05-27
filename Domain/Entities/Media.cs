using CommunicationsApp.Domain.Common;

namespace CommunicationsApp.Domain.Entities;

public class Media : BaseStrongEntity
{
    public string Filename { get; set; } = null!;
    public int MessageId { get; set; }

    public byte MediaTypeId { get; set; }
}