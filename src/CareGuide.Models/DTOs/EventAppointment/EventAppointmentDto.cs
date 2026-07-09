namespace CareGuide.Models.DTOs.EventAppointment
{
    public record EventAppointmentDto(
        Guid Id,
        Guid EventId,
        string? Doctor,
        string? Type
    );
}
