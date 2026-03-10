using BlogRealtime.Domain.Cryptography;
using BlogRealtime.Domain.Repository;
using BlogRealtime.Domain.Services;
using BlogRealtime.Domain.Settings;
using BlogRealtime.Infra.Cryptography;
using BlogRealtime.Infra.Repositories;
using BlogRealtime.Infra.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BlogRealtime.Infra.ExtensionMethods;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfraServices(this IServiceCollection services, JwtSettings jwtSettings)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        services.AddDbContext<BlogDbContext>(options =>
        {
            options.UseSqlite(connection);
        });

        services.AddTransient<ICryptographyHelper, Argon2CryptographyHelper>();
        services.AddScoped<IUserRepository, InMemoryUserRepository>();
        services.AddScoped<IPostRepository, InMemoryPostRepository>();
        services.AddSingleton<ITokenService>(new TokenService(jwtSettings));
        return services;
    }
}
