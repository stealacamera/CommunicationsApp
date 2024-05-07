using CommunicationsApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CommunicationsApp.Domain.Abstractions.Repositories;

public interface IUsersRepository
{
    Task<IdentityResult> AddAsync(User user, string password);

    Task<SignInResult> SignInUserAsync(User user, string password, bool isPersistent = true);
    Task LogOutUserAsync();

    Task<string> GetEmailConfirmationTokenAsync(User user);
    Task<bool> IsEmailConfirmationTokenValidAsync(User user, string token);
    
    Task<User?> GetByIdAsync(int id, bool excludeDeleted = true, bool excludeNonConfirmedEmail = true);
    Task<User?> GetByEmailAsync(string email, bool excludeDeleted = true, bool excludeNonConfirmedEmail = true);
    Task<bool> DoesUserExistAsync(int id, bool excludeDeleted = true);

    Task<IEnumerable<User>> QueryByEmailAndUsernameAsync(string query, int? excludeUserId = null);
}