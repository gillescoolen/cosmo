using System.ComponentModel.DataAnnotations;

namespace Cosmo.Presentation.Models.Account
{
    public class InviteViewModel
    {

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"[ABCZ]")]
        public string License { get; set; }
    }
}