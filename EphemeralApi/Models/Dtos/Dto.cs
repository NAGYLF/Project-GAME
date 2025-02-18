namespace EphemeralApi.Models.Dtos
{
    public class CreatePlayerDto
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsAdmin { get; set; }
    }
    public class UpdatePlayerDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public string Password { get; set; } = null!;
    }

    public class PlayerStatsDto
    {
        public int PlayerId { get; set; }

        // Statisztika
        public int DeathCount { get; set; }
        public int Score { get; set; }
        public int EnemiesKilled { get; set; }

        // Achievementek
        public bool FirstBlood { get; set; }
        public bool RookieWork { get; set; }
        public bool YouAreOnYourOwnNow { get; set; }
    }
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }


}
