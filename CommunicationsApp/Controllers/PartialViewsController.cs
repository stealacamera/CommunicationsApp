using CommunicationsApp.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Web.Controllers;

[Authorize]
public class PartialViewsController : BaseController
{
    public PartialViewsController(IConfiguration configuration) : base(configuration)
    {
    }

    [HttpPost("partialViews/messagePartial")]
    public IActionResult GetMessagePartialView([FromBody] Message message)
    {
        return PartialView("_MessagePartial", message);
    }

    [HttpPost("partialViews/channelSidebarPartial")]
    public IActionResult GetChannelMessagesPartial([FromBody] Channel_BriefOverview channel)
    {
        return PartialView("_ChannelSidebarGroupPartial", channel);
    }
}
