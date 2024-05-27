using CommunicationsApp.Application;
using CommunicationsApp.Infrastructure;
using CommunicationsApp.Web.Common.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                });

builder.Services.RegisterApplicationServices();
builder.Services.RegisterInfrastructureServices(builder.Configuration);


builder.WebHost.ConfigureKestrel(options => 
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024);

builder.Services.AddSignalR();

Log.Logger = new LoggerConfiguration().ReadFrom
                                      .Configuration(builder.Configuration)
                                      .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandlingPath = "/Errors/Index" });
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        context.Request.Path = "/Errors/NotFound";
        await next();
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatApp");
app.Run();
