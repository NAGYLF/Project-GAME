using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace EphemeralApi.Models;

public partial class EphemeralCourageContext : DbContext
{
    public EphemeralCourageContext()
    {
    }

    public EphemeralCourageContext(DbContextOptions<EphemeralCourageContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Statistic> Statistics { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("achievement");

            entity.HasIndex(e => e.PlayerId, "PlayerId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.FirstBlood).HasDefaultValueSql("'0'");
            entity.Property(e => e.PlayerId).HasColumnType("int(11)");
            entity.Property(e => e.RookieWork).HasDefaultValueSql("'0'");
            entity.Property(e => e.YouAreOnYourOwnNow).HasDefaultValueSql("'0'");

            entity.HasOne(d => d.Player).WithMany(p => p.Achievements)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("achievement_ibfk_1");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("admin");

            entity.HasIndex(e => e.PlayerId, "PlayerId").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.PlayerId).HasColumnType("int(11)");

            entity.HasOne(d => d.Player).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.PlayerId)
                .HasConstraintName("admin_ibfk_1");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("player");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
        });

        modelBuilder.Entity<Statistic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("statistic");

            entity.HasIndex(e => e.PlayerId, "PlayerId");

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.DeathCount)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.EnemiesKilled)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.PlayerId).HasColumnType("int(11)");
            entity.Property(e => e.Score)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");

            entity.HasOne(d => d.Player).WithMany(p => p.Statistics)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("statistic_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
