namespace CareGuide.Models.Entities
{
    public class EventFile
    {
        public Guid EventId { get; set; }
        public Guid FileId { get; set; }

        public Event Event { get; set; } = null!;
        public FileAsset File { get; set; } = null!;
    }
}
