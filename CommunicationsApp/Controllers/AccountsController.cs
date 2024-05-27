using CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ConfigureExternalAuthProperties;
using CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ConfirmEmail;
using CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ExternalLoginUser;
using CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ExternalSignUpUser;
using CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.LoginUser;
using CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.LogoutUser;
using CommunicationsApp.Application.Behaviour.Operations.Identity.Commands.ResendEmailConfirmation;
using CommunicationsApp.Application.Behaviour.Operations.Identity.Queries.GetExternalAuthSchemes;
using CommunicationsApp.Application.Behaviour.Operations.Users.Commands.CreateUser;
using CommunicationsApp.Domain.Common;
using CommunicationsApp.Web.Models;
using FluentValidation.AspNetCore;
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
        var model = new SignUpDTO
        {
            AuthSchemes = await Sender.Send(new GetExternalAuthSchemesQuery())
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpDTO model)
    {
        var validationResult = await new SignUpValidator().ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            model.AuthSchemes = await Sender.Send(new GetExternalAuthSchemesQuery());
            return View(model);
        }

        CreateUserCommand command = new(model.Username, model.Password,
                                        model.Email, GetEmailConfirmationUrl());

        var createResult = await Sender.Send(command);

        if (createResult.Succeded)
            return RedirectToAction(nameof(EmailConfirmation));
        else
        {
            model.AuthSchemes = await Sender.Send(new GetExternalAuthSchemesQuery());
            AddSignupModelErrors(createResult.Errors);

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
        var validationResult = await new LoginValidator().ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            model.AuthSchemes = await Sender.Send(new GetExternalAuthSchemesQuery());

            return View(model);
        }

        LoginUserCommand command = new(model.Email, model.Password, model.RememberMe);
        var loginResult = await Sender.Send(command);

        return loginResult.Succeded ?
               await GetLoginSuccessResponseAsync(model, loginResult.Value) :
               await GetLoginFailResponseAsync(loginResult, model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> LogOff()
    {
        LogoutUserCommand command = new(GetCurrentUserId());
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

    [HttpGet, HttpPost]
    public async Task<IActionResult> ExternalSignUpResponse(string? returnUrl, string? remoteError)
    {
        string baseExternalError = @"Something went wrong. Make sure this email 
                                    hasn't been used for an existing account and try again later";

        if (!string.IsNullOrEmpty(remoteError))
        {
            ViewData["ExternalError"] = baseExternalError;
            return View(nameof(SignUp));
        }

        ExternalSignUpCommand command = new();
        var result = await Sender.Send(command);

        if (result.Failed)
        {
            ViewData["ExternalError"] = baseExternalError;
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

    [HttpGet, HttpPost]
    public async Task<IActionResult> ExternalLoginResponse(string? returnUrl, string? remoteError)
    {
        if (!string.IsNullOrEmpty(remoteError))
        {
            ViewData["ExternalError"] = @"Something went wrong with the external login 
                                        provider. Try again in a moment";

            var model = new SignUpDTO
            {
                AuthSchemes = await Sender.Send(new GetExternalAuthSchemesQuery())
            };

            return View(nameof(SignUp), model);
        }

        ExternalLoginCommand command = new();
        var result = await Sender.Send(command);

        if (result.Failed)
        {
            ViewData["ExternalError"] = @"Something went wrong with the external 
                                        login provider. Make sure you have signed
                                        up with this email and try again in a moment";

            LoginDTO model = new LoginDTO
            {
                AuthSchemes = await Sender.Send(new GetExternalAuthSchemesQuery())
            };

            return View(nameof(Login), model);
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
    public async Task<IActionResult> ConfirmEmail(
        [Required, Range(1, int.MaxValue)] int userId,
        [Required] string token)
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

    private async Task<IActionResult> GetLoginSuccessResponseAsync(LoginDTO model, bool loginSucceded)
    {
        if (loginSucceded)
            return LocalRedirect(Url.Action("Index", "Home")!);
        else
        {
            model.AuthSchemes = await Sender.Send(new GetExternalAuthSchemesQuery());
            ModelState.AddModelError(string.Empty, "Wrong email and/or password. Try again");

            return View(model);
        }
    }

    private async Task<IActionResult> GetLoginFailResponseAsync(Result errorResult, LoginDTO model)
    {
        if (errorResult.ContainsErrorType(ErrorType.UnverifiedUser))
            return RedirectToAction(nameof(EmailConfirmation), new { isEmailConfirmed = false });
        else
        {
            string errorMessage = errorResult.ContainsErrorType(ErrorType.NotFound) ?
                                  "Wrong email and/or password. Try again" :
                                  "Something went wrong. Try again later";

            model.AuthSchemes = await Sender.Send(new GetExternalAuthSchemesQuery());
            ModelState.AddModelError(string.Empty, errorMessage);

            return View(model);
        }
    }

    private void AddSignupModelErrors(IList<Error> errors)
    {
        var propertyNames = typeof(SignUpDTO).GetProperties()
                                                 .Select(e => e.Name)
                                                 .ToList();

        foreach (var error in errors)
        {
            string errorPropertyName = string.Empty;
            int propertyIndex = propertyNames.FindIndex(e => e.Equals(
                                                error.PropertyName,
                                                StringComparison.OrdinalIgnoreCase));

            if (propertyIndex >= 0)
                errorPropertyName = propertyNames[propertyIndex];

            foreach (var reason in error.Reasons)
                ModelState.AddModelError(errorPropertyName, reason);
        }
    }

    private string GetEmailConfirmationUrl()
        => Url.Action(
                nameof(ConfirmEmail),
                ControllerContext.ActionDescriptor.ControllerName,
                null,
                protocol: Request.Scheme)!;
}
