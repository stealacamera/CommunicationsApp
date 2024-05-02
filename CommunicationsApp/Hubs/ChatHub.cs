using CommunicationsApp.Application.DTOs;
using Microsoft.AspNetCore.SignalR;

namespace CommunicationsApp.Web.Hubs;

public sealed class ChatHub : Hub
{
    public async Task SendMessage(Message message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }

    // call to hub after creating channel? how?

    //create group with the code from db

    //public async Task SendMessageToChannel(string channelCode, Message message)
    //{
    //    Clients.Group(channelCode).addChatMessage(message);
    //}
}
