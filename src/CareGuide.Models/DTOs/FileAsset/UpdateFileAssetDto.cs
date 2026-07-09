namespace CareGuide.Models.DTOs.FileAsset
{
    public record UpdateFileAssetDto(
        Guid Id,
        string Name,
        long Size,
        string Hash
    );
}
