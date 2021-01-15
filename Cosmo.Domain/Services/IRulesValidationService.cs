using System.Collections.Generic;
using Domain.Models;

namespace Domain.Services
{
    public interface IRulesValidationService
    {
        string Initialize(IEnumerable<WingWithWeapons> configuredWings, int hullId, int engineId);
        double CalculateEnergyUsed();
        double CalculateEnergyAvailable();
        double CalculateWeight();
        string HullExists(int id);
        string EngineExists(int id);
        string WingExists(int id);
        string WeaponExists(int id);
        string HasNullifier(WingWithWeapons configuredWing);
        string CheckExceedsWingCapacity(WingWithWeapons configuredWing);
        string CheckCombinations();
        string CheckWeightExceeded();
        string CheckEnergyExceeded();
        string CheckExplosionDanger();
        string CheckExceededKineticDifference();
    }
}