using Ardalis.SmartEnum.SystemTextJson;
using CommunicationsApp.Domain.Common.Enums;
using System.Text.Json.Serialization;

namespace CommunicationsApp.Application.DTOs;

public record Media
{
    public string Filename { get; set; }

    [JsonConverter(typeof(SmartEnumNameConverter<MediaType, byte>))] 
    public MediaType Type { get; set; }

    public Media(string filename, MediaType type)
    {
        Filename = filename;
        Type = type;
    }
}