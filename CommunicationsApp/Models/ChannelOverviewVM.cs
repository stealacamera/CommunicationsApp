using CommunicationsApp.Application.DTOs;

namespace CommunicationsApp.Web.Models;

public record ChannelOverviewVM(Channel Channel, IList<Message> Messages);
