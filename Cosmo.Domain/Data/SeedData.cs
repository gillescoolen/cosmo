using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cosmo.Domain.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            if (context.UserRoles.Any()) return;

            string[] roles = { "Administrator", "Visitor" };

            foreach (string role in roles)
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                if (!context.Roles.Any(r => r.Name == role)) roleStore.CreateAsync(new IdentityRole(role));
            }

            var user = new User
            {
                UserName = "Administrator",
                NormalizedUserName = "ADMIN",
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<User>();
                var hashed = password.HashPassword(user, "password");
                user.PasswordHash = hashed;

                var userStore = new UserStore<User>(context);
                var result = userStore.CreateAsync(user);

            }

            AssignRoles(serviceProvider, user.Id, roles);
            
            context.SaveChanges();
        }

        public static async Task<IdentityResult> AssignRoles(IServiceProvider services, string id, string[] roles)
        {
            UserManager<User> _userManager = services.GetService<UserManager<User>>();
            User user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.AddToRolesAsync(user, roles);
            return result;
        }
    }
}