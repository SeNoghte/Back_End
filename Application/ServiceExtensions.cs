using Application.Common.Services.IdentityService;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class ServiceExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        services.AddHttpClient();

        services.AddScoped<ApplicationDBContext>();
        services.AddScoped<IIdentityService, IdentityService>();
    }
}