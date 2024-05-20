using CommunicationsApp.Application.Common.Enums;
using CommunicationsApp.Application.Operations.ChannelMembers.Commands.AddMembers;
using CommunicationsApp.Application.Operations.ChannelMembers.Commands.RemoveMembers;
using CommunicationsApp.Application.Operations.Channels.Queries.GetChannelByCode;
using CommunicationsApp.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Controllers;

[Authorize]
public class ChannelMembersController : BaseController
{
    public ChannelMembersController(IConfiguration configuration) : base(configuration)
    {
    }

    [HttpPost]
    public async Task<IActionResult> JoinByCode(string code)
    {
        // Check if channel exists
        GetChannelByCodeQuery channelQuery = new(code);
        var channelQueryResult = await Sender.Send(channelQuery);

        if (channelQueryResult.Failed)
            return NotFound();

        // Add current user to channel
        AddChannelMembersCommand addCurrentUserCommand = new(null, channelQueryResult.Value.Id, ChannelRole.Member, GetCurrentUserId());
        var addMemberResult = await Sender.Send(addCurrentUserCommand);

        return (addMemberResult.Failed || !addMemberResult.Value.Any()) ?
               BadRequest() :
               Created(nameof(JoinByCode), addMemberResult.Value);
    }

    [HttpPost]
    public async Task<IActionResult> AddUserAsMembersToChannel(int channelId, [FromBody, Required] int[] userIds)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        AddChannelMembersCommand command = new(GetCurrentUserId(), channelId, ChannelRole.Member, userIds);
        var result = await Sender.Send(command);

        if (result.Failed)
            return result.Error.Type == ErrorType.Unauthorized ?
                   Unauthorized() :
                   BadRequest(result.Error.Description);
        else
            return Created(nameof(AddUserAsMembersToChannel), result.Value);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveMemberFromChannel(int channelId, int memberId)
    {
        RemoveChannelMembersCommand command = new(GetCurrentUserId(), channelId, memberId);
        var result = await Sender.Send(command);

        if (result.Failed)
            return result.Error.Type == ErrorType.Unauthorized ?
                   Unauthorized() :
                   BadRequest(result.Error.Description);
        else
        {
            Response.StatusCode = StatusCodes.Status204NoContent;
            return Json(result.Value);
        }
    }
}
