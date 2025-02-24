namespace EphemeralApi.Models.Dtos
{
    public class CreatePlayerDto
    {
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
    }

    public class UpdatePlayerDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public bool DevConsole { get; set; }
        public string Password { get; set; } = null!;
        public bool IsBanned { get; set; }
    }

    public class PlayerStatsDto
    {
        public int PlayerId { get; set; }

        public int DeathCount { get; set; }
        public int Score { get; set; }
        public int EnemiesKilled { get; set; }

        public bool FirstBlood { get; set; }
        public bool RookieWork { get; set; }
        public bool YouAreOnYourOwnNow { get; set; }
    }
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class DevConsoleUpdateDto
    {
        public bool DevConsole { get; set; }
    }


}
