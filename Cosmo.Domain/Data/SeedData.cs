using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Cosmo.Domain.Data
{
    public static class SeedData
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (context.Users.Any()) return;

            var roleStore = new RoleStore<IdentityRole>(context);
            var administrator = new IdentityRole("Administrator");
            var visitor = new IdentityRole("Visitor");

            administrator.NormalizedName = "Administrator";
            visitor.NormalizedName = "Visitor";

            await context.Roles.AddAsync(administrator);
            await context.Roles.AddAsync(visitor);

            var user = new User
            {
                UserName = "Admin",
                NormalizedUserName = "Admin",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, "password");

            await context.Users.AddAsync(user);

            var userRole = new IdentityUserRole<string>
            {
                RoleId = administrator.Id,
                UserId = user.Id
            };

            await context.UserRoles.AddAsync(userRole);

            context.SaveChanges();
        }
    }
}