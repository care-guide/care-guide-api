namespace CareGuide.Models.DTOs.EventFile
{
    public record EventFileDto(
        Guid EventId,
        Guid FileId,
        string Name,
        long Size,
        string Hash,
        string ContentType
    );
}
