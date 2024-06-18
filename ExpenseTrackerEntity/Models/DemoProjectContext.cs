using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerEntity.Models;

public partial class DemoProjectContext : DbContext
{
    public DemoProjectContext()
    {
    }

    public DemoProjectContext(DbContextOptions<DemoProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID=postgres;Password=Dishant@2002;Server=localhost;Port=5432;Database=DemoProject;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasIndex(e => e.CategoryId, "IX_Expenses_Category_Id");

            entity.HasIndex(e => e.UserId, "IX_Expenses_User_Id");

            entity.Property(e => e.ExpenseId).HasColumnName("Expense_Id");
            entity.Property(e => e.CategoryId).HasColumnName("Category_Id");
            entity.Property(e => e.Name).HasColumnType("character varying");
            entity.Property(e => e.UserId).HasColumnName("User_Id");

            entity.HasOne(d => d.Category).WithMany(p => p.Expenses).HasForeignKey(d => d.CategoryId);

            entity.HasOne(d => d.User).WithMany(p => p.Expenses).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnName("Created_At");
            entity.Property(e => e.UpdatedAt).HasColumnName("Updated_At");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
