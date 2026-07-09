namespace CareGuide.Models.DTOs.FileAsset
{
    public record CreateFileAssetDto(
        string Name,
        long Size,
        string Hash
    );
}
