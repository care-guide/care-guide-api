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
            builder.Property(x => x.Name).IsRequired().HasMaxLength(DatabaseConstants.MaxLengthStandardText).HasColumnName("name");
            builder.Property(x => x.Size).IsRequired().HasColumnName("size");
            builder.Property(x => x.Hash).IsRequired().HasMaxLength(128).HasColumnName("hash");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true).HasColumnName("is_active");

            builder.HasIndex(x => x.Hash);
        }
    }
}
