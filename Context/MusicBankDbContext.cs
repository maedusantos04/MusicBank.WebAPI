using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MusicBank.WebAPI.Models;

namespace MusicBank.WebAPI.Context;

public partial class MusicBankDbContext : DbContext
{
    public MusicBankDbContext()
    {
    }

    public MusicBankDbContext(DbContextOptions<MusicBankDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<GeneroMusical> GeneroMusicals { get; set; }

    public virtual DbSet<Musica> Musicas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MusicBankDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GeneroMusical>(entity =>
        {
            entity.HasKey(e => e.IdGeneroMusical).HasName("PK__GeneroMu__9E64F6D0FCF723A8");

            entity.ToTable("GeneroMusical");

            entity.HasIndex(e => e.GeneroMusical1, "UQ__GeneroMu__1FB4641430D036CB").IsUnique();

            entity.Property(e => e.GeneroMusical1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("GeneroMusical");
        });

        modelBuilder.Entity<Musica>(entity =>
        {
            entity.HasKey(e => e.IdMusica).HasName("PK__Musica__F026ACA77E40E0EA");

            entity.ToTable("Musica");

            entity.Property(e => e.ClipeMusica).IsUnicode(false);
            entity.Property(e => e.ImagemCapa).IsUnicode(false);
            entity.Property(e => e.NomeArtista)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.NomeMusica)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasMany(d => d.IdGeneroMusicals).WithMany(p => p.IdMusicas)
                .UsingEntity<Dictionary<string, object>>(
                    "MusicaGenero",
                    r => r.HasOne<GeneroMusical>().WithMany()
                        .HasForeignKey("IdGeneroMusical")
                        .HasConstraintName("FK_GeneroMusical"),
                    l => l.HasOne<Musica>().WithMany()
                        .HasForeignKey("IdMusica")
                        .HasConstraintName("FK_MusicaMusicaGenero"),
                    j =>
                    {
                        j.HasKey("IdMusica", "IdGeneroMusical");
                        j.ToTable("MusicaGenero");
                    });
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF97FD600C50");

            entity.ToTable("Usuario", tb => tb.HasTrigger("TR_UsuarioSoftDelete"));

            entity.HasIndex(e => e.Email, "UQ__Usuario__A9D105347BD4CADB").IsUnique();

            entity.Property(e => e.Ativo).HasDefaultValue(true);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Senha)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TipoUsuario)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
