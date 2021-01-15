using Microsoft.AspNetCore.Mvc;
using GalacticSpaceTransitAuthority;
using Microsoft.AspNetCore.Authorization;
using Cosmo.Presentation.Models.ShipConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Domain.Services;
using Microsoft.AspNetCore.Identity;
using Cosmo.Domain;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Cosmo.Presentation.Controllers
{
    [Authorize(Roles = "Visitor")]
    public class ShipConfigController : Controller
    {
        private readonly ISpaceTransitAuthority spaceTransitAuthority;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        private readonly IRulesValidationService rulesValidationService;

        public ShipConfigController(ISpaceTransitAuthority spaceTransitAuthority, UserManager<User> userManager, SignInManager<User> signInManager, IRulesValidationService rulesValidationService)
        {
            this.spaceTransitAuthority = spaceTransitAuthority;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.rulesValidationService = rulesValidationService;
        }

        public IActionResult ConfigureBase()
        {
            var model = new HullEngineViewModel
            {
                Engines = spaceTransitAuthority.GetEngines(),
                Hulls = spaceTransitAuthority.GetHulls(),
            };

            return View(model);
        }

        public IActionResult ConfigureAccessoires(HullEngineViewModel model)
        {
            if (!TryValidateModel(model, nameof(HullEngineViewModel)))
                return RedirectToAction("ConfigureBase");

            var wings = new List<WingWithWeapons>();

            for (var i = 0; i < model.WingAmount; ++i)
            {
                var weapons = new List<int> { 1 };
                wings.Add(new WingWithWeapons { WingId = 1, WeaponIds = weapons });
            }

            return View(new WingsWeaponsViewModel
            {
                HullId = model.HullId,
                EngineId = model.EngineId,
                WingAmount = model.WingAmount,
                Weapons = spaceTransitAuthority.GetWeapons(),
                WingsSelectList = CreateSelectItem(spaceTransitAuthority.GetWings()),
                WingsWithWeapons = wings
            });
        }


        public IActionResult Overview(OverviewViewModel model)
        {
            if (model == null || !ModelState.IsValid)
                return RedirectToAction("ConfigureBase");

            return View(model);
        }

        public IActionResult Finished()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfigureBase(HullEngineViewModel model)
        {
            if (!ModelState.IsValid || model == null)
            {
                return View(new HullEngineViewModel
                {
                    Hulls = spaceTransitAuthority.GetHulls().ToList(),
                    Engines = spaceTransitAuthority.GetEngines().ToList(),
                    WingAmount = model.WingAmount
                });
            }

            return RedirectToAction("ConfigureAccessoires", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfigureAccessoires(WingsWeaponsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new WingsWeaponsViewModel
                {
                    HullId = model.HullId,
                    EngineId = model.EngineId,
                    WingAmount = model.WingsWithWeapons.Count,
                    Weapons = spaceTransitAuthority.GetWeapons(),
                    Wings = spaceTransitAuthority.GetWings(),
                    WingsWithWeapons = model.WingsWithWeapons,
                    WingsSelectList = CreateSelectItem(spaceTransitAuthority.GetWings())
                });
            }

            var overviewModel = new OverviewViewModel
            {
                Ship = CreateShip(model.HullId, model.EngineId, model.WingsWithWeapons),
                WingsWithWeapons = model.WingsWithWeapons,
                HullId = model.HullId,
                EngineId = model.EngineId,
                WingAmount = model.WingAmount,
                TotalEnergy = rulesValidationService.CalculateEnergyUsed(),
                TotalWeight = rulesValidationService.CalculateWeight()
            };

            return View("Overview", overviewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Finish(OverviewViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction("Overview", model);
            
            var ship = CreateShip(model.HullId, model.EngineId, model.WingsWithWeapons);

            var user = await userManager.GetUserAsync(User);

            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            await HttpContext.SignOutAsync();

            Response.Cookies.Delete(".AspNetCore.Cookies");

            var finishedModel = new FinishedViewModel
            {
                Code = spaceTransitAuthority.RegisterShip(JsonConvert.SerializeObject(ship))
            };

            return View("Finished", finishedModel);
        }

        public IEnumerable<SelectListItem> CreateSelectItem(IEnumerable<Wing> wings)
        {
            return wings.Select(weapon => new SelectListItem
                {
                    Text = $"{weapon.Name} - {weapon.Weight}KG - {weapon.NumberOfHardpoints} slot(s)",
                    Value = weapon.Id.ToString()
                }
            );
        }

        public Ship CreateShip(int hullId, int engineId, List<WingWithWeapons> wingsWithWeapons)
        {
            var ship = new Ship
            {
                Hull = spaceTransitAuthority.GetHulls().First(h => h.Id == hullId),
                Engine = spaceTransitAuthority.GetEngines().First(e => e.Id == engineId),
                Wings = new List<Wing>()
            };

            var wings = new List<Wing>();

            wingsWithWeapons.ForEach(c =>
            {
                var foundWing = spaceTransitAuthority.GetWings().First(a => a.Id == c.WingId);

                var wing = new Wing
                {
                    Id = foundWing.Id,
                    Agility = foundWing.Agility,
                    Energy = foundWing.Energy,
                    Hardpoint = new List<Weapon>(),
                    Name = foundWing.Name,
                    Speed = foundWing.Speed,
                    Weight = foundWing.Weight,
                    NumberOfHardpoints = foundWing.NumberOfHardpoints
                };

                wing.Hardpoint = spaceTransitAuthority.GetWeapons().Where(w => c.WeaponIds.Contains(w.Id)).ToList();
                
                wings.Add(wing);
            });

            ship.Wings = wings;

            return ship;
        }
    }
}