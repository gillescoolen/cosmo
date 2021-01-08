using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Cosmo.Domain;
using Cosmo.Presentation.Models.Manager;
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

        public ViewResult ManagerLogin()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ManagerLogin(LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, true, false);

            if (result.Succeeded) return LocalRedirect("/");
            else ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateVisitor(LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, true, false);

            if (result.Succeeded) return LocalRedirect("/");
            else ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
