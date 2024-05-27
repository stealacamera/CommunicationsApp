using CommunicationsApp.Application.DTOs;

namespace CommunicationsApp.Web.Models.ViewModels;

public class ChannelMessagesVM
{
    public ChannelOverviewVM ChannelOverview { get; set; } = null!;
    public ChannelMember CurrentUser { get; set; } = null!;
}
