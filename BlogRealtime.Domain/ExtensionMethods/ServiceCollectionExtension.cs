using BlogRealtime.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlogRealtime.Domain.ExtensionMethods;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
