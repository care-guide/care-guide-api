using CareGuide.Models.Entities.Shared;

namespace CareGuide.Models.Entities
{
    public class FileAsset : Entity
    {
        public required string Name { get; set; }
        public required long Size { get; set; }
        public required string Hash { get; set; }
    }
}
