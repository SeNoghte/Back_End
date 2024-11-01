using System.Reflection;
using System.Runtime.Loader;
using Application.Common.Services.IdentityService;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceExtensions
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
        });

        services.AddScoped<ApplicationDBContext>();
        services.AddScoped<IIdentityService, IdentityService>();
    }
}