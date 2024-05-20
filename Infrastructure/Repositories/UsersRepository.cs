using CommunicationsApp.Domain.Abstractions.Repositories;
using CommunicationsApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CommunicationsApp.Infrastructure.Repositories;

internal class UsersRepository : IUsersRepository
{
    private readonly UserManager<User> _userManager;
    private readonly IQueryable<User> _untrackedSet;

    public UsersRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
        _untrackedSet = _userManager.Users.AsNoTracking();
    }

    public async Task<IdentityResult> AddAsync(User user, string? password = null)
    {
        return password != null
               ? await _userManager.CreateAsync(user, password)
               : await _userManager.CreateAsync(user);
    }

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

    public async Task<bool> ConfirmEmailTokenAsync(User user, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<string> GetEmailConfirmationTokenAsync(User user)
        => await _userManager.GenerateEmailConfirmationTokenAsync(user);

    public async Task<User?> GetByEmailAsync(string email, bool excludeDeleted = true, bool excludeNonConfirmedEmail = true)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null
           || (excludeDeleted && user.DeletedAt != null)
           || (excludeNonConfirmedEmail && user.EmailConfirmed == false))
            return null;

        return user;
    }

    public async Task<IEnumerable<User>> QueryByEmailAndUsernameAsync(string queryString, CancellationToken cancellationToken, int? excludeUserId = null)
    {
        if (queryString.IsNullOrEmpty())
            return new List<User>();

        IQueryable<User> query = _untrackedSet.Where(
            e => e.Email.Contains(queryString)
            || e.UserName.Contains(queryString));

        if (excludeUserId.HasValue)
            query = query.Where(e => e.Id != excludeUserId.Value);

        return await query.ToListAsync(cancellationToken);
    }
}
