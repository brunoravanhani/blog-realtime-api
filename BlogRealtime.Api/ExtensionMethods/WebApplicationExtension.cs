using BlogRealtime.Domain.Cryptography;
using BlogRealtime.Infra.Seed;

namespace BlogRealtime.Infra.ExtensionMethods;

public static class ServiceProviderExtension
{
    public static void SetupDBSeed(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
            var cryptography = scope.ServiceProvider.GetRequiredService<ICryptographyHelper>();
            db.Database.EnsureCreated();
            DbSeeder.Seed(db, cryptography);
        }
    }
}
