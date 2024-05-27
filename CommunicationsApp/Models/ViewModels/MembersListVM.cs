using CommunicationsApp.Application.DTOs;

namespace CommunicationsApp.Web.Models.ViewModels;

public class MembersListVM
{
    public ChannelMember ChannelMember { get; set; } = null!;
    public ChannelMember CurrentUser { get; set; } = null!;
    public bool IsGroupChannel { get; set; } = false;
}