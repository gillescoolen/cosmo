using Microsoft.AspNetCore.Mvc;
using GalacticSpaceTransitAuthority;
using Microsoft.AspNetCore.Authorization;
using Cosmo.Presentation.Models.Ship;

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
                Hulls = spaceTransitAuthority.GetHulls()
            };

            return View(model);
        }
    }
}