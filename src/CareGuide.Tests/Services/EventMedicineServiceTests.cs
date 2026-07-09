using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventMedicine;
using CareGuide.Models.Entities;

namespace CareGuide.Tests.Services;

public class EventMedicineServiceTests
{
    private readonly IEventMedicineRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly EventMedicineService _sut;

    public EventMedicineServiceTests()
    {
        _repository = Substitute.For<IEventMedicineRepository>();
        _eventRepository = Substitute.For<IEventRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new EventMedicineService(_repository, _eventRepository, _mapper);
    }

    [Fact(DisplayName = "GetByEventIdAsync: not found throws KeyNotFoundException")]
    public async Task GetByEventIdAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventMedicine?)null);

        var act = async () => await _sut.GetByEventIdAsync(eventId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "GetByEventIdAsync: found returns mapped DTO")]
    public async Task GetByEventIdAsync_Found_ReturnsMappedDto()
    {
        var eventId = Guid.NewGuid();
        var medicine = new EventMedicine { EventId = eventId, Dosage = "500mg", Laboratory = "Acme" };
        var expected = new EventMedicineDto(medicine.Id, eventId, "500mg", "Acme");

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(medicine);
        _mapper.Map<EventMedicineDto>(medicine).Returns(expected);

        var result = await _sut.GetByEventIdAsync(eventId, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CreateAsync: null dto throws ArgumentNullException")]
    public async Task CreateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.CreateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync: event not owned by person throws UnauthorizedAccessException")]
    public async Task CreateAsync_EventNotOwned_ThrowsUnauthorizedAccessException()
    {
        var eventId = Guid.NewGuid();
        var dto = new CreateEventMedicineDto("500mg", "Acme");

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns((Event?)null);

        var act = async () => await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "CreateAsync: medicine record already exists throws InvalidOperationException")]
    public async Task CreateAsync_AlreadyExists_ThrowsInvalidOperationException()
    {
        var eventId = Guid.NewGuid();
        var dto = new CreateEventMedicineDto("500mg", "Acme");

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Medicine });
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(new EventMedicine { EventId = eventId });

        var act = async () => await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "CreateAsync: valid dto sets EventId and persists entity")]
    public async Task CreateAsync_ValidDto_SetsEventIdAndPersists()
    {
        var eventId = Guid.NewGuid();
        var dto = new CreateEventMedicineDto("500mg", "Acme");
        var entity = new EventMedicine { EventId = Guid.Empty, Dosage = "500mg", Laboratory = "Acme" };
        var expected = new EventMedicineDto(entity.Id, eventId, "500mg", "Acme");

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Medicine });
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventMedicine?)null);
        _mapper.Map<EventMedicine>(dto).Returns(entity);
        _repository.AddAsync(Arg.Any<EventMedicine>(), Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<EventMedicineDto>(entity).Returns(expected);

        await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        entity.EventId.Should().Be(eventId);
    }

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: not found throws KeyNotFoundException")]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        var dto = new UpdateEventMedicineDto("500mg", "Acme");

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventMedicine?)null);

        var act = async () => await _sut.UpdateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var eventId = Guid.NewGuid();
        var existing = new EventMedicine { EventId = eventId, Dosage = "Old", Laboratory = "Old Lab" };
        var dto = new UpdateEventMedicineDto("New Dosage", "New Lab");
        var expected = new EventMedicineDto(existing.Id, eventId, "New Dosage", "New Lab");

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<EventMedicineDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(eventId, dto, CancellationToken.None);

        existing.Dosage.Should().Be("New Dosage");
        existing.Laboratory.Should().Be("New Lab");
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "DeleteAsync: not found throws KeyNotFoundException")]
    public async Task DeleteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventMedicine?)null);

        var act = async () => await _sut.DeleteAsync(eventId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "DeleteAsync: found calls repository DeleteAsync with medicine id")]
    public async Task DeleteAsync_Found_CallsDeleteAsync()
    {
        var eventId = Guid.NewGuid();
        var medicine = new EventMedicine { EventId = eventId };

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(medicine);

        await _sut.DeleteAsync(eventId, CancellationToken.None);

        await _repository.Received(1).DeleteAsync(medicine.Id, Arg.Any<CancellationToken>());
    }
}
