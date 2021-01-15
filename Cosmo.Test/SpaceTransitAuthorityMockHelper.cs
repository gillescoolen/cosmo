using System.Collections.Generic;
using GalacticSpaceTransitAuthority;
using Moq;

namespace Cosmo.Test
{
    public class SpaceTransitAuthorityMockHelper
    {
        public Mock<ISpaceTransitAuthority> SpaceTransitAuthorityMock { get; set; }

        public SpaceTransitAuthorityMockHelper()
        {
            SpaceTransitAuthorityMock = new Mock<ISpaceTransitAuthority>();
        }

        public Mock<ISpaceTransitAuthority> Initialize()
        {
            Setup();
            return SpaceTransitAuthorityMock;
        }

        private void Setup()
        {
            SpaceTransitAuthorityMock.Setup(s => s.GetEngines()).Returns(new List<Engine>
            {
                new Engine
                {
                    Energy = 10,
                    Id = 1,
                    Name = "Engine 1",
                    Weight = 5
                },
            });

            SpaceTransitAuthorityMock.Setup(s => s.GetWeapons()).Returns(new List<Weapon>
            {
                new Weapon
                {
                    Id = 1,
                    Name = "Weapon 1",
                    Weight = 10,
                    DamageType = DamageTypeEnum.Cold
                },
            });

            SpaceTransitAuthorityMock.Setup(s => s.GetHulls()).Returns(new List<Hull>
            {
                new Hull
                {
                    Id = 1,
                    Name = "Hull 1",
                    Agility = 10,
                    Speed = 10,
                    ColdShielding = 10,
                    HeatShielding = 10,
                    DefaultMaximumTakeOffMass = TakeOffMassEnum.Interceptor
                },
            });

            SpaceTransitAuthorityMock.Setup(s => s.GetWings()).Returns(new List<Wing>
            {
                new Wing
                {
                    Id = 1,
                    Name = "Wing 1",
                    Agility = 10,
                    Speed = 10,
                    Energy = 10,
                    Weight = 10,
                    NumberOfHardpoints = 2
                },
                new Wing
                {
                    Id = 2,
                    Name = "Wing 2",
                    Agility = 10,
                    Speed = 10,
                    Energy = 10,
                    Weight = 10,
                    NumberOfHardpoints = 1
                }
            });
        }
    }
}