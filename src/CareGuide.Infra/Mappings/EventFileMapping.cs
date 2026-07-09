using CareGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuide.Infra.Mappings
{
    public class EventFileMapping : IEntityTypeConfiguration<EventFile>
    {
        public void Configure(EntityTypeBuilder<EventFile> builder)
        {
            builder.ToTable("event_files");

            builder.HasKey(x => new { x.EventId, x.FileId });

            builder.Property(x => x.EventId).IsRequired().HasColumnName("event_id");
            builder.Property(x => x.FileId).IsRequired().HasColumnName("file_id");

            // Event deletion cascades to its file attachments (they're meaningless without the event).
            builder.HasOne(x => x.Event).WithMany(x => x.EventFiles).HasForeignKey(x => x.EventId).HasConstraintName("fk_event_file_event").OnDelete(DeleteBehavior.Cascade);
            // File deletion is restricted while still attached to an event, forcing an explicit detach first to avoid silently orphaning a referenced attachment.
            builder.HasOne(x => x.File).WithMany(x => x.EventFiles).HasForeignKey(x => x.FileId).HasConstraintName("fk_event_file_file").OnDelete(DeleteBehavior.Restrict);
        }
    }
}
