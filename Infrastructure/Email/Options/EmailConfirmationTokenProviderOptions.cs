using Microsoft.AspNetCore.Identity;

namespace CommunicationsApp.Infrastructure.Email.Options;

public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{
    public EmailConfirmationTokenProviderOptions()
    {
        Name = "EmailDataProtectorTokenProvider";
        TokenLifespan = TimeSpan.FromHours(1);
    }
}
