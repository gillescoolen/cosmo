using System.Collections.Generic;
using GalacticSpaceTransitAuthority;

namespace Cosmo.Presentation.Models.Ship
{
    public class HullEngineViewModel : ShipViewModel
    {
        public IEnumerable<Hull> Hulls;
        public IEnumerable<Engine> Engines;
    }
}