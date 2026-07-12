using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventAddress;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class EventAddressServiceTests
{
    private readonly IEventAddressRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;
    private readonly EventAddressService _sut;

    public EventAddressServiceTests()
    {
        _repository = Substitute.For<IEventAddressRepository>();
        _eventRepository = Substitute.For<IEventRepository>();
        _addressRepository = Substitute.For<IAddressRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new EventAddressService(_repository, _eventRepository, _addressRepository, _mapper);
    }

    private static Address CreateAddress(Guid id) => new()
    {
        Id = id,
        PostalCode = "12345-000",
        Type = AddressType.Clinic,
        City = "Sao Paulo",
        Country = "Brazil",
        State = "SP",
    };

    // ── GetAllByEventIdAsync ──────────────────────────────────────────────────

    [Fact(DisplayName = "GetAllByEventIdAsync: returns paged envelope with real total")]
    public async Task GetAllByEventIdAsync_ReturnsMappedList()
    {
        var eventId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var eventAddresses = new List<EventAddress> { new() { EventId = eventId, AddressId = addressId, Address = CreateAddress(addressId) } };
        var dtos = new List<EventAddressDto> { new(eventId, addressId, "12345-000", null, AddressType.Clinic, "Sao Paulo", null, "Brazil", null, null, "SP") };

        _repository.GetAllByEventAsync(1, 10, eventId, Arg.Any<CancellationToken>()).Returns(eventAddresses);
        _repository.CountByEventAsync(eventId, Arg.Any<CancellationToken>()).Returns(1);
        _mapper.Map<List<EventAddressDto>>(eventAddresses).Returns(dtos);

        var result = await _sut.GetAllByEventIdAsync(1, 10, eventId, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Total.Should().Be(1);
    }

    [Fact(DisplayName = "GetAllByEventIdAsync: empty result returns empty collection")]
    public async Task GetAllByEventIdAsync_Empty_ReturnsEmptyCollection()
    {
        var eventId = Guid.NewGuid();
        _repository.GetAllByEventAsync(1, 10, eventId, Arg.Any<CancellationToken>()).Returns(new List<EventAddress>());
        _repository.CountByEventAsync(eventId, Arg.Any<CancellationToken>()).Returns(0);
        _mapper.Map<List<EventAddressDto>>(Arg.Any<List<EventAddress>>()).Returns(new List<EventAddressDto>());

        var result = await _sut.GetAllByEventIdAsync(1, 10, eventId, CancellationToken.None);

        result.Items.Should().BeEmpty();
        result.Total.Should().Be(0);
    }

    // ── AttachAsync ───────────────────────────────────────────────────────────

    [Fact(DisplayName = "AttachAsync: event not owned by person throws UnauthorizedAccessException")]
    public async Task AttachAsync_EventNotOwned_ThrowsUnauthorizedAccessException()
    {
        var eventId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns((Event?)null);

        var act = async () => await _sut.AttachAsync(eventId, addressId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "AttachAsync: address not found throws KeyNotFoundException")]
    public async Task AttachAsync_AddressNotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Appointment });
        _addressRepository.GetAsync(addressId, Arg.Any<CancellationToken>()).Returns((Address?)null);

        var act = async () => await _sut.AttachAsync(eventId, addressId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{addressId}*");
    }

    [Fact(DisplayName = "AttachAsync: already attached throws InvalidOperationException")]
    public async Task AttachAsync_AlreadyAttached_ThrowsInvalidOperationException()
    {
        var eventId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Appointment });
        _addressRepository.GetAsync(addressId, Arg.Any<CancellationToken>()).Returns(CreateAddress(addressId));
        _repository.GetByEventAndAddressAsync(eventId, addressId, Arg.Any<CancellationToken>()).Returns(new EventAddress { EventId = eventId, AddressId = addressId });

        var act = async () => await _sut.AttachAsync(eventId, addressId, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*{addressId}*{eventId}*");
    }

    [Fact(DisplayName = "AttachAsync: valid attach persists entity and returns mapped DTO")]
    public async Task AttachAsync_Valid_PersistsAndReturnsDto()
    {
        var eventId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var created = new EventAddress { EventId = eventId, AddressId = addressId, Address = CreateAddress(addressId) };
        var expected = new EventAddressDto(eventId, addressId, "12345-000", null, AddressType.Clinic, "Sao Paulo", null, "Brazil", null, null, "SP");

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Appointment });
        _addressRepository.GetAsync(addressId, Arg.Any<CancellationToken>()).Returns(CreateAddress(addressId));
        _repository.GetByEventAndAddressAsync(eventId, addressId, Arg.Any<CancellationToken>()).Returns((EventAddress?)null, created);
        _mapper.Map<EventAddressDto>(created).Returns(expected);

        var result = await _sut.AttachAsync(eventId, addressId, CancellationToken.None);

        await _repository.Received(1).AddAsync(Arg.Is<EventAddress>(ea => ea.EventId == eventId && ea.AddressId == addressId), Arg.Any<CancellationToken>());
        result.Should().Be(expected);
    }

    // ── DetachAsync ───────────────────────────────────────────────────────────

    [Fact(DisplayName = "DetachAsync: not found throws KeyNotFoundException")]
    public async Task DetachAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        _repository.GetByEventAndAddressAsync(eventId, addressId, Arg.Any<CancellationToken>()).Returns((EventAddress?)null);

        var act = async () => await _sut.DetachAsync(eventId, addressId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "DetachAsync: found calls repository DeleteAsync")]
    public async Task DetachAsync_Found_CallsDeleteAsync()
    {
        var eventId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var eventAddress = new EventAddress { EventId = eventId, AddressId = addressId };

        _repository.GetByEventAndAddressAsync(eventId, addressId, Arg.Any<CancellationToken>()).Returns(eventAddress);

        await _sut.DetachAsync(eventId, addressId, CancellationToken.None);

        await _repository.Received(1).DeleteAsync(eventAddress, Arg.Any<CancellationToken>());
    }
}
