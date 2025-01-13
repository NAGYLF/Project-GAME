using Microsoft.EntityFrameworkCore;

namespace EphemeralCourage_API.Models
{
    public class DatasDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string conn = "server=localhost; database=EphemeralCourage_DataBase; user=root; password=";

                optionsBuilder.UseMySQL(conn);
            }
        }
        public DbSet<Player> Players { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
    }
}