using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Cosmo.Domain;
using Cosmo.Presentation.Models.Account;
using System;

namespace Cosmo.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public ViewResult Login()
        {
            return View();
        }

        public ViewResult Register()
        {
            return View();
        }

        public ViewResult Manage()
        {
            return View();
        }

        public ViewResult Invite()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, true, false);

            var user = await userManager.FindByNameAsync(model.Username);

            if (result.Succeeded) return View("Invite");
            else ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(LoginViewModel model)
        {
            var password = new PasswordHasher<User>();
            var user = new User
            {
                UserName = model.Username,
                NormalizedUserName = model.Username.ToUpper()
            };

            var hashed = password.HashPassword(user, model.Password);
            user.PasswordHash = hashed;

            await userManager.CreateAsync(user);

            await userManager.AddToRoleAsync(user, "Administrator");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Invite(InviteViewModel model)
        {
            var user = new User
            {
                UserName = model.Username,
                NormalizedUserName = model.Username.ToUpper(),
                License = model.License
            };

            model.Password = Guid.NewGuid().ToString("n").Substring(0, 8);

            await userManager.CreateAsync(user, model.Password);
            await userManager.AddToRoleAsync(user, "Visitor");

            return View("Invitation", model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return View("Logout");
        }
    }
}
