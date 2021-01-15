using System.Collections.Generic;

namespace Domain.Models
{
    public class WingWithWeapons
    {
        public int WingId { get; set; }
        public List<int> WeaponIds { get; set; }
    }
}
