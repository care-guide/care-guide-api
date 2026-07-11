using CareGuide.Models.Enums;
using System.Text.Json.Serialization;

namespace CareGuide.Models.DTOs.EventExam
{
    public record UpdateEventExamDto(
        [property: JsonConverter(typeof(JsonStringEnumConverter))] EventExamType Type
    );
}
