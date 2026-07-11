using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareGuide.Infra.Mappings
{
    public class EventMapping : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            var eventTypeConverter = new ValueConverter<EventType, string>(x => x.ToString(), x => (EventType)Enum.Parse(typeof(EventType), x));
            var recurrenceTypeConverter = new ValueConverter<RecurrenceType?, string?>(x => x == null ? null : x.ToString(), x => x == null ? null : (RecurrenceType)Enum.Parse(typeof(RecurrenceType), x));
            var recurrenceFrequencyConverter = new ValueConverter<RecurrenceFrequency?, string?>(x => x == null ? null : x.ToString(), x => x == null ? null : (RecurrenceFrequency)Enum.Parse(typeof(RecurrenceFrequency), x));

            builder.ToTable("events");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.PersonId).IsRequired().HasColumnName("person_id");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("name");
            builder.Property(x => x.StartDate).IsRequired().HasColumnName("start_date");
            builder.Property(x => x.EndDate).HasColumnName("end_date");
            builder.Property(x => x.Type).IsRequired().HasColumnType("varchar(20)").HasConversion(eventTypeConverter).HasColumnName("type");
            builder.Property(x => x.IsAllDay).IsRequired().HasDefaultValue(false).HasColumnName("is_all_day");
            builder.Property(x => x.RecurrenceType).HasColumnType("varchar(20)").HasConversion(recurrenceTypeConverter).HasColumnName("recurrence_type");
            builder.Property(x => x.RecurrenceFrequency).HasColumnType("varchar(20)").HasConversion(recurrenceFrequencyConverter).HasColumnName("recurrence_frequency");
            builder.Property(x => x.Details).HasColumnType("text").HasMaxLength(DatabaseConstants.MaxLengthLargeText).HasColumnName("details");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasIndex(x => x.PersonId);
            builder.HasIndex(x => x.StartDate);

            builder.HasOne(x => x.Person).WithMany(x => x.Events).HasForeignKey(x => x.PersonId).HasConstraintName("fk_event_person").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
