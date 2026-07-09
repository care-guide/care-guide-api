namespace CareGuide.Models.DTOs.FileAsset
{
    public record FileAssetDto(
        Guid Id,
        string Name,
        long Size,
        string Hash,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
