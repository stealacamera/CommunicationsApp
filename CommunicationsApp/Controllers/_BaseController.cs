using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunicationsApp.Web.Controllers;

public abstract class BaseController : Controller
{
    private ISender _sender;
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetService<ISender>();

    protected int GetCurrentUserId()
    {
        return User.Identity.IsAuthenticated 
               ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!) 
               : 0;
    }
}
