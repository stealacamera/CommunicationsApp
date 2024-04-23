using Microsoft.AspNetCore.Identity;

namespace CommunicationsApp.Domain.Entities;

public class User : IdentityUser<int>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public class Role : IdentityRole<int>
{
}