using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareGuide.Infra.Mappings
{
    public class EventExamMapping : IEntityTypeConfiguration<EventExam>
    {
        public void Configure(EntityTypeBuilder<EventExam> builder)
        {
            var typeConverter = new ValueConverter<EventExamType, string>(x => ConvertTypeToString(x), x => ConvertStringToType(x));

            builder.ToTable("event_exams", x =>
            {
                x.HasCheckConstraint("CK_EventExam_Type", "type IN ('BLO','IMG','ELE','IMM','MON','NEW','GEN','OTH')");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.EventId).IsRequired().HasColumnName("event_id");
            builder.Property(x => x.Type).IsRequired().HasMaxLength(3).HasColumnName("type").HasConversion(typeConverter);
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasIndex(x => x.EventId).IsUnique();

            builder.HasOne(x => x.Event).WithOne().HasForeignKey<EventExam>(x => x.EventId).HasConstraintName("fk_event_exam_event").OnDelete(DeleteBehavior.Cascade);
        }

        private static string ConvertTypeToString(EventExamType type)
        {
            return type switch
            {
                EventExamType.Blood => "BLO",
                EventExamType.Imaging => "IMG",
                EventExamType.Electrodiagnosis => "ELE",
                EventExamType.Immunology => "IMM",
                EventExamType.Monitoring => "MON",
                EventExamType.Newborn => "NEW",
                EventExamType.Genetic => "GEN",
                EventExamType.Other => "OTH",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }

        private static EventExamType ConvertStringToType(string typeStr)
        {
            return typeStr switch
            {
                "BLO" => EventExamType.Blood,
                "IMG" => EventExamType.Imaging,
                "ELE" => EventExamType.Electrodiagnosis,
                "IMM" => EventExamType.Immunology,
                "MON" => EventExamType.Monitoring,
                "NEW" => EventExamType.Newborn,
                "GEN" => EventExamType.Genetic,
                "OTH" => EventExamType.Other,
                _ => throw new ArgumentOutOfRangeException(nameof(typeStr), typeStr, null),
            };
        }
    }
}
