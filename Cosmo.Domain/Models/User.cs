using Microsoft.AspNetCore.Identity;

namespace Cosmo.Domain
{
    public class User : IdentityUser
    {
        [PersonalData]
        public string License { get; set; }
    }
}