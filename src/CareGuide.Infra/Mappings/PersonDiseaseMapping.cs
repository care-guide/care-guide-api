using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareGuide.Infra.Mappings
{
    public class PersonDiseaseMapping : IEntityTypeConfiguration<PersonDisease>
    {
        public void Configure(EntityTypeBuilder<PersonDisease> builder)
        {
            var bloodTypeConverter = new ValueConverter<DiseaseType, string>(x => ConvertDiseaseTypeToString(x), x => ConvertStringToDiseaseType(x));

            builder.ToTable("person_diseases", x =>
            {
                x.HasCheckConstraint("CK_PersonDisease_DiseaseType", "disease_type IN ('ONC','INF','CHR','GEN','AUT','PSY','TRA','INL','RES','CAR','OTH')");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired().HasColumnName("id");
            builder.Property(x => x.PersonId).IsRequired().HasColumnName("person_id");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("name");
            builder.Property(x => x.DiagnosisDate).HasColumnName("diagnosis_date");
            builder.Property(x => x.DiseaseType).IsRequired().HasMaxLength(3).HasColumnName("disease_type").HasConversion(bloodTypeConverter);
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasOne(x => x.Person).WithMany(x => x.PersonDiseases).HasForeignKey(x => x.PersonId).HasConstraintName("fk_person_disease_person").OnDelete(DeleteBehavior.Cascade);
        }

        private static string ConvertDiseaseTypeToString(DiseaseType diseaseType)
        {
            return diseaseType switch
            {
                DiseaseType.Oncological => "ONC",
                DiseaseType.Infectious => "INF",
                DiseaseType.Chronic => "CHR",
                DiseaseType.Genetic => "GEN",
                DiseaseType.Autoimmune => "AUT",
                DiseaseType.Psychiatric => "PSY",
                DiseaseType.Traumatic => "TRA",
                DiseaseType.Inflammatory => "INL",
                DiseaseType.Respiratory => "RES",
                DiseaseType.Cardiovascular => "CAR",
                DiseaseType.Other => "OTH",
                _ => throw new ArgumentOutOfRangeException(nameof(diseaseType), diseaseType, null),
            };
        }

        private static DiseaseType ConvertStringToDiseaseType(string diseaseTypeStr)
        {
            return diseaseTypeStr switch
            {
                "ONC" => DiseaseType.Oncological,
                "INF" => DiseaseType.Infectious,
                "CHR" => DiseaseType.Chronic,
                "GEN" => DiseaseType.Genetic,
                "AUT" => DiseaseType.Autoimmune,
                "PSY" => DiseaseType.Psychiatric,
                "TRA" => DiseaseType.Traumatic,
                "INL" => DiseaseType.Inflammatory,
                "RES" => DiseaseType.Respiratory,
                "CAR" => DiseaseType.Cardiovascular,
                "OTH" => DiseaseType.Other,
                _ => throw new ArgumentOutOfRangeException(nameof(diseaseTypeStr), diseaseTypeStr, null),
            };
        }
    }
}
