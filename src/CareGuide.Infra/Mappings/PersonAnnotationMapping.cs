using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuide.Infra.Mappings
{
    public class PersonAnnotationMapping : IEntityTypeConfiguration<PersonAnnotation>
    {
        public void Configure(EntityTypeBuilder<PersonAnnotation> builder)
        {
            builder.ToTable("person_annotations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.PersonId).IsRequired().HasColumnName("person_id");
            builder.Property(x => x.Details).IsRequired().HasColumnType("text").HasColumnName("details");
            builder.Property(x => x.FileUrl).HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("file_url");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasOne(x => x.Person).WithMany(x => x.PersonAnnotations).HasForeignKey(x => x.PersonId).HasConstraintName("fk_person_annotation_person").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
