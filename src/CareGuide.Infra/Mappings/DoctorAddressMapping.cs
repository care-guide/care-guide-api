using CareGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuide.Infra.Mappings
{
    public class DoctorAddressMapping : IEntityTypeConfiguration<DoctorAddress>
    {
        public void Configure(EntityTypeBuilder<DoctorAddress> builder)
        {
            builder.ToTable("doctor_addresses");

            builder.HasKey(x => new { x.DoctorId, x.AddressId });

            builder.Property(x => x.DoctorId).IsRequired().HasColumnName("doctor_id");
            builder.Property(x => x.AddressId).IsRequired().HasColumnName("address_id");

            builder.HasOne(x => x.Doctor).WithMany(x => x.DoctorAddresses).HasForeignKey(x => x.DoctorId).HasConstraintName("fk_doctor_address_doctor").OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Address).WithMany(x => x.DoctorAddresses).HasForeignKey(x => x.AddressId).HasConstraintName("fk_doctor_address_address").OnDelete(DeleteBehavior.Restrict);
        }
    }
}
