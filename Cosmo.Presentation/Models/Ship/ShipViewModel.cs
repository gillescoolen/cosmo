using System.Collections.Generic;
using GalacticSpaceTransitAuthority;

namespace Cosmo.Presentation.Models.Ship
{
    public class ShipViewModel
    {
        public Hull EquippedHull { get; set; }
        public List<Wing> EquippedWings { get; set; }
        public List<Weapon> EquippedWeapons { get; set; }
        public Engine EquippedEngine { get; set; }
    }
}