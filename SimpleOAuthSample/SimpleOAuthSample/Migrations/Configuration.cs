namespace SimpleOAuthSample.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using SimpleOAuthSample.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SimpleOAuthSample.Models.OAuthDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SimpleOAuthSample.Models.OAuthDbContext context)
        {
            context.Clients.AddOrUpdate(
                client => client.Name,
                new Client
                {
                    Id = "42ff5dad3c274c97a3a7c3d44b67bb42",
                    Name = "Demo Resource Owner Password Credentials Grant Client",
                    ClientSecretHash = new PasswordHasher().HashPassword("client123456"),
                    AllowedGrant = AllowedGrant.ResourceOwnerGrant,
                    CreatedOn = DateTimeOffset.UtcNow
                });

            context.Users.AddOrUpdate(
                user => user.UserName,
                new IdentityUser("Tugberk")
                {
                    Id = Guid.NewGuid().ToString("N"),
                    PasswordHash = new PasswordHasher().HashPassword("user123456"),
                    Email = "tugberk@example.com",
                    EmailConfirmed = true
                });
        }
    }
}
