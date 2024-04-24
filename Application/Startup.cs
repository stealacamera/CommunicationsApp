using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CommunicationsApp.Application;

public static class Startup
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(
            configuration => configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    }
}
