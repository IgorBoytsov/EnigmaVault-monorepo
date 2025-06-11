using System;
using System.Collections.Generic;
using EnigmaVault.SecretService.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnigmaVault.SecretService.Infrastructure.Data;

public partial class SecretDBContext : DbContext
{
    public SecretDBContext()
    {
    }

    public SecretDBContext(DbContextOptions<SecretDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Secret> Secrets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Secret>(entity =>
        {
            entity.HasKey(e => e.IdSecret);

            entity.Property(e => e.IdSecret).HasColumnName("id_secret");
            entity.Property(e => e.DateAdded)
                .HasColumnType("datetime")
                .HasColumnName("date_added");
            entity.Property(e => e.DateUpdate)
                .HasColumnType("datetime")
                .HasColumnName("date_update");
            entity.Property(e => e.EncryptedData).HasColumnName("encrypted_data");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IsFavorite).HasColumnName("isFavorite");
            entity.Property(e => e.Nonce)
                .HasMaxLength(12)
                .HasColumnName("nonce");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.SchemaVersion).HasColumnName("schema_version");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(255)
                .HasColumnName("service_name");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
