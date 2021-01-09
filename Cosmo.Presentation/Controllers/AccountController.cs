using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Cosmo.Domain;
using Cosmo.Presentation.Models.Account;
using System;
using System.Security.Claims;

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

        [Authorize(Roles = "Administrator")]
        public ViewResult Invite()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, true, false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(model.Username);
                    var isAdministrator = await signInManager.UserManager.IsInRoleAsync(user, "Administrator");

                    return isAdministrator ? RedirectToAction("Invite", "Account") : RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Invite(InviteViewModel model)
        {
            if (!ModelState.IsValid) return View();

            var user = new User
            {
                UserName = model.Username,
                NormalizedUserName = model.Username,
            };

            var password = Guid.NewGuid().ToString("n").Substring(0, 8);

            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, "Visitor");
            await userManager.AddClaimAsync(user, new Claim("License", model.License));

            var invite = new InvitationViewModel
            {
                Username = model.Username,
                Password = password,
                License = model.License
            };

            return View("Invitation", invite);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
