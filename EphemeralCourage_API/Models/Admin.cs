using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EphemeralCourage_API.Models
{ 
    public class Admin
    {
        [Key] public string? PlayerName { get; set; }
        public string? Entitlement { get; set; }
    }
}