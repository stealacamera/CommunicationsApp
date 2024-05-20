using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Application.Operations.Channels.Commands.CreateChannel;
using CommunicationsApp.Application.Operations.Channels.Commands.EditChannel;
using CommunicationsApp.Application.Operations.Channels.Queries.GetChannelById;
using CommunicationsApp.Application.Operations.Messages.Queries.GetAllMessagesForChannel;
using CommunicationsApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Controllers;

[Authorize]
public class ChannelsController : BaseController
{
    public ChannelsController(IConfiguration configuration) : base(configuration)
    {
    }

    [HttpPost("channels")]
    public async Task<IActionResult> Create([FromBody] CreateChannelDTO model)
    {
        CreateChannelCommand command = new(GetCurrentUserId(), model.ChannelName, model.MemberIds.ToList());
        var newChannelResult = await Sender.Send(command);

        return newChannelResult.Failed
               ? BadRequest(newChannelResult.Error.Description)
               : Created(nameof(Create), new Channel_BriefOverview(newChannelResult.Value, null));
    }

    [HttpPatch("channels/{id:int}")]
    public async Task<IActionResult> Edit(int id, [FromBody, Required, MaxLength(55)] string newName)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        EditChannelCommand command = new(GetCurrentUserId(), id, newName);
        var editResult = await Sender.Send(command);

        return editResult.Failed
               ? BadRequest(editResult.Error.Description)
               : Ok();
    }

    [HttpGet("channels/{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        GetChannelByIdQuery channelCommand = new(id, GetCurrentUserId());
        var channelResult = await Sender.Send(channelCommand);

        if (channelResult.Failed)
            return BadRequest(channelResult.Error.Description);

        GetAllMessagesForChannelQuery command = new(id, GetCurrentUserId(), GetStandardPaginationSize());
        var messagesResult = await Sender.Send(command);

        if (messagesResult.Succeded)
        {
            Response.StatusCode = StatusCodes.Status200OK;
            return PartialView(
                "/Views/Shared/Messages/_ChannelMessagesPartial.cshtml", 
                new ChannelOverviewVM(channelResult.Value, messagesResult.Value));
        }
        else
            return BadRequest(messagesResult.Error.Description);
    }
}
