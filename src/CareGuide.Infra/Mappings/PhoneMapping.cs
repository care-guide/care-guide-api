using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareGuide.Infra.Mappings
{
    public class PhoneMapping : IEntityTypeConfiguration<Phone>
    {
        public void Configure(EntityTypeBuilder<Phone> builder)
        {
            var phoneTypeConverter = new ValueConverter<PhoneType, string>(x => x.ToString(), x => (PhoneType)Enum.Parse(typeof(PhoneType), x));

            builder.ToTable("phones", x =>
            {
                x.HasCheckConstraint("CK_Phone_Type", "type IN ('R', 'COM', 'CEL', 'O')");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Number).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("number");
            builder.Property(x => x.AreaCode).IsRequired().HasMaxLength(5).HasColumnName("area_code");
            builder.Property(x => x.Type).IsRequired().HasColumnType("varchar(3)").HasConversion(phoneTypeConverter).HasColumnName("type");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");
        }
    }
}
