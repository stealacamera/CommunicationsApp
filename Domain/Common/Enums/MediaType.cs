using Ardalis.SmartEnum;

namespace CommunicationsApp.Domain.Common.Enums;

public sealed class MediaType : SmartEnum<MediaType>
{
    public static MediaType Image = new("Image", 0),
                            Document = new("Document", 1),
                            Video = new("Video", 2);

    private MediaType(string name, int value) : base(name, value)
    {
    }
}