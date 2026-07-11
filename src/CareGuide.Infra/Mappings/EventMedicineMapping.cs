using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuide.Infra.Mappings
{
    public class EventMedicineMapping : IEntityTypeConfiguration<EventMedicine>
    {
        public void Configure(EntityTypeBuilder<EventMedicine> builder)
        {
            builder.ToTable("event_medicines");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.EventId).IsRequired().HasColumnName("event_id");
            builder.Property(x => x.Dosage).HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("dosage");
            builder.Property(x => x.Laboratory).HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("laboratory");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasIndex(x => x.EventId).IsUnique();

            builder.HasOne(x => x.Event).WithOne().HasForeignKey<EventMedicine>(x => x.EventId).HasConstraintName("fk_event_medicine_event").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
