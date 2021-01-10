using System.Collections.Generic;
using System.Linq;
using GalacticSpaceTransitAuthority;

namespace Domain.Services
{
    public class CalculationService : ICalculationService
    {
        public double CalculateEnergyUsed(List<Weapon> weapons)
        {
            var energy = 0.0;

            weapons.ForEach(w =>
            {
                var sameWeaponsUsed = weapons.Count(countWeapon => countWeapon.DamageType == w.DamageType);
                energy += sameWeaponsUsed >= 2 ? w.EnergyDrain * 0.8 : w.EnergyDrain;
            });

            return energy;
        }
        
        public double CalculateEnergyAvailable(List<Wing> wings, Engine engine)
        {
            return wings.Sum(w => w.Energy) + engine.Energy;
        }

        public double CalculateWeight(List<Wing> wings, List<Weapon> weapons, Engine engine)
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
    }
}