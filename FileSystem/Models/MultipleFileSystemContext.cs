using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FileSystem.Models;

public partial class MultipleFileSystemContext : DbContext
{
    public MultipleFileSystemContext()
    {
    }

    public MultipleFileSystemContext(DbContextOptions<MultipleFileSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>(entity =>
        {
            entity.Property(e => e.ContentType)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FilePath)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.Documents)
                .HasForeignKey(d => d.Uid)
                .HasConstraintName("FK_Documents_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
