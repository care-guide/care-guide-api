using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuide.Infra.Mappings
{
    public class PersonFamilyHistoryMapping : IEntityTypeConfiguration<PersonFamilyHistory>
    {
        public void Configure(EntityTypeBuilder<PersonFamilyHistory> builder)
        {
            builder.ToTable("person_family_histories");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.PersonId).IsRequired().HasColumnName("person_id");
            builder.Property(x => x.Relationship).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("relationship");
            builder.Property(x => x.Diagnosis).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("diagnosis");
            builder.Property(x => x.AgeAtDiagnosis).HasColumnName("age_at_diagnosis");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasOne(x => x.Person).WithMany(x => x.PersonFamilyHistories).HasForeignKey(x => x.PersonId).HasConstraintName("fk_person_family_history_person").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
