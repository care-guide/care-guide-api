using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuide.Infra.Mappings
{
    public class DoctorMapping : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("doctors");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.PersonId).IsRequired().HasColumnName("person_id");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("name");
            builder.Property(x => x.Details).HasColumnType("text").HasMaxLength(DatabaseConstants.MaxLengthLargeText).HasColumnName("details");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasOne(x => x.Person).WithMany(x => x.Doctors).HasForeignKey(x => x.PersonId).HasConstraintName("fk_doctor_person").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
