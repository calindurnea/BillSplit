using BillSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillSplit.Persistance.Configurations;

internal class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(b => b.Email)
            .IsRequired()
            .HasMaxLength(254);
        builder.Property(b => b.Name)
            .IsRequired();
        builder.Property(b => b.PhoneNumber)
            .IsRequired()
            .HasColumnType("bigint");
        builder.Property(b => b.Password)
            .IsRequired();
        builder.Property(b => b.IsSuperUser)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(b => b.CreatedBy)
            .HasColumnType("bigint")
            .IsRequired(false);
        builder.Property(b => b.CreatedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(b => b.UpdatedBy)
            .HasColumnType("bigint")
            .IsRequired(false);
        builder.Property(b => b.UpdatedDate)
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(b => b.DeletedBy)
            .HasColumnType("bigint")
            .IsRequired(false);
        builder.Property(b => b.DeletedDate)
            .ValueGeneratedOnUpdateSometimes()
            .IsRequired(false);
    }
}
