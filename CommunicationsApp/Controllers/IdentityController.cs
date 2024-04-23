using CommunicationsApp.Application.Identity.Commands;
using CommunicationsApp.Application.Users.Commands;
using CommunicationsApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using System.ComponentModel.DataAnnotations;

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
            return RedirectToAction(nameof(SignUp));

        CreateUserCommand command = new()
        {
            Email = model.Email,
            Password = model.Password,
            Username = model.Username
        };

        var user = await Sender.Send(command);

        return View();
    }

    [HttpGet]
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
        return View(isEmailConfirmed);
    }

    #region API
    [HttpPost]
    public async Task<IActionResult> ResendEmailConfirmation([FromBody, EmailAddress] string email)
    {
        ResendEmailConfirmationCommand command = new() { Email = email };
        var hasEmailBeenSent = await Sender.Send(command);

        return hasEmailBeenSent ? Ok() : BadRequest(/*"Something went wrong with your request. Please make sure that the email you inputted was the one you signed up with"*/);
    }
    #endregion
}
