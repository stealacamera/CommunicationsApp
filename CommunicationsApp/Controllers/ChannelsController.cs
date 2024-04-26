using CommunicationsApp.Application.Operations.Channels.Commands.CreateChannel;
using CommunicationsApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Web.Controllers;

//[Authorize]
public class ChannelsController : BaseController
{
    #region API
    public async Task<IActionResult> Create([FromBody] CreateChannelDTO model)
    {
        CreateChannelCommand command = new(GetCurrentUserId(), model.ChannelName, model.MemberIds.ToList());
        var newChannelResult = await Sender.Send(command);

        return newChannelResult.Succeded
               ? Created(nameof(Create), newChannelResult.Value)
               : BadRequest(newChannelResult.Error.Description);
    }
    #endregion

    public IActionResult Index()
    {
        return View();
    }
}
