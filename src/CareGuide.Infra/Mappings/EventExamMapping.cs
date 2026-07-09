using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuide.Infra.Mappings
{
    public class EventExamMapping : IEntityTypeConfiguration<EventExam>
    {
        public void Configure(EntityTypeBuilder<EventExam> builder)
        {
            builder.ToTable("event_exams");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.EventId).IsRequired().HasColumnName("event_id");
            builder.Property(x => x.Type).HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("type");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true).HasColumnName("is_active");

            builder.HasIndex(x => x.EventId).IsUnique();

            builder.HasOne(x => x.Event).WithOne().HasForeignKey<EventExam>(x => x.EventId).HasConstraintName("fk_event_exam_event").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
