#nullable enable
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using GalacticSpaceTransitAuthority;

namespace Domain.Services
{
    public class CustomValidationService : ICustomValidationService
    {
        private readonly ISpaceTransitAuthority spaceTransitAuthority;
        private readonly ICalculationService calculationService;
        private readonly List<Weapon> weapons = new List<Weapon>();
        private readonly List<Wing> wings = new List<Wing>();
        private Hull? hull;
        private Engine? engine;

        public CustomValidationService(ISpaceTransitAuthority spaceTransitAuthority, ICalculationService calculationService)
        {
            this.spaceTransitAuthority = spaceTransitAuthority;
            this.calculationService = calculationService;
        }

        public string? Initialize(IEnumerable<ConfiguredWing> configuredWings, int hullId, int engineId)
        {
            var wingList = configuredWings.ToList();
            if (wingList.ToList().Count % 2 != 0) return "You must equip an even amount of wings!";
            
            var engineMessage = EngineExists(engineId);
            if (engineMessage != null) return engineMessage;

            var hullMessage = HullExists(hullId);
            if (hullMessage != null) return hullMessage;

            foreach (var configuredWing in wingList)
            {
                foreach (var message in configuredWing.WeaponIds.Select(WeaponExists).Where(m => m != null))
                    return message;

                var wingMessage = WingExists(configuredWing.WingId);
                if (wingMessage != null) return wingMessage;

                var nullifierMessage = HasNullifier(configuredWing);
                if (nullifierMessage != null) return nullifierMessage;

                var wingCapacityMessage = CheckExceedsWingCapacity(configuredWing);
                if (wingCapacityMessage != null) return wingCapacityMessage;

                weapons.AddRange(spaceTransitAuthority.GetWeapons().Where(w => configuredWing.WeaponIds.Contains(w.Id)).ToList());
                wings.Add(spaceTransitAuthority.GetWings().First(w => w.Id == configuredWing.WingId));
            }

            engine = spaceTransitAuthority.GetEngines().First(e => e.Id == engineId);
            hull = spaceTransitAuthority.GetHulls().First(h => h.Id == hullId);

            return null;
        }

        public string? HullExists(int id)
        {
            return spaceTransitAuthority.GetHulls().FirstOrDefault(h => h.Id == id) == null
                ? $"Hull {id} does not exist"
                : null;
        }

        public string? EngineExists(int id)
        {
            var engine = spaceTransitAuthority.GetEngines().FirstOrDefault(e => e.Id == id) == null;

            return engine
                ? $"Engine {id} does not exist"
                : null;
        }

        public string? WingExists(int id)
        {
            return spaceTransitAuthority.GetWings().FirstOrDefault(w => w.Id == id) == null
                ? $"Wing {id} does not exist"
                : null;
        }

        public string? WeaponExists(int id)
        {
            return spaceTransitAuthority.GetWeapons().FirstOrDefault(w => w.Id == id) == null
                ? $"Weapon {id} does not exist"
                : null;
        }

        public string? HasNullifier(ConfiguredWing configuredWing)
        {
            var wingWeapons = spaceTransitAuthority.GetWeapons().Where(w => configuredWing.WeaponIds.Contains(w.Id))
                .ToList();
            var hasNullifier = wingWeapons.FirstOrDefault(w => w.Name == "Nullifier");

            return wingWeapons.Count == 1 && hasNullifier != null
                ? "The nullifier weapon cannot be on a wing unaccompanied"
                : null;
        }

        public string? CheckExceedsWingCapacity(ConfiguredWing configuredWing)
        {
            var wing = spaceTransitAuthority.GetWings().First(w => w.Id == configuredWing.WingId);
            return configuredWing.WeaponIds.Count > wing.NumberOfHardpoints
                ? $"Wing {wing.Name} only has {wing.NumberOfHardpoints} slot(s)"
                : null;
        }

        public string? CheckCombinations()
        {
            string? message = null;

            if (weapons.FirstOrDefault(w => w.DamageType == DamageTypeEnum.Heat) != null &&
                weapons.FirstOrDefault(w => w.DamageType == DamageTypeEnum.Cold) != null)
                message = "You cannot combine heat and cold weapons.";

            if (weapons.FirstOrDefault(w => w.DamageType == DamageTypeEnum.Statis) != null &&
                weapons.FirstOrDefault(w => w.DamageType == DamageTypeEnum.Gravity) != null)
                message = "You cannot combine stasis and gravity weapons.";

            return message;
        }

        public string? CheckWeightExceeded()
        {
            return GetWeight() > spaceTransitAuthority.CheckActualHullCapacity(hull)
                ? $"The selected hull {hull!.Name} cannot support the current weight"
                : null;
        }

        public string? CheckEnergyExceeded()
        {
            return GetEnergyUsed() > engine!.Energy
                ? $"Engine {engine.Name} does not supply enough power to support the selected weapons"
                : null;
        }

        public string? CheckExplosionDanger()
        {
            return engine!.Name == "Intrepid Class" && weapons.FirstOrDefault(w => w.Id == 9) != null
                ? "The selected engine Intrepid Class and weapon Imploder may implode, please choose another engine/weapon combination"
                : null;
        }

        public string? CheckExceededKineticDifference()
        {
            var kineticWeapons = weapons.Where(w => w.DamageType == DamageTypeEnum.Kinetic).ToList();

            switch (kineticWeapons.Count)
            {
                case 1 when kineticWeapons[0].EnergyDrain >= 35:
                    return "A single kinetic weapon cannot drain more than 34 energy";
                case 0:
                    return null;
            }

            var min = kineticWeapons.Min(k => k.EnergyDrain);
            var max = kineticWeapons.Max(k => k.EnergyDrain);

            return max - min >= 35
                ? "Kinetic energy difference between multiple kinetic weapons cannot exceed 34"
                : null;
        }

        public double GetWeight()
        {
            return calculationService.CalculateWeight(wings, weapons, engine);
        }

        public double GetEnergyAvailable()
        {
            return calculationService.CalculateEnergyAvailable(wings, engine);
        }
        
        public double GetEnergyUsed()
        {
            return calculationService.CalculateEnergyUsed(weapons);
        }
    }
}