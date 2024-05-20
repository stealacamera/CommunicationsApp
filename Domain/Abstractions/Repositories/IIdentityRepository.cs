using CommunicationsApp.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IIdentityRepository
{
    Task SignInUserAsync(User user, bool isPersistent = true);
    Task<SignInResult> SignInUserAsync(User user, string password, bool isPersistent = true);

    Task LogOutUserAsync();
    Task<ExternalLoginInfo> GetExternalLoginInfoAsync();
    
    Task<IEnumerable<AuthenticationScheme>> GetExternalAuthSchemesAsync();
    AuthenticationProperties ConfigureExternalAuthProperties(string provider, string redirectUrl);
}