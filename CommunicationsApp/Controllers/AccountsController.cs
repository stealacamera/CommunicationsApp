using CommunicationsApp.Application.Common.Errors;
using CommunicationsApp.Application.Operations.Identity.Commands.ConfigureExternalAuthProperties;
using CommunicationsApp.Application.Operations.Identity.Commands.ConfirmEmail;
using CommunicationsApp.Application.Operations.Identity.Commands.ExternalLoginUser;
using CommunicationsApp.Application.Operations.Identity.Commands.ExternalSignUpUser;
using CommunicationsApp.Application.Operations.Identity.Commands.LoginUser;
using CommunicationsApp.Application.Operations.Identity.Commands.LogoutUser;
using CommunicationsApp.Application.Operations.Identity.Commands.ResendEmailConfirmation;
using CommunicationsApp.Application.Operations.Identity.Queries.GetExternalAuthSchemes;
using CommunicationsApp.Application.Operations.Users.Commands.CreateUser;
using CommunicationsApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CommunicationsApp.Web.Controllers;

public class AccountsController : BaseController
{
    public AccountsController(IConfiguration configuration) : base(configuration)
    {
    }

    [HttpGet]
    public async Task<IActionResult> SignUp()
    {
        var model = new SignUpDTO {
            AuthSchemes = await Sender.Send(new GetExternalAuthSchemesQuery())
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpDTO model)
    {
        if (!ModelState.IsValid)
            return View(model);

        CreateUserCommand command = new(model.Username, model.Password, model.Email, GetEmailConfirmationUrl());
        var createResult = await Sender.Send(command);

        if (createResult.Succeded)
            return RedirectToAction(nameof(EmailConfirmation));
        else
        {
            ModelState.AddModelError(string.Empty, createResult.Error.Description);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        var model = new LoginDTO
        {
            AuthSchemes = await Sender.Send(new GetExternalAuthSchemesQuery())
        };

        return View(model);
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
                return LocalRedirect(Url.Action("Index", "Home")!);
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LogOff()
    {
        LogoutUserCommand command = new();
        await Sender.Send(command);

        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> ExternalSignUp(string provider)
    {
        var redirectUrl = Url.Action(
            nameof(ExternalSignUpResponse), 
            ControllerContext.ActionDescriptor.ControllerName)!;
        
        var properties = await Sender.Send(new ConfigureExternalAuthPropertiesCommand(provider, redirectUrl));
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> ExternalSignUpResponse(string? returnUrl, string? remoteError)
    {
        string baseExternalError = "Something went wrong with the external login provider. Try again in a moment";

        if (!string.IsNullOrEmpty(remoteError))
        {
            ViewData["ExternalError"] = baseExternalError;
            return View(nameof(SignUp));
        }

        ExternalSignUpCommand command = new();
        var result = await Sender.Send(command);

        if (result.Failed)
        {
            ViewData["ExternalError"] = result.Error == IdentityErrors.ExternalLoginError
                                        ? baseExternalError
                                        : "Something went wrong, please try again";

            return View(nameof(SignUp));
        }
        else
            return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> ExternalLogin(string provider)
    {
        var redirectUrl = Url.Action(
            nameof(ExternalLoginResponse), 
            ControllerContext.ActionDescriptor.ControllerName)!;
        var properties = await Sender.Send(new ConfigureExternalAuthPropertiesCommand(provider, redirectUrl));

        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> ExternalLoginResponse(string? returnUrl, string? remoteError)
    {
        string baseExternalError = "Something went wrong with the external login provider. Try again in a moment";

        if (!string.IsNullOrEmpty(remoteError))
        {
            ViewData["ExternalError"] = baseExternalError;
            return View(nameof(SignUp));
        }

        ExternalLoginCommand command = new();
        var result = await Sender.Send(command);

        if (result.Failed)
        {
            if (result.Error == UserErrors.NotFound)
                ViewData["NonexistantUser"] = "The given account is not signed up";
            else if (result.Error == IdentityErrors.ExternalLoginError)
                ViewData["ExternalError"] = baseExternalError;

            return View(nameof(Login));
        }
        else
            return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult EmailConfirmation(bool? isEmailConfirmed = null)
    {
        return View(isEmailConfirmed);
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail([Required] int userId, [Required] string token)
    {
        ConfirmEmailCommmand confirmationCommand = new(userId, token);
        bool isEmailConfirmed = await Sender.Send(confirmationCommand);

        if (isEmailConfirmed)
        {
            TempData["EmailConfirmed"] = "Email was confirmed successfully";
            return RedirectToAction(nameof(Login));
        }
        else
        {
            TempData["FailedConfirmation"] = "Previous confirmation failed. Please try again";
            return RedirectToAction(nameof(EmailConfirmation), isEmailConfirmed);
        }
    }

    #region API
    [HttpPost]
    public async Task<IActionResult> ResendEmailConfirmation([FromBody, Required, EmailAddress] string email)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        ResendEmailConfirmationCommand command = new(email, GetEmailConfirmationUrl());
        var hasEmailBeenSent = await Sender.Send(command);

        return hasEmailBeenSent ? Ok() : BadRequest();
    }
    #endregion

    private string GetEmailConfirmationUrl()
        => Url.Action(nameof(ConfirmEmail), ControllerContext.ActionDescriptor.ControllerName, null, protocol: Request.Scheme)!;
}
