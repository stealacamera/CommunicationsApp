using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunicationsApp.Infrastructure.Repositories;

internal class UsersRepository : IUsersRepository
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IQueryable<User> _untrackedSet;

    public UsersRepository(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _untrackedSet = _userManager.Users.AsNoTracking();

        _signInManager = signInManager;
    }

    public async Task<IdentityResult> AddAsync(User user, string password)
        => await _userManager.CreateAsync(user, password);

    public async Task<bool> DoesUserExistAsync(int id, bool excludeDeleted = true)
        => await GetByIdAsync(id, excludeDeleted) != null;

    public async Task<User?> GetByIdAsync(int id, bool excludeDeleted = true, bool excludeNonConfirmedEmail = true)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (excludeDeleted && user.DeletedAt != null)
            return null;
        else if (excludeNonConfirmedEmail && user.EmailConfirmed == false)
            return null;

        return user;
    }

    public async Task<bool> IsEmailConfirmationTokenValidAsync(User user, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<string> GetEmailConfirmationTokenAsync(User user)
        => await _userManager.GenerateEmailConfirmationTokenAsync(user);

    public async Task<IEnumerable<User>> QueryByEmail(string queryString)
    {
        if(string.IsNullOrEmpty(queryString) || string.IsNullOrWhiteSpace(queryString))
            return new List<User>();

        IQueryable<User> query = _untrackedSet.Where(
            e => e.EmailConfirmed == true 
            && e.Email.Contains(queryString));

        return await query.ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email, bool excludeDeleted = true, bool excludeNonConfirmedEmail = true)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (excludeDeleted && user.DeletedAt != null)
            return null;
        else if (excludeNonConfirmedEmail && user.EmailConfirmed == false)
            return null;

        return user;
    }

    public async Task<SignInResult> SignInUserAsync(User user, string password, bool isPersistent = true)
        => await _signInManager.PasswordSignInAsync(user, password, isPersistent, false);
}
