using AutoMapper;
using CareGuide.Models.DTOs.EventFile;
using CareGuide.Models.Entities;
using CareGuide.Models.Mappers.EventFile;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class EventFileProfileMapperTests
{
    private readonly IMapper _mapper;

    public EventFileProfileMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<EventFileProfileMapper>());
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "EventFile→EventFileDto: maps all fields from nested File")]
    public void Map_EventFileToDto_MapsAllFields()
    {
        var eventId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var entity = new EventFile
        {
            EventId = eventId,
            FileId = fileId,
            File = new FileAsset
            {
                Id = fileId,
                Name = "exam-result.pdf",
                Size = 2048,
                Hash = "abc123",
                ContentType = "application/pdf",
            }
        };

        var dto = _mapper.Map<EventFileDto>(entity);

        dto.EventId.Should().Be(eventId);
        dto.FileId.Should().Be(fileId);
        dto.Name.Should().Be("exam-result.pdf");
        dto.Size.Should().Be(2048);
        dto.Hash.Should().Be("abc123");
        dto.ContentType.Should().Be("application/pdf");
    }

    [Fact(DisplayName = "EventFile→EventFileDto: null File navigation throws InvalidOperationException")]
    public void Map_EventFileToDto_NullFile_ThrowsInvalidOperationException()
    {
        var entity = new EventFile
        {
            EventId = Guid.NewGuid(),
            FileId = Guid.NewGuid(),
            File = null!
        };

        var act = () => _mapper.Map<EventFileDto>(entity);

        act.Should().Throw<AutoMapperMappingException>()
            .WithInnerException<InvalidOperationException>()
            .WithMessage("File not found.");
    }
}
