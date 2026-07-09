using CareGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuide.Infra.Mappings
{
    public class EventAddressMapping : IEntityTypeConfiguration<EventAddress>
    {
        public void Configure(EntityTypeBuilder<EventAddress> builder)
        {
            builder.ToTable("event_addresses");

            builder.HasKey(x => new { x.EventId, x.AddressId });

            builder.Property(x => x.EventId).IsRequired().HasColumnName("event_id");
            builder.Property(x => x.AddressId).IsRequired().HasColumnName("address_id");

            // Event deletion cascades to its address attachments (they're meaningless without the event).
            builder.HasOne(x => x.Event).WithMany(x => x.EventAddresses).HasForeignKey(x => x.EventId).HasConstraintName("fk_event_address_event").OnDelete(DeleteBehavior.Cascade);
            // Addresses are a shared catalog (can be reused across events/doctors) — restrict deletion while still attached instead of cascading.
            builder.HasOne(x => x.Address).WithMany(x => x.EventAddresses).HasForeignKey(x => x.AddressId).HasConstraintName("fk_event_address_address").OnDelete(DeleteBehavior.Restrict);
        }
    }
}
