using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareGuide.Infra.Mappings
{
    public class AddressMapping : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            var typeConverter = new ValueConverter<AddressType, string>(x => ConvertTypeToString(x), x => ConvertStringToType(x));

            builder.ToTable("addresses", x =>
            {
                x.HasCheckConstraint("CK_Address_Type", "type IN ('CLI','URG','OTH')");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.PostalCode).IsRequired().HasMaxLength(20).HasColumnName("postal_code");
            builder.Property(x => x.Number).HasColumnName("number");
            builder.Property(x => x.Type).IsRequired().HasMaxLength(3).HasColumnName("type").HasConversion(typeConverter);
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

        private static string ConvertTypeToString(AddressType type)
        {
            return type switch
            {
                AddressType.Clinic => "CLI",
                AddressType.UrgencyEmergency => "URG",
                AddressType.Other => "OTH",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }

        private static AddressType ConvertStringToType(string typeStr)
        {
            return typeStr switch
            {
                "CLI" => AddressType.Clinic,
                "URG" => AddressType.UrgencyEmergency,
                "OTH" => AddressType.Other,
                _ => throw new ArgumentOutOfRangeException(nameof(typeStr), typeStr, null),
            };
        }
    }
}
