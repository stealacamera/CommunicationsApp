using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.Operations.Identity.Commands.ConfirmEmail;
using CommunicationsApp.Application.Operations.Identity.Commands.LoginUser;
using CommunicationsApp.Application.Operations.Identity.Commands.ResendEmailConfirmation;
using CommunicationsApp.Application.Operations.Users.Commands.CreateUser;
using CommunicationsApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Controllers;

public class IdentityController : BaseController
{
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
        var createResult = await Sender.Send(command);

        if(createResult.Succeded)
            return RedirectToAction(nameof(EmailConfirmation));
        else
        {
            ModelState.AddModelError(string.Empty, createResult.Error.Description);
            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        LoginUserCommand command = new(model.Email, model.Password, model.RememberMe);
        var loginResult = await Sender.Send(command);

        if (loginResult.Succeded)
        {
            if (loginResult.Value)
                return RedirectToAction("Index", "Home");
            else
            {
                ModelState.AddModelError(string.Empty, "Wrong email and/or password. Try again");
                return View(model);
            }
        }
        else
        {
            if (loginResult.Error == UserErrors.NotFound)
            {
                ModelState.AddModelError(string.Empty, "Wrong email and/or password. Try again");
                return View(model);
            }
            else if (loginResult.Error == IdentityErrors.UnverifiedEmail)
                return RedirectToAction(nameof(EmailConfirmation), new { isEmailConfirmed = false });
            else
            {
                ModelState.AddModelError(string.Empty, "Something went wrong. Try again later");
                return View(model);
            }
        }
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
        ConfirmEmailCommmand confirmationCommand = new(userId, token);

        bool isEmailConfirmed = await Sender.Send(confirmationCommand);
        return RedirectToAction(nameof(EmailConfirmation), isEmailConfirmed);
    }

    #region API
    [HttpPost]
    public async Task<IActionResult> ResendEmailConfirmation([FromBody, EmailAddress] string email)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        ResendEmailConfirmationCommand command = new(email, Url.Action(nameof(ConfirmEmail)));
        var hasEmailBeenSent = await Sender.Send(command);

        return hasEmailBeenSent ? Ok() : BadRequest();
    }
    #endregion
}
