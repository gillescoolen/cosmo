using System.Collections.Generic;
using GalacticSpaceTransitAuthority;

namespace Cosmo.Presentation.Models.Ship
{
    public class WingsWeaponsViewModel : ShipViewModel
    {
        public IEnumerable<Wing> Wings { get; set; }
        public IEnumerable<Weapon> Weapons { get; set; }
    }
}