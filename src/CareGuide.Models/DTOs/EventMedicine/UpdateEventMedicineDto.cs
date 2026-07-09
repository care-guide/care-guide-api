namespace CareGuide.Models.DTOs.EventMedicine
{
    public record UpdateEventMedicineDto(
        string? Dosage,
        string? Laboratory
    );
}
