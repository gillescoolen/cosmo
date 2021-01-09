using System.ComponentModel.DataAnnotations;

namespace Cosmo.Presentation.Models.Account
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(100)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
    }
}