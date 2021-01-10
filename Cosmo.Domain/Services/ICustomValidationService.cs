using System.Collections.Generic;
using Domain.Models;

namespace Domain.Services
{
    public interface ICustomValidationService
    {
        string Initialize(IEnumerable<ConfiguredWing> configuredWings, int hullId, int engineId);
        string HullExists(int id);
        string EngineExists(int id);
        string WingExists(int id);
        string WeaponExists(int id);
        string HasNullifier(ConfiguredWing configuredWing);
        string CheckExceedsWingCapacity(ConfiguredWing configuredWing);
        string CheckCombinations();
        string CheckWeightExceeded();
        string CheckEnergyExceeded();
        string CheckExplosionDanger();
        string CheckExceededKineticDifference();
        double GetWeight();
        double GetEnergyAvailable();
        double GetEnergyUsed();
    }
}