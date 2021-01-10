using System.Collections.Generic;
using GalacticSpaceTransitAuthority;

namespace Cosmo.Presentation.Models.Ship
{
    public class ShipViewModel
    {

        public int WingAmount { get; set; }
        public int EquippedHull { get; set; }
        public List<int> EquippedWings { get; set; }
        public List<int> EquippedWeapons { get; set; }
        public int EquippedEngine { get; set; }
    }
}