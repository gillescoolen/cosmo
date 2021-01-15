using System.ComponentModel.DataAnnotations;

namespace Cosmo.Presentation.Models.Account
{
    public class LoginViewModel
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Password { get; set; }
    }
}