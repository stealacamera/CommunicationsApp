using CommunicationsApp.Application;
using CommunicationsApp.Domain.Abstractions;
using CommunicationsApp.Domain.Entities;
using CommunicationsApp.Infrastructure.Email;
using CommunicationsApp.Infrastructure.Email.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommunicationsApp.Infrastructure;

public static class Startup
{
    public static void RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Application
        services.RegisterApplicationServices();

        // Database
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
            configuration.GetConnectionString("DbConnectionString")));

        services.AddDefaultIdentity<User>(options =>
                {
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;

                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";

                    options.SignIn.RequireConfirmedAccount = true;
                    options.Lockout.AllowedForNewUsers = false;

                    // Email
                    options.Tokens.ProviderMap.Add(
                        nameof(EmailConfirmationTokenProvider<User>),
                        new TokenProviderDescriptor(typeof(EmailConfirmationTokenProvider<User>)));

                    options.Tokens.EmailConfirmationTokenProvider = nameof(EmailConfirmationTokenProvider<User>);
                })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<AppDbContext>();

        services.ConfigureApplicationCookie(options =>
        {
            string loginPath = "/Accounts/Login",
                   logoutPath = "/Accounts/LogOff;";

            options.ForwardSignIn = loginPath;
            options.ForwardSignOut = logoutPath;
            options.LoginPath = loginPath;
            options.LogoutPath = logoutPath;
        });

        services.AddTransient<EmailConfirmationTokenProvider<User>>();
        services.AddScoped<IWorkUnit, WorkUnit>();

        // Email
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.ConfigurationName));
        services.AddTransient<IEmailService, EmailService>();

        // Logger
        services.AddTransient<ILogger, Logger>();
    }
}
