using System;
using System.Collections.Generic;
using BillSplit.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence;

public partial class BillsplitContext : DbContext
{
    public BillsplitContext()
    {
    }

    public BillsplitContext(DbContextOptions<BillsplitContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillGroup> BillGroups { get; set; }

    public virtual DbSet<BillSplit> BillSplits { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserBillGroup> UserBillGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:ApplicationContext");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.ToTable("Bill", "billsplit");

            entity.Property(e => e.Amount).HasPrecision(19, 4);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.BillGroup).WithMany(p => p.Bills)
                .HasForeignKey(d => d.BillGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Bill_BillGroup_Id_fk");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BillCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_Created_By_User_Id_fk");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.BillDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("User_Deleted_By_User__fk");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BillUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("User_Updated_By_User__fk");
        });

        modelBuilder.Entity<BillGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("BillGroup_pk");

            entity.ToTable("BillGroup", "billsplit");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.BillGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("BillGroup_Created_By_User_Id_fk");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.BillGroupDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("BillGroup_Deleted_By_User_Id_fk");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.BillGroupUpdatedByNavigations)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("BillGroup_Updated_By_User_Id_fk");
        });

        modelBuilder.Entity<BillSplit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Id");

            entity.ToTable("BillSplit", "billsplit");

            entity.HasOne(d => d.Bill).WithMany(p => p.BillSplits)
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BillSplit_Bill_Id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.BillSplits)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("BillSplit_User_Id_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User", "billsplit");

            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Email).HasMaxLength(254);
            entity.Property(e => e.UpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InverseCreatedByNavigation)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("User_Created_By_User_Id_fk");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.InverseDeletedByNavigation)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("User_Deleted_By_User__fk");

            entity.HasOne(d => d.UpdatedByNavigation).WithMany(p => p.InverseUpdatedByNavigation)
                .HasForeignKey(d => d.UpdatedBy)
                .HasConstraintName("User_Updated_By_User__fk");
        });

        modelBuilder.Entity<UserBillGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("UserBillGroup_pk");

            entity.ToTable("UserBillGroup", "billsplit");

            entity.Property(e => e.Id).UseIdentityAlwaysColumn();
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.BillGroup).WithMany(p => p.UserBillGroups)
                .HasForeignKey(d => d.BillGroupId)
                .HasConstraintName("UserBillGroup_Bill_Group_Id_fk");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.UserBillGroupCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("UserBillGroup_Created_By_User_Id_fk");

            entity.HasOne(d => d.DeletedByNavigation).WithMany(p => p.UserBillGroupDeletedByNavigations)
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("UserBillGroup_Deleted_By_User_Id_fk");

            entity.HasOne(d => d.User).WithMany(p => p.UserBillGroupUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserBillGroup_User_Id_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
