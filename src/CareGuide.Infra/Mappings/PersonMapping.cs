using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareGuide.Infra.Mappings
{
    public class PersonMapping : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            var genderConverter = new ValueConverter<Gender, string>(x => x.ToString(), x => (Gender)Enum.Parse(typeof(Gender), x));
            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(x => x.ToDateTime(TimeOnly.MinValue), x => DateOnly.FromDateTime(x));

            builder.ToTable("persons", x =>
            {
                x.HasCheckConstraint("CK_Person_Gender", "gender IN ('M', 'F', 'O')");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("name");
            builder.Property(x => x.Gender).IsRequired().HasColumnType("char(1)").HasConversion(genderConverter).HasColumnName("gender");
            builder.Property(x => x.Birthday).IsRequired().HasColumnType("date").HasConversion(dateOnlyConverter).HasColumnName("birthday");
            builder.Property(x => x.Picture).HasColumnName("picture");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");
        }
    }
}
