using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareGuide.Infra.Mappings
{
    public class FileAssetMapping : IEntityTypeConfiguration<FileAsset>
    {
        public void Configure(EntityTypeBuilder<FileAsset> builder)
        {
            builder.ToTable("files");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.PersonId).IsRequired().HasColumnName("person_id");
            builder.Property(x => x.Name).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("name");
            builder.Property(x => x.Size).IsRequired().HasColumnName("size");
            builder.Property(x => x.Hash).IsRequired().HasMaxLength(128).HasColumnName("hash");
            builder.Property(x => x.StorageKey).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("storage_key");
            builder.Property(x => x.ContentType).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("content_type");
            builder.Property(x => x.StorageProvider).IsRequired().HasConversion<string>().HasMaxLength(50).HasColumnName("storage_provider");
            builder.Property(x => x.UploadStatus).IsRequired().HasConversion<string>().HasMaxLength(50).HasColumnName("upload_status");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");

            builder.HasIndex(x => x.Hash);
            builder.HasIndex(x => x.StorageKey).IsUnique();
            builder.HasIndex(x => x.PersonId);

            builder.HasOne(x => x.Person).WithMany().HasForeignKey(x => x.PersonId).HasConstraintName("fk_file_person").OnDelete(DeleteBehavior.Cascade);
        }
    }
}
