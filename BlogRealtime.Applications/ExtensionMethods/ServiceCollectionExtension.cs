using BlogRealtime.Application.Interfaces;
using BlogRealtime.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlogRealtime.Application.ExtensionMethods;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPostApplication, PostApplication>();
        services.AddScoped<IUserApplication, UserApplication>();
        return services;
    }
}
