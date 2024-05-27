using CommunicationsApp.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunicationsApp.Web.Controllers;

public abstract class BaseController : Controller
{
    private IConfiguration _configuration;

    public BaseController(IConfiguration configuration)
        => _configuration = configuration;

    private int _standardPaginationSize;
    protected int StandardPaginationSize
    {
        get
        {
            if (_standardPaginationSize <= 0)
            {
                if (!int.TryParse(_configuration["PaginationSize"], out _standardPaginationSize))
                    throw new ArgumentException("\"PaginationSize\" configuration key missing");
            }

            return _standardPaginationSize;
        }
    }

    private ISender _sender;
    protected ISender Sender => _sender ??= HttpContext.RequestServices.GetService<ISender>();

    protected int GetCurrentUserId()
    {
        return User.Identity!.IsAuthenticated
               ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
               : 0;
    }

    protected IActionResult ConvertFailureToResponse(Result result)
    {
        if (result.Succeded)
            throw new ArgumentException("Cannot convert successful result");

        AddResultToModelstate(result);

        if (result.Errors.Any(error => error.Type == ErrorType.Unauthorized))
            return Unauthorized(ModelState);
        else if (result.Errors.Any(error => error.Type == ErrorType.NotFound))
            return NotFound(ModelState);
        else
            return BadRequest(ModelState);
    }

    protected IActionResult ConvertResultToResponse(Result result, Func<IActionResult> onSuccess)
    {
        return result.Failed ? 
               ConvertFailureToResponse(result) : 
               onSuccess();
    }

    protected void AddResultToModelstate(Result result)
    {
        if (result.Succeded)
            throw new ArgumentException();

        foreach(var error in result.Errors)
        {
            foreach (var reason in error.Reasons)
                ModelState.AddModelError(error.PropertyName, reason);
        }
    }
}
