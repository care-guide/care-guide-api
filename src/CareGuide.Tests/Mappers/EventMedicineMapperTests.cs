using AutoMapper;
using CareGuide.Models.DTOs.EventMedicine;
using CareGuide.Models.Entities;
using CareGuide.Models.Mappers.EventMedicine;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class EventMedicineMapperTests
{
    private readonly IMapper _mapper;

    public EventMedicineMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EventMedicineProfileMapper>();
        });
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "EventMedicine→EventMedicineDto: all fields map correctly")]
    public void Map_EventMedicineToDto_MapsAllFields()
    {
        var eventId = Guid.NewGuid();
        var entity = new EventMedicine
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Dosage = "500mg",
            Laboratory = "Acme",
        };

        var dto = _mapper.Map<EventMedicineDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.EventId.Should().Be(eventId);
        dto.Dosage.Should().Be("500mg");
        dto.Laboratory.Should().Be("Acme");
    }

    [Fact(DisplayName = "EventMedicine→EventMedicineDto: null fields map to null")]
    public void Map_EventMedicineToDto_NullFields_MapToNull()
    {
        var entity = new EventMedicine { EventId = Guid.NewGuid(), Dosage = null, Laboratory = null };

        var dto = _mapper.Map<EventMedicineDto>(entity);

        dto.Dosage.Should().BeNull();
        dto.Laboratory.Should().BeNull();
    }

    [Fact(DisplayName = "CreateEventMedicineDto→EventMedicine: all fields map correctly")]
    public void Map_CreateEventMedicineDtoToEntity_MapsFields()
    {
        var dto = new CreateEventMedicineDto("500mg", "Acme");

        var entity = _mapper.Map<EventMedicine>(dto);

        entity.Dosage.Should().Be("500mg");
        entity.Laboratory.Should().Be("Acme");
    }

    [Fact(DisplayName = "UpdateEventMedicineDto→EventMedicine: all fields map correctly")]
    public void Map_UpdateEventMedicineDtoToEntity_MapsFields()
    {
        var dto = new UpdateEventMedicineDto("New Dosage", "New Lab");

        var entity = _mapper.Map<EventMedicine>(dto);

        entity.Dosage.Should().Be("New Dosage");
        entity.Laboratory.Should().Be("New Lab");
    }
}
