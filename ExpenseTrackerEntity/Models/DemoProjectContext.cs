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

    public virtual DbSet<Budget> Budgets { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Freequency> Freequencies { get; set; }

    public virtual DbSet<Recurrence> Recurrences { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User ID = postgres;Password=Dishant@2002;Server=localhost;Port=5432;Database=DemoProject;Integrated Security=true;Pooling=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(e => e.BudgetId).HasName("Budget_pkey");

            entity.ToTable("Budget");

            entity.Property(e => e.BudgetId).HasColumnName("Budget_Id");
            entity.Property(e => e.CategroryId).HasColumnName("Categrory_Id");
            entity.Property(e => e.CreatedBy).HasColumnName("Created_By");
            entity.Property(e => e.FrequenceyId).HasColumnName("Frequencey_Id");

            entity.HasOne(d => d.Categrory).WithMany(p => p.Budgets)
                .HasForeignKey(d => d.CategroryId)
                .HasConstraintName("Budget_Categrory_Id_fkey");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Budgets)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("Budget_Created_By_fkey");

            entity.HasOne(d => d.Frequencey).WithMany(p => p.Budgets)
                .HasForeignKey(d => d.FrequenceyId)
                .HasConstraintName("Budget_Frequencey_Id_fkey");
        });

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

        modelBuilder.Entity<Freequency>(entity =>
        {
            entity.HasKey(e => e.FreequencyId).HasName("Freequency_pkey");

            entity.ToTable("Freequency");

            entity.Property(e => e.FreequencyId)
                .ValueGeneratedNever()
                .HasColumnName("Freequency_Id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("Name ");
        });

        modelBuilder.Entity<Recurrence>(entity =>
        {
            entity.HasKey(e => e.RecurrenceId).HasName("Recurrence_pkey");

            entity.ToTable("Recurrence");

            entity.Property(e => e.RecurrenceId).HasColumnName("Recurrence_id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.CreatedBy).HasColumnName("Created_By");
            entity.Property(e => e.DueDate).HasColumnName("Due_Date");
            entity.Property(e => e.FreequencyId).HasColumnName("Freequency_Id");
            entity.Property(e => e.RecurrenceName)
                .HasColumnType("character varying")
                .HasColumnName("Recurrence_Name");
            entity.Property(e => e.StartDate).HasColumnName("Start_Date");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Recurrences)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("Recurrence_Created_By_fkey");

            entity.HasOne(d => d.Freequency).WithMany(p => p.Recurrences)
                .HasForeignKey(d => d.FreequencyId)
                .HasConstraintName("Recurrence_Freequency_Id_fkey");
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
