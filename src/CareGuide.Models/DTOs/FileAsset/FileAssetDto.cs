using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.FileAsset
{
    public record FileAssetDto(
        Guid Id,
        string Name,
        long Size,
        string Hash,
        string StorageKey,
        string ContentType,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] StorageProvider StorageProvider,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] FileUploadStatus UploadStatus,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
