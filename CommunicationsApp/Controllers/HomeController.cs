using CommunicationsApp.Application.Operations.Channels.Queries.GetAllChannelsForUser;
using CommunicationsApp.Models;
using CommunicationsApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CommunicationsApp.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
        {
        }

        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();

            if (userId == 0)
                return View("Welcome");

            GetAllChannelsForUserCommad command = new(userId);
            var userChannelsResult = await Sender.Send(command);

            if (userChannelsResult.Failed)
                throw new UnauthorizedAccessException();

            return View(userChannelsResult.Value);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}