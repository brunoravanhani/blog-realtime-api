using BlogRealtime.Application.Interfaces;
using BlogRealtime.Application.Services;
using BlogRealtime.Application.Validators;
using BlogRealtime.Domain.Dtos;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BlogRealtime.Application.ExtensionMethods;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPostApplication, PostApplication>();
        services.AddScoped<IUserApplication, UserApplication>();

        services.AddScoped<IValidator<CreateUserDto>, CreateUserValidator>();
        services.AddScoped<IValidator<UserLoginDto>, UserLoginValidator>();
        services.AddScoped<IValidator<CreatePostDto>, CreatePostValidator>();
        services.AddScoped<IValidator<UpdatePostDto>, UpdatePostValidator>();
        return services;
    }
}
