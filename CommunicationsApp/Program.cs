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

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandlingPath = "/Errors/Index" });

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
