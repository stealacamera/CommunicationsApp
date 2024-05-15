using CommunicationsApp.Domain.Common.Enums;

namespace CommunicationsApp.Application.DTOs;

public record Media(string Filename, MediaType Type);