using System;
using System.Collections.Generic;
using JessyProject.Models;
using Microsoft.EntityFrameworkCore;

namespace JessyProject.Data;

public partial class ClassementDbContext : DbContext
{
    public ClassementDbContext()
    {
    }

    public ClassementDbContext(DbContextOptions<ClassementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bouygue> Bouygues { get; set; }

    public virtual DbSet<Engie> Engies { get; set; }

    public virtual DbSet<Ohm> Ohms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_URL"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bouygue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Bouygues_pkey");

            entity.ToTable("Bouygues", "c2e");

            entity.HasIndex(e => e.Num_contrat, "Bouygues_Num_contrat_key").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Bouygues_Id_seq\"'::regclass)");
            entity.Property(e => e.Client).HasMaxLength(255);
            entity.Property(e => e.Date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Equipe).HasMaxLength(100);
            entity.Property(e => e.Num_contrat).HasMaxLength(50);
            entity.Property(e => e.Produite).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Vendeur).HasMaxLength(100);
        });

        modelBuilder.Entity<Engie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Engie_pkey");

            entity.ToTable("Engie", "c2e");

            entity.HasIndex(e => e.Num_contrat, "Engie_Num_contrat_key").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Engie_Id_seq\"'::regclass)");
            entity.Property(e => e.Client).HasMaxLength(255);
            entity.Property(e => e.Date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Equipe).HasMaxLength(100);
            entity.Property(e => e.Num_contrat).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);
            entity.Property(e => e.Vendeur).HasMaxLength(100);
        });

        modelBuilder.Entity<Ohm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Ohm_pkey");

            entity.ToTable("Ohm", "c2e");

            entity.HasIndex(e => e.Num_contrat, "Ohm_Num_contrat_key").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("nextval('\"Ohm_Id_seq\"'::regclass)");
            entity.Property(e => e.Client).HasMaxLength(255);
            entity.Property(e => e.Date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Equipe).HasMaxLength(100);
            entity.Property(e => e.Num_contrat).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Vendeur).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
