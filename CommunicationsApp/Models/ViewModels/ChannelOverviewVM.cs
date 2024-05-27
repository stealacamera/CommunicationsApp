using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.DTOs.ViewModels;

namespace CommunicationsApp.Web.Models.ViewModels;

public record ChannelOverviewVM(Channel Channel, CursorPaginatedList<int, Message> Messages);
