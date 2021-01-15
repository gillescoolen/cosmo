using GalacticSpaceTransitAuthority;

namespace Cosmo.Presentation.Models.ShipConfig
{
    public class OverviewViewModel : WingsWeaponsViewModel
    {
        public Ship Ship { get; set; }
        public double TotalWeight { get; set; }
        public double TotalEnergy { get; set; }
    }
}