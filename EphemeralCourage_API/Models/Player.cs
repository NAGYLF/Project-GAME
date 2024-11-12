using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EphemeralCourage_API.Models
{

    public class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] [Key] public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}