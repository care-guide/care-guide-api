using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CareGuide.Infra.Mappings
{
    public class EventAppointmentMapping : IEntityTypeConfiguration<EventAppointment>
    {
        public void Configure(EntityTypeBuilder<EventAppointment> builder)
        {
            var typeConverter = new ValueConverter<EventAppointmentType, string>(x => ConvertTypeToString(x), x => ConvertStringToType(x));

            builder.ToTable("event_appointments", x =>
            {
                x.HasCheckConstraint("CK_EventAppointment_Type", "type IN ('ROU','EME','FOL','SPE','PRE','SCR','OTH')");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.EventId).IsRequired().HasColumnName("event_id");
            builder.Property(x => x.Doctor).HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("doctor");
            builder.Property(x => x.DoctorId).HasColumnName("doctor_id");
            builder.Property(x => x.Type).IsRequired().HasMaxLength(3).HasColumnName("type").HasConversion(typeConverter);
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasIndex(x => x.EventId).IsUnique();
            builder.HasIndex(x => x.DoctorId);

            builder.HasOne(x => x.Event).WithOne().HasForeignKey<EventAppointment>(x => x.EventId).HasConstraintName("fk_event_appointment_event").OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.DoctorRef).WithMany().HasForeignKey(x => x.DoctorId).HasConstraintName("fk_event_appointment_doctor").OnDelete(DeleteBehavior.SetNull);
        }

        private static string ConvertTypeToString(EventAppointmentType type)
        {
            return type switch
            {
                EventAppointmentType.Routine => "ROU",
                EventAppointmentType.Emergency => "EME",
                EventAppointmentType.FollowUp => "FOL",
                EventAppointmentType.Specialized => "SPE",
                EventAppointmentType.Preventive => "PRE",
                EventAppointmentType.Screening => "SCR",
                EventAppointmentType.Other => "OTH",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }

        private static EventAppointmentType ConvertStringToType(string typeStr)
        {
            return typeStr switch
            {
                "ROU" => EventAppointmentType.Routine,
                "EME" => EventAppointmentType.Emergency,
                "FOL" => EventAppointmentType.FollowUp,
                "SPE" => EventAppointmentType.Specialized,
                "PRE" => EventAppointmentType.Preventive,
                "SCR" => EventAppointmentType.Screening,
                "OTH" => EventAppointmentType.Other,
                _ => throw new ArgumentOutOfRangeException(nameof(typeStr), typeStr, null),
            };
        }
    }
}
