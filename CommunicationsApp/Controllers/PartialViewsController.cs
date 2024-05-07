using CommunicationsApp.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Web.Controllers;

public class PartialViewsController : BaseController
{
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
