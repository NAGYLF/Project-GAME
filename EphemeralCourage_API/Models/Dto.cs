namespace EphemeralCourage_API.Models
{
    public class Dto
    {
        public record CreatePlayer(string Name, string Email, string Passworld,bool IsAdmin);
        public record UpdatePlayer(string Name, string Email, string Passworld,bool IsAdmin);
    }
}
