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

    public virtual DbSet<Folder> Folders { get; set; }

    public virtual DbSet<Icon> Icons { get; set; }

    public virtual DbSet<IconCategory> IconCategories { get; set; }

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
        modelBuilder.Entity<Folder>(entity =>
        {
            entity.HasKey(e => e.IdFolder);

            entity.Property(e => e.IdFolder).HasColumnName("id_folder");
            entity.Property(e => e.FolderName)
                .HasMaxLength(100)
                .HasColumnName("folder_name");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
        });

        modelBuilder.Entity<Icon>(entity =>
        {
            entity.HasKey(e => e.IdIcon);

            entity.Property(e => e.IdIcon).HasColumnName("id_icon");
            entity.Property(e => e.IconName)
                .HasMaxLength(100)
                .HasColumnName("icon_name");
            entity.Property(e => e.IdIconCategory).HasColumnName("id_icon_category");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.SvgCode).HasColumnName("svg_code");

            entity.HasOne(d => d.IdIconCategoryNavigation).WithMany(p => p.Icons)
                .HasForeignKey(d => d.IdIconCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Icons_Icon_category");
        });

        modelBuilder.Entity<IconCategory>(entity =>
        {
            entity.HasKey(e => e.IdCategory);

            entity.ToTable("Icon_category");

            entity.Property(e => e.IdCategory).HasColumnName("id_category");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
        });

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
            entity.Property(e => e.IdFolder).HasColumnName("id_folder");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IsArchive).HasColumnName("isArchive");
            entity.Property(e => e.IsFavorite).HasColumnName("isFavorite");
            entity.Property(e => e.Nonce)
                .HasMaxLength(12)
                .HasColumnName("nonce");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.SchemaVersion).HasColumnName("schema_version");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(255)
                .HasColumnName("service_name");
            entity.Property(e => e.SvgIcon).HasColumnName("svg_icon");
            entity.Property(e => e.Url).HasColumnName("url");

            entity.HasOne(d => d.IdFolderNavigation).WithMany(p => p.Secrets)
                .HasForeignKey(d => d.IdFolder)
                .HasConstraintName("FK_Secrets_Folders");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
