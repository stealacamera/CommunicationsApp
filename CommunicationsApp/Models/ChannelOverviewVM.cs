using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.DTOs.ViewModels;

namespace CommunicationsApp.Web.Models;

public record ChannelOverviewVM(Channel Channel, CursorPaginatedList<int, Message> Messages);
