using CareGuide.Models.Entities.Shared;
using CareGuide.Models.Enums;
using System.Diagnostics.CodeAnalysis;

namespace CareGuide.Models.Entities
{
    public class FileAsset : Entity, IPersonOwnedEntity
    {
        [SetsRequiredMembers]
        public FileAsset()
        {
            Name = string.Empty;
            Hash = string.Empty;
            StorageKey = string.Empty;
            ContentType = string.Empty;
        }

        public required Guid? PersonId { get; set; }
        public required string Name { get; set; }
        public required long Size { get; set; }
        public required string Hash { get; set; }
        public required string StorageKey { get; set; }
        public required string ContentType { get; set; }
        public StorageProvider StorageProvider { get; set; }
        public FileUploadStatus UploadStatus { get; set; }

        public Person Person { get; set; } = null!;
        public ICollection<EventFile> EventFiles { get; set; } = new List<EventFile>();
    }
}
