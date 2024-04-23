﻿using CommunicationsApp.Domain.Entities;
using CommunicationsApp.Infrastructure.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommunicationsApp.Infrastructure;

public static class Startup
{
    public static void RegisterInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
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
        })
                .AddRoles<Role>()
                .AddEntityFrameworkStores<AppDbContext>();

        // Email
        services.Configure<EmailOptions>(configuration.GetSection("EmailOptions"));
        services.AddTransient<IEmailSender, EmailSender>();
    }
}