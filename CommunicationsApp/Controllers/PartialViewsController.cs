using CommunicationsApp.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Web.Controllers;

public class PartialViewsController : Controller
{
    [HttpPost("partialViews/messagePartial")]
    public IActionResult GetMessagePartialView([FromBody] Message message)
    {
        return PartialView("_MessagePartial", message);
    }
}
