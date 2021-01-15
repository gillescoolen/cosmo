#nullable enable
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using GalacticSpaceTransitAuthority;

namespace Domain.Services
{
    public class RulesValidationService : IRulesValidationService
    {
        private readonly ISpaceTransitAuthority spaceTransitAuthority;
        private readonly List<Weapon> weapons = new List<Weapon>();
        private readonly List<Wing> wings = new List<Wing>();
        private Hull? hull;
        private Engine? engine;

        public RulesValidationService(ISpaceTransitAuthority spaceTransitAuthority)
        {
            this.spaceTransitAuthority = spaceTransitAuthority;
        }

        public string? Initialize(IEnumerable<WingWithWeapons> configuredWings, int hullId, int engineId)
        {
            var wings = configuredWings.ToList();
            if (wings.ToList().Count % 2 != 0) return "You must equip an even amount of wings!";
            
            var engineMessage = EngineExists(engineId);
            if (engineMessage != null) return engineMessage;

            var hullMessage = HullExists(hullId);
            if (hullMessage != null) return hullMessage;

            foreach (var wing in wings)
            {
                foreach (var message in wing.WeaponIds.Select(WeaponExists).Where(m => m != null))
                    return message;

                var wingMessage = WingExists(wing.WingId);
                if (wingMessage != null) return wingMessage;

                var nullifierMessage = HasNullifier(wing);
                if (nullifierMessage != null) return nullifierMessage;

                var wingCapacityMessage = CheckExceedsWingCapacity(wing);
                if (wingCapacityMessage != null) return wingCapacityMessage;

                weapons.AddRange(spaceTransitAuthority.GetWeapons().Where(w => wing.WeaponIds.Contains(w.Id)).ToList());
                this.wings.Add(spaceTransitAuthority.GetWings().First<Wing>(w => w.Id == wing.WingId));
            }

            engine = spaceTransitAuthority.GetEngines().First(e => e.Id == engineId);
            hull = spaceTransitAuthority.GetHulls().First(h => h.Id == hullId);

            return null;
        }
        public double CalculateEnergyUsed()
        {
            var energy = 0.0;

            weapons.ForEach(w =>
            {
                var sameWeapons = weapons.Count(countWeapon => countWeapon.DamageType == w.DamageType);
                energy += sameWeapons >= 2 ? w.EnergyDrain * 0.8 : w.EnergyDrain;
            });

            return energy;
        }

        public double CalculateEnergyAvailable()
        {
            return wings.Sum(w => w.Energy) + engine!.Energy;
        }

        public double CalculateWeight()
        {
            double weight = 0;
            weight += engine!.Weight;
            weight += wings.Sum(w => w.Weight);

            weapons.ForEach(w =>
            {
                var twoStatisWeaponsUsed = weapons.Count(countWeapon => countWeapon.DamageType == DamageTypeEnum.Statis);
                weight += twoStatisWeaponsUsed >= 2 ? w.Weight * 0.85 : w.Weight;
            });

            return weight;
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

        public string? HasNullifier(WingWithWeapons configuredWing)
        {
            var wingWeapons = spaceTransitAuthority.GetWeapons().Where(w => configuredWing.WeaponIds.Contains(w.Id))
                .ToList();
            var hasNullifier = wingWeapons.FirstOrDefault(w => w.Name == "Nullifier");

            return wingWeapons.Count == 1 && hasNullifier != null
                ? "The nullifier weapon cannot be on a wing unaccompanied"
                : null;
        }

        public string? CheckExceedsWingCapacity(WingWithWeapons configuredWing)
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
            return CalculateWeight() > spaceTransitAuthority.CheckActualHullCapacity(hull)
                ? $"The selected hull {hull!.Name} cannot support the current weight"
                : null;
        }

        public string? CheckEnergyExceeded()
        {
            return CalculateEnergyUsed() > engine!.Energy
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
    }
}