namespace CareGuide.Models.DTOs.EventAppointment
{
    public record CreateEventAppointmentDto(
        string? Doctor,
        string? Type
    );
}
