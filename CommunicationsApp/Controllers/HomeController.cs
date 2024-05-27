using CommunicationsApp.Application.Behaviour.Operations.Channels.Queries.GetAllChannelsForUser;
using CommunicationsApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Controllers;

public class HomeController : BaseController
{
    public HomeController(IConfiguration configuration) : base(configuration)
    {
    }

    public async Task<IActionResult> Index()
    {
        int userId = GetCurrentUserId();

        if (userId == 0)
            return View("Welcome");

        GetAllChannelsForUserQuery command = new(userId);
        var userChannelsResult = await Sender.Send(command);

        return ConvertResultToResponse(
            userChannelsResult,
            () =>
            {
                ViewBag.CurrentUserId = userId;
                return View(userChannelsResult.Value);
            });
    }
}