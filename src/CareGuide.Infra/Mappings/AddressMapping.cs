using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuide.Infra.Mappings
{
    public class AddressMapping : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("addresses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.PostalCode).IsRequired().HasMaxLength(20).HasColumnName("postal_code");
            builder.Property(x => x.Number).HasColumnName("number");
            builder.Property(x => x.Type).HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("type");
            builder.Property(x => x.City).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("city");
            builder.Property(x => x.Complement).HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("complement");
            builder.Property(x => x.Country).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("country");
            builder.Property(x => x.District).HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("district");
            builder.Property(x => x.ReferencePoint).HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("reference_point");
            builder.Property(x => x.State).IsRequired().HasMaxLength(100).HasColumnName("state");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true).HasColumnName("is_active");
        }
    }
}
