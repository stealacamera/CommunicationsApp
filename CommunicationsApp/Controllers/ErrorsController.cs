using CommunicationsApp.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationsApp.Web.Controllers;

public class ErrorsController : Controller
{
    public IActionResult Index()
    {
        AppError error = new(
            StatusCodes.Status500InternalServerError, 
            "Something went wrong. Please try again later");

        Response.StatusCode = StatusCodes.Status500InternalServerError;

        return View(error);
    }
}
