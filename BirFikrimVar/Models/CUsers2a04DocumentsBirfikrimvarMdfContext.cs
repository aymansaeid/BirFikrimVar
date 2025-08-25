using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BirFikrimVar.Models;

public partial class CUsers2a04DocumentsBirfikrimvarMdfContext : DbContext
{
    public CUsers2a04DocumentsBirfikrimvarMdfContext()
    {
    }

    public CUsers2a04DocumentsBirfikrimvarMdfContext(DbContextOptions<CUsers2a04DocumentsBirfikrimvarMdfContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Idea> Ideas { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\2A04\\Documents\\BirFikrimVar.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCA395AFA82");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Idea).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdeaId)
                .HasConstraintName("FK_Comments_Ideas");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Users");
        });

        modelBuilder.Entity<Idea>(entity =>
        {
            entity.HasKey(e => e.IdeaId).HasName("PK__Ideas__FE218203C167977F");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LikeCount).HasDefaultValue(0);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.Ideas)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Ideas_Users");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__Likes__A2922C1447157838");

            entity.HasIndex(e => new { e.IdeaId, e.UserId }, "UQ_Likes").IsUnique();

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Idea).WithMany(p => p.Likes)
                .HasForeignKey(d => d.IdeaId)
                .HasConstraintName("FK_Likes_Ideas");

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Likes_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CEEBC16EE");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105344908B257").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
