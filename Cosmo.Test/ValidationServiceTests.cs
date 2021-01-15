using System.Collections.Generic;
using Domain.Models;
using Domain.Services;
using Moq;
using NUnit.Framework;

namespace Cosmo.Test
{
    [TestFixture]
    public class ValidationServiceTests
    {
        private RulesValidationService validationService;
        private List<WingWithWeapons> configuredWings;

        [SetUp]
        public void SetUp()
        {
            var spaceTransitAuthority = new SpaceTransitAuthorityMockHelper().Initialize();
            var calculationService = new Mock<RulesValidationService>();
            
            validationService = new RulesValidationService(spaceTransitAuthority.Object);

            configuredWings = new List<WingWithWeapons>
            {
                new WingWithWeapons
                {
                    WingId = 1,
                    WeaponIds = new List<int> { 1, 1 }
                },
                new WingWithWeapons
                {
                    WingId = 2,
                    WeaponIds = new List<int> { 1 }
                }
            };

            validationService.Initialize(configuredWings, 1, 1);
        }

        [Test(Description = "Initialize with valid data.")]
        public void TestInitialization_ValidData_ShouldReturnNull()
        {
            var response = validationService.Initialize(configuredWings, 1, 1);

            Assert.AreEqual(null, response);
        }
        
        [Test(Description = "Initialize service with non existing hull.")]
        public void TestInitialization_InvalidHull_ShouldReturnMessage()
        {
            var response = validationService.Initialize(configuredWings, 99, 1);

            Assert.AreEqual("Hull 99 does not exist.", response);
        }
        
        [Test(Description = "Initialize service with non existing engine.")]
        public void TestInitialization_InvalidEngine_ShouldReturnMessage()
        {
            var response = validationService.Initialize(configuredWings, 1, 99);

            Assert.AreEqual("Engine 99 does not exist.", response);
        }
        
        [Test(Description = "Initialize service with uneven amount of wings.")]
        public void TestInitialization_UnevenAmountOfWings_ShouldReturnMessage()
        {
            var singleWing = new List<WingWithWeapons> { configuredWings[0] };
            var response = validationService.Initialize(singleWing, 1, 1);

            Assert.AreEqual("You must equip an even amount of wings!", response);
        }
    }
}