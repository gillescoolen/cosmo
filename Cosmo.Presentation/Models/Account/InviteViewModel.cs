using System.ComponentModel.DataAnnotations;

namespace Cosmo.Presentation.Models.Account
{
    public class InviteViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"[ABCZ]")]
        public string License { get; set; }
    }
}