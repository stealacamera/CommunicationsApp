using CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Queries.GetUserMembershipForChannel;
using CommunicationsApp.Application.Behaviour.Operations.Channels.Commands.CreateChannel;
using CommunicationsApp.Application.Behaviour.Operations.Channels.Commands.EditChannel;
using CommunicationsApp.Application.Behaviour.Operations.Channels.Queries.GetChannelById;
using CommunicationsApp.Application.Behaviour.Operations.Messages.Queries.GetAllMessagesForChannel;
using CommunicationsApp.Application.DTOs;
using CommunicationsApp.Web.Models;
using CommunicationsApp.Web.Models.ViewModels;
using FluentValidation.AspNetCore;
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

    #region API
    [HttpPost("channels")]
    public async Task<IActionResult> Create([FromBody] Channel_AddRequestModel model)
    {
        var validationResult = await new ChannelValidator().ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return BadRequest(ModelState);
        }

        CreateChannelCommand command = new(GetCurrentUserId(), model.ChannelName, model.MemberIds.ToList());
        var newChannelResult = await Sender.Send(command);

        return ConvertResultToResponse(
            newChannelResult,
            () => Created(
                nameof(Create), 
                new Channel_BriefOverview(newChannelResult.Value, null)));
    }

    [HttpPatch("channels/{id:int}")]
    public async Task<IActionResult> Edit(
        int id, 
        [FromBody, Required, MaxLength(55)] string newName)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        EditChannelCommand command = new(GetCurrentUserId(), id, newName);
        var editResult = await Sender.Send(command);

        return ConvertResultToResponse(editResult, () => Ok());
    }

    [HttpGet("channels/{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        GetChannelByIdQuery channelCommand = new(id, GetCurrentUserId());
        var channelResult = await Sender.Send(channelCommand);

        if (channelResult.Failed)
            return ConvertFailureToResponse(channelResult);

        GetAllMessagesForChannelQuery command = new(id, GetCurrentUserId(), StandardPaginationSize);
        var messagesResult = await Sender.Send(command);

        GetUserMembershipForChannelQuery membershipQuery = new(GetCurrentUserId(), id);
        var membershipResult = await Sender.Send(membershipQuery);
        
        if(membershipResult.Failed)
            return ConvertFailureToResponse(membershipResult);

        return ConvertResultToResponse(
            messagesResult,
            () =>
            {
                Response.StatusCode = StatusCodes.Status200OK;

                return PartialView(
                    "/Views/Shared/Messages/_ChannelMessagesPartial.cshtml",
                    new ChannelMessagesVM
                    {
                        ChannelOverview = new ChannelOverviewVM(channelResult.Value, messagesResult.Value),
                        CurrentUser = membershipResult.Value!
                    });
            });
    }
    #endregion
}
