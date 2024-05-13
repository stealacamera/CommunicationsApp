using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunicationsApp.Web.Controllers;

public abstract class BaseController : Controller
{
    private IConfiguration _configuration;
    private int _standardPaginationSize;

    private ISender _sender;
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetService<ISender>();


    public BaseController(IConfiguration configuration)
        => _configuration = configuration;

    protected int GetStandardPaginationSize()
    {
        if (_standardPaginationSize <= 0)
        {
            if (!int.TryParse(_configuration["PaginationSize"], out _standardPaginationSize))
                throw new ArgumentException("\"PaginationSize\" configuration key missing");
        }

        return _standardPaginationSize;
    }

    protected int GetCurrentUserId()
    {
        return User.Identity.IsAuthenticated
               ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
               : 0;
    }
}
