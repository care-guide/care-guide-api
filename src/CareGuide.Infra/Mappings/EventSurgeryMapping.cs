using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareGuide.Infra.Mappings
{
    public class EventSurgeryMapping : IEntityTypeConfiguration<EventSurgery>
    {
        public void Configure(EntityTypeBuilder<EventSurgery> builder)
        {
            var typeConverter = new ValueConverter<EventSurgeryType, string>(x => ConvertTypeToString(x), x => ConvertStringToType(x));

            builder.ToTable("event_surgeries", x =>
            {
                x.HasCheckConstraint("CK_EventSurgery_Type", "type IN ('CUR','PAL','DIA','REP','REC','OTH')");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.EventId).IsRequired().HasColumnName("event_id");
            builder.Property(x => x.Type).IsRequired().HasMaxLength(3).HasColumnName("type").HasConversion(typeConverter);
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasIndex(x => x.EventId).IsUnique();

            builder.HasOne(x => x.Event).WithOne().HasForeignKey<EventSurgery>(x => x.EventId).HasConstraintName("fk_event_surgery_event").OnDelete(DeleteBehavior.Cascade);
        }

        private static string ConvertTypeToString(EventSurgeryType type)
        {
            return type switch
            {
                EventSurgeryType.Curative => "CUR",
                EventSurgeryType.Palliative => "PAL",
                EventSurgeryType.Diagnostic => "DIA",
                EventSurgeryType.Reparative => "REP",
                EventSurgeryType.Reconstructive => "REC",
                EventSurgeryType.Other => "OTH",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }

        private static EventSurgeryType ConvertStringToType(string typeStr)
        {
            return typeStr switch
            {
                "CUR" => EventSurgeryType.Curative,
                "PAL" => EventSurgeryType.Palliative,
                "DIA" => EventSurgeryType.Diagnostic,
                "REP" => EventSurgeryType.Reparative,
                "REC" => EventSurgeryType.Reconstructive,
                "OTH" => EventSurgeryType.Other,
                _ => throw new ArgumentOutOfRangeException(nameof(typeStr), typeStr, null),
            };
        }
    }
}
