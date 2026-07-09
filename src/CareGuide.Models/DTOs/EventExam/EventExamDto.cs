namespace CareGuide.Models.DTOs.EventExam
{
    public record EventExamDto(
        Guid Id,
        Guid EventId,
        string? Type
    );
}
