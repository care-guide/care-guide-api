using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.FileAsset
{
    // StorageKey (internal R2 object key) is intentionally NOT exposed here — see docs/SECURITY.md §7
    // ("object keys ... must not appear in application logs or error bodies"). It remains available
    // internally on the FileAsset entity for the service/repository layer.
    public record FileAssetDto(
        Guid Id,
        string Name,
        long Size,
        string Hash,
        string ContentType,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] StorageProvider StorageProvider,
        [property: JsonConverter(typeof(JsonStringEnumConverter))] FileUploadStatus UploadStatus,
        DateTime CreatedAt,
        DateTime UpdatedAt
    );
}
