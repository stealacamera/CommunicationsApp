using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.Operations.Channels.Commands.CreateChannel;
using CommunicationsApp.Application.Operations.Channels.Queries.GetChannelById;
using CommunicationsApp.Application.Operations.Messages.Queries.GetAllMessagesForChannel;
using CommunicationsApp.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Web.Controllers;

//[Authorize]
public class ChannelsController : BaseController
{
    [HttpPost("channels")]
    public async Task<IActionResult> Create([FromBody] CreateChannelDTO model)
    {
        CreateChannelCommand command = new(GetCurrentUserId(), model.ChannelName, model.MemberIds.ToList());
        var newChannelResult = await Sender.Send(command);

        return newChannelResult.Failed
               ? BadRequest(newChannelResult.Error.Description)
               : Created(nameof(Create), new Channel_BriefOverview(newChannelResult.Value, null));
    }

    [HttpGet("channels/{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        GetChannelByIdQuery channelCommand = new(id, GetCurrentUserId());
        var channelResult = await Sender.Send(channelCommand);

        if (channelResult.Failed)
            return BadRequest(channelResult.Error.Description);

        GetAllMessagesForChannelQuery command = new(id, GetCurrentUserId());
        var messagesResult = await Sender.Send(command);

        if (messagesResult.Succeded)
        {
            Response.StatusCode = StatusCodes.Status200OK;
            return PartialView(
                "_ChannelMessagesPartial", 
                new ChannelOverviewVM(channelResult.Value, messagesResult.Value));
        }
        else
            return BadRequest(messagesResult.Error.Description);
    }
}
