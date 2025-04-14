using System;

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

    public class AdminDto
    {
        public int Id { get; set; }
        public bool DevConsole { get; set; }
    }

    public class UpdatePlayerBanDto
    {
        public bool IsBanned { get; set; }
    }

    public class UpdateStatisticsDto
    {
        public int DeathCount { get; set; }
        public int Score { get; set; }
        public int EnemiesKilled { get; set; }
    }

    public class UpdateAchievementsDto
    {
        public bool FirstBlood { get; set; }
        public bool RookieWork { get; set; }
        public bool YouAreOnYourOwnNow { get; set; }
    }

    // Player DTO that holds basic player info
    public class PlayerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
    }

    // Statistics DTO that holds player's statistics
    public class StatisticsDto
    {
        public int DeathCount { get; set; }
        public int Score { get; set; }
        public int EnemiesKilled { get; set; }
    }

    // Achievements DTO that holds player's achievements
    public class AchievementsDto
    {
        public bool FirstBlood { get; set; }
        public bool RookieWork { get; set; }
        public bool YouAreOnYourOwnNow { get; set; }
    }

    // DTO that combines Player, Statistics, Achievements, and Admin
    public class PlayerAllDto
    {
        public PlayerDto Player { get; set; } = null!;
        public StatisticsDto? Statistics { get; set; }
        public AchievementsDto? Achievements { get; set; }
        public AdminDto? Admin { get; set; }
    }
}
