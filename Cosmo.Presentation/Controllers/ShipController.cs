using Microsoft.AspNetCore.Mvc;
using GalacticSpaceTransitAuthority;
using Microsoft.AspNetCore.Authorization;
using Cosmo.Presentation.Models.Ship;
using System;
using System.Collections.Generic;

namespace Cosmo.Presentation.Controllers
{
    [Authorize(Roles = "Visitor")]
    public class ShipController : Controller
    {
        private readonly ISpaceTransitAuthority spaceTransitAuthority;

        public ShipController(ISpaceTransitAuthority spaceTransitAuthority)
        {
            this.spaceTransitAuthority = spaceTransitAuthority;
        }

        public IActionResult Index()
        {
            var model = new HullEngineViewModel
            {
                Engines = spaceTransitAuthority.GetEngines(),
                Hulls = spaceTransitAuthority.GetHulls(),
                EquippedWeapons = new List<int>(),
                EquippedWings = new List<int>()
            };

            return View(model);
        }

        public IActionResult Wings()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConfigureBase(ShipViewModel model)
        {
            var newModel = new WingsWeaponsViewModel
            {
                EquippedEngine = model.EquippedEngine,
                EquippedHull = model.EquippedHull,
                EquippedWeapons = model.EquippedWeapons,
                EquippedWings = model.EquippedWings,
                WingAmount = model.WingAmount,
                Wings = spaceTransitAuthority.GetWings(),
                Weapons = spaceTransitAuthority.GetWeapons(),
            };

            return View("Wings", newModel);
        }

        [HttpPost]
        public IActionResult ConfigureAccessoires(ShipViewModel model)
        {
            // if (ModelState.IsValid)
            // {
                
            // }

            return View("Wings", model);
        }
    }
}