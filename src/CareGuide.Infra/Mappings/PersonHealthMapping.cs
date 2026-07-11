using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareGuide.Infra.Mappings
{
    public class PersonHealthMapping : IEntityTypeConfiguration<PersonHealth>
    {
        public void Configure(EntityTypeBuilder<PersonHealth> builder)
        {
            var bloodTypeConverter = new ValueConverter<BloodType, string>(x => ConvertBloodTypeToString(x), x => ConvertStringToBloodType(x));

            builder.ToTable("person_healths", x =>
            {
                x.HasCheckConstraint("CK_PersonHealth_BloodType", "blood_type IN ('A+','A-','B+','B-','AB+','AB-','O+','O-')");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.PersonId).IsRequired().HasColumnName("person_id");
            builder.Property(x => x.BloodType).IsRequired().HasMaxLength(3).HasColumnName("blood_type").HasConversion(bloodTypeConverter);
            builder.Property(x => x.Height).IsRequired().HasColumnName("height").HasColumnType("decimal(5,2)");
            builder.Property(x => x.Weight).IsRequired().HasColumnName("weight").HasColumnType("decimal(6,2)");
            builder.Property(x => x.Description).HasColumnType("text").HasColumnName("description");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasOne(x => x.Person).WithOne(x => x.PersonHealth).HasForeignKey<PersonHealth>(x => x.PersonId).OnDelete(DeleteBehavior.Cascade);
        }

        private static string ConvertBloodTypeToString(BloodType bloodType)
        {
            return bloodType switch
            {
                BloodType.A_Positive => "A+",
                BloodType.A_Negative => "A-",
                BloodType.B_Positive => "B+",
                BloodType.B_Negative => "B-",
                BloodType.AB_Positive => "AB+",
                BloodType.AB_Negative => "AB-",
                BloodType.O_Positive => "O+",
                BloodType.O_Negative => "O-",
                _ => throw new ArgumentOutOfRangeException(nameof(bloodType), bloodType, null),
            };
        }

        private static BloodType ConvertStringToBloodType(string bloodTypeStr)
        {
            return bloodTypeStr switch
            {
                "A+" => BloodType.A_Positive,
                "A-" => BloodType.A_Negative,
                "B+" => BloodType.B_Positive,
                "B-" => BloodType.B_Negative,
                "AB+" => BloodType.AB_Positive,
                "AB-" => BloodType.AB_Negative,
                "O+" => BloodType.O_Positive,
                "O-" => BloodType.O_Negative,
                _ => throw new ArgumentOutOfRangeException(nameof(bloodTypeStr), bloodTypeStr, null),
            };
        }
    }
}
