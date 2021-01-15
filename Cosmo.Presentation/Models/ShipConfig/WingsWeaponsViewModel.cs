using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cosmo.Domain;
using Domain.Models;
using Domain.Services;
using GalacticSpaceTransitAuthority;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cosmo.Presentation.Models.ShipConfig
{
    public class WingsWeaponsViewModel : HullEngineViewModel, IValidatableObject
    {
        public IEnumerable<Wing> Wings { get; set; }
        public IEnumerable<SelectListItem> WingsSelectList { get; set; }
        public IEnumerable<Weapon> Weapons { get; set; }
        
        [Required]
        public List<WingWithWeapons> WingsWithWeapons { get; set; }

        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var service = (IRulesValidationService) validationContext.GetService(typeof(IRulesValidationService));
            var httpContext = (IHttpContextAccessor) validationContext.GetService(typeof(IHttpContextAccessor));
            var message = service.Initialize(WingsWithWeapons, HullId, EngineId);

            if (message != null)
            {
                yield return new ValidationResult(message);
                yield break;
            }

            var claim = httpContext!.HttpContext?.User.FindFirst("License");
            var value = (int)Enum.Parse(typeof(License), claim.Value);

            if (claim != null && service.CalculateWeight() > value)
            {
                yield return new ValidationResult($"License is authorized up to {value.ToString()} KG. Please lower your weight to match your license.");
                yield break;
            }

            var messages = new List<string>
            {
                service.CheckCombinations(),
                service.CheckWeightExceeded(),
                service.CheckEnergyExceeded(),
                service.CheckExplosionDanger(),
                service.CheckExceededKineticDifference()
            };

            messages.RemoveAll(m => m == null);
            
            foreach (var m in messages)
            {
                yield return new ValidationResult(m);
            }
        }
    }
}