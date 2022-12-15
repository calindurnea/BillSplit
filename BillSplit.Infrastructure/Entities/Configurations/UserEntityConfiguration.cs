using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BillSplit.Infrastructure.Entities.Configurations;

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
            .HasColumnType("bigint");
        builder.Property(b => b.CreatedDate)
            .HasDefaultValueSql("getutcdate()")
            .IsRequired();

        builder.Property(b => b.UpdatedBy)
            .HasColumnType("bigint")
            .IsRequired();
        builder.Property(b => b.UpdatedDate)
            .HasColumnType("timestamp")
            .ValueGeneratedOnUpdate()
            .HasDefaultValueSql("getutcdate()")
            .IsRequired();

        builder.Property(b => b.DeletedBy)
            .HasColumnType("bigint");
        builder.Property(b => b.DeletedDate)
            .HasColumnType("timestamp");
    }
}
