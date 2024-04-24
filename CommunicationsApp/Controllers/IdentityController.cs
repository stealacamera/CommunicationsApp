using CommunicationsApp.Application.Identity.Commands;
using CommunicationsApp.Application.Users.Commands;
using CommunicationsApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CommunicationsApp.Web.Controllers;

public class IdentityController : BaseController
{
    // login
    // logout

    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        string baseUrl = Url.Action(nameof(ConfirmEmail));

        CreateUserCommand command = new(model.Username, model.Password, model.Email, baseUrl);
        await Sender.Send(command);
        
        return RedirectToAction(nameof(EmailConfirmation));
    }

    [HttpGet]
    public IActionResult EmailConfirmation(bool? isEmailConfirmed = null)
    {
        return View(isEmailConfirmed);
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery(Name = "userId")] int userId, 
        [FromQuery(Name = "token")] string token)
    {
        ConfirmEmailCommmand confirmationCommand = new()
        {
            EmailConfirmationToken = token,
            UserId = userId
        };

        bool isEmailConfirmed = await Sender.Send(confirmationCommand);
        return RedirectToAction(nameof(EmailConfirmation), isEmailConfirmed);
    }

    #region API
    [HttpPost]
    public async Task<IActionResult> ResendEmailConfirmation([FromBody, EmailAddress] string email)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        ResendEmailConfirmationCommand command = new() { Email = email };
        var hasEmailBeenSent = await Sender.Send(command);

        return hasEmailBeenSent ? Ok() : BadRequest();
    }
    #endregion
}
