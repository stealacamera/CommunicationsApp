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

    [HttpPost("partialView/messagePreview")]
    public IActionResult GetMessagePreview([FromBody] Message message)
    {
        return PartialView("/Views/Shared/Messages/_ChannelSidebarMessagePreview.cshtml", message);
    }

    [HttpPost("partialViews/messagePartial")]
    public IActionResult GetMessagePartialView([FromBody] Message message)
    {
        return PartialView("/Views/Shared/Messages/_MessagePartial.cshtml", message);
    }

    [HttpPost("partialViews/channelSidebarPartial")]
    public IActionResult GetChannelMessagesPartial([FromBody] Channel_BriefOverview channel)
    {
        ViewBag["CurrentUserId"] = GetCurrentUserId();
        return PartialView("/Views/Shared/Channels/_ChannelSidebarGroupPartial.cshtml", channel);
    }

    [HttpPost("partialViews/openChannelMembersListItem")]
    public IActionResult GetOpenChannelMembersListItem([FromBody] ChannelMember member)
    {
        ViewBag["CurrentUserId"] = GetCurrentUserId();
        return PartialView("/Views/Shared/Channels/_OpenChannelMembersListItem.cshtml", member);
    }
}
