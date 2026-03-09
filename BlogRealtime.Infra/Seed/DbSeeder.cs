using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogRealtime.Infra.Seed;

public static class DbSeeder
{
    public static void Seed(BlogDbContext context)
    {
        if (context.Posts.Any())
            return;

        var user = new Domain.Entity.User(
            name: "Admin User",
            email: "admin@example.com",
            password: "password123"
        );

        context.Users.Add(user);

        var posts = new[]
        {
            new Domain.Entity.Post(
                title: "Welcome to the blog",
                body: "This is the first post seeded into the database.",
                image: "https://example.com/images/welcome.jpg",
                authorId: user.Id
            ),
            new Domain.Entity.Post(
                title: "Realtime updates",
                body: "Realtime features are now enabled.",
                image: "https://example.com/images/realtime.jpg",
                authorId: user.Id
            )
        };

        context.Posts.AddRange(posts);

        context.SaveChanges();
    }
}
