namespace CareGuide.Models.DTOs.EventMedicine
{
    public record EventMedicineDto(
        Guid Id,
        Guid EventId,
        string? Dosage,
        string? Laboratory
    );
}
