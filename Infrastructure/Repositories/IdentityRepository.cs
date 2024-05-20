using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace CommunicationsApp.Infrastructure.Repositories;

internal class IdentityRepository : IIdentityRepository
{
    private readonly SignInManager<User> _signInManager;

    public IdentityRepository(SignInManager<User> signInManager)
        => _signInManager = signInManager;

    public async Task SignInUserAsync(User user, bool isPersistent = true)
        => await _signInManager.SignInAsync(user, isPersistent);

    public async Task<SignInResult> SignInUserAsync(User user, string password, bool isPersistent = true)
        => await _signInManager.PasswordSignInAsync(user, password, isPersistent, false);

    public async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        => await _signInManager.GetExternalLoginInfoAsync();

    public async Task LogOutUserAsync()
        => await _signInManager.SignOutAsync();

    public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthSchemesAsync()
        => await _signInManager.GetExternalAuthenticationSchemesAsync();

    public AuthenticationProperties ConfigureExternalAuthProperties(string provider, string redirectUrl)
        => _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
}
