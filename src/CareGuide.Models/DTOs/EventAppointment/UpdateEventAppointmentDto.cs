namespace CareGuide.Models.DTOs.EventAppointment
{
    public record UpdateEventAppointmentDto(
        string? Doctor,
        string? Type
    );
}
