namespace CareGuide.Models.DTOs.EventSurgery
{
    public record EventSurgeryDto(
        Guid Id,
        Guid EventId,
        string? Type
    );
}
