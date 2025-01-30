using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EphemeralCourage_API.Models;

public partial class EphemeralcourageDatabaseContext : DbContext
{
    public EphemeralcourageDatabaseContext()
    {
    }

    public EphemeralcourageDatabaseContext(DbContextOptions<EphemeralcourageDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<Newplayer> Newplayers { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Statistic> Statistics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;database=ephemeralcourage_database;user=root;password=;sslmode=none;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("achievements");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.PlayerId).HasColumnType("int(11)");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Achievement)
                .HasForeignKey<Achievement>(d => d.Id)
                .HasConstraintName("fk_achievements_players");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("admins");

            entity.HasIndex(e => e.Id, "id");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasColumnType("text");
            entity.Property(e => e.Rang).HasColumnType("text");

            entity.HasOne(d => d.IdNavigation).WithMany()
                .HasForeignKey(d => d.Id)
                .HasConstraintName("admins_ibfk_1");
        });

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__efmigrationshistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<Newplayer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("newplayer");

            entity.Property(e => e.Id).HasColumnType("int(11)");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("players");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Email).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Name).HasDefaultValueSql("'NULL'");
            entity.Property(e => e.Password).HasDefaultValueSql("'NULL'");
        });

        modelBuilder.Entity<Statistic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("statistics");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DeathCount).HasColumnType("int(11)");
            entity.Property(e => e.EnemiesKilled).HasColumnType("int(11)");
            entity.Property(e => e.PlayerId).HasColumnType("int(11)");
            entity.Property(e => e.Score).HasColumnType("int(11)");
            entity.Property(e => e.TimePlayed).HasColumnType("int(11)");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Statistic)
                .HasForeignKey<Statistic>(d => d.Id)
                .HasConstraintName("fk_statistics_players");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
