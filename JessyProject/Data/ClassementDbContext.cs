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
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ClassementDB;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bouygue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07E23E7070");

            entity.Property(e => e.Client)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.NumContrat)
                .HasMaxLength(50)
                .HasColumnName("Num_contrat");
            entity.Property(e => e.Produite)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Vendeur)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Equipe)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        modelBuilder.Entity<Engie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07F4414995");

            entity.ToTable("Engie");

            entity.Property(e => e.Client)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.NumContrat)
                .HasMaxLength(50)
                .HasColumnName("Num_contrat");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Vendeur)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Equipe)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        modelBuilder.Entity<Ohm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07AAD91352");

            entity.ToTable("Ohm");

            entity.Property(e => e.Client)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.NumContrat)
                .HasMaxLength(50)
                .HasColumnName("Num_contrat");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Vendeur)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Equipe)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
