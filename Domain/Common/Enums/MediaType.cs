using Ardalis.SmartEnum;

namespace CommunicationsApp.Domain.Common.Enums;

public sealed class MediaType : SmartEnum<MediaType, byte>
{
    public static MediaType Image = new("Image", 1),
                            Document = new("Document", 2),
                            Video = new("Video", 3);

    private MediaType(string name, byte value) : base(name, value) { }
}