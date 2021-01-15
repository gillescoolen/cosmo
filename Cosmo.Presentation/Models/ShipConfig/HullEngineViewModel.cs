using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Models;
using Domain.Services;
using GalacticSpaceTransitAuthority;

namespace Cosmo.Presentation.Models.ShipConfig
{
    public class HullEngineViewModel : IValidatableObject
    {
        public IEnumerable<Hull> Hulls { get; set; } = new List<Hull>();
        public IEnumerable<Engine> Engines { get; set; } = new List<Engine>();

        [Required]
        public int HullId { get; set; }
        
        [Required]
        public int EngineId { get; set; }

        [Required]
        [Range(2, 8)]
        [RegularExpression(@"^\d*[02468]$", ErrorMessage = "You need an even amount of wings")]
        public int WingAmount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var service = (IRulesValidationService) validationContext.GetService(typeof(IRulesValidationService))!;
            var message = service.Initialize(new List<WingWithWeapons>(), HullId, EngineId);

            if (message != null)
                yield return new ValidationResult(message);
        }
    }
}