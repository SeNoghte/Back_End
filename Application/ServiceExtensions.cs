using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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

        services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            config.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddGoogle(op =>
        {
            op.ClientId = "178853996623-7d8dh0tal921q54iju05fhqhqdm03gen.apps.googleusercontent.com";
            op.ClientSecret = "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";
        });

        services.AddAuthentication();
        services.AddAuthorization();
    }
}