using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Web.Controllers;

public abstract class BaseController : Controller
{
    private ISender _sender;
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetService<ISender>();
}
