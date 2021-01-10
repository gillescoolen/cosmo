using System.Collections.Generic;
using GalacticSpaceTransitAuthority;

namespace Domain.Services
{
    public interface ICalculationService
    {
        double CalculateEnergyUsed(List<Weapon> weapons);
        double CalculateEnergyAvailable(List<Wing> wings, Engine engine);
        double CalculateWeight(List<Wing> wings, List<Weapon> weapons, Engine engine);
    }
}