namespace CareGuide.Models.DTOs.EventMedicine
{
    public record CreateEventMedicineDto(
        string? Dosage,
        string? Laboratory
    );
}
