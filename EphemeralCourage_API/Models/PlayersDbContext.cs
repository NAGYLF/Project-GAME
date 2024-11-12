using Microsoft.EntityFrameworkCore;

namespace EphemeralCourage_API.Models
{
    public class PlayersDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string conn = "server=localhost; database=EphemeralCourage_DataBase; user=root; password=";

                optionsBuilder.UseMySQL(conn);
            }
        }
        public DbSet<Player> NewPlayer { get; set; } = null!;
    }
}
