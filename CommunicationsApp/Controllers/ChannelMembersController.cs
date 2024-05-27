using CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Commands.AddMembers;
using CommunicationsApp.Application.Behaviour.Operations.ChannelMembers.Commands.RemoveMembers;
using CommunicationsApp.Application.Behaviour.Operations.Channels.Queries.GetChannelByCode;
using CommunicationsApp.Application.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            return NotFound(channelQueryResult.Errors);

        // Add current user to channel
        AddChannelMembersCommand addCurrentUserCommand = new(null, channelQueryResult.Value.Id, 
                                                            ChannelRole.Member, GetCurrentUserId());
        
        var addMemberResult = await Sender.Send(addCurrentUserCommand);

        return ConvertResultToResponse(
            addMemberResult, 
            () => Created(nameof(JoinByCode), addMemberResult.Value));
    }

    [HttpPost]
    public async Task<IActionResult> AddMembersToChannel(int channelId, [FromBody] int[] userIds)
    {
        AddChannelMembersCommand command = new(GetCurrentUserId(), channelId, 
                                               ChannelRole.Member, userIds);
        
        var result = await Sender.Send(command);

        return ConvertResultToResponse(
            result, 
            () => Created(nameof(AddMembersToChannel), result.Value));
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveMemberFromChannel(int channelId, int memberId)
    {
        RemoveChannelMembersCommand command = new(GetCurrentUserId(), channelId, memberId);
        var result = await Sender.Send(command);

        return ConvertResultToResponse(result, () => NoContent());
    }
}
