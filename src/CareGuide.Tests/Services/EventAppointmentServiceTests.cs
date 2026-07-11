using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventAppointment;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class EventAppointmentServiceTests
{
    private readonly IEventAppointmentRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;
    private readonly EventAppointmentService _sut;

    public EventAppointmentServiceTests()
    {
        _repository = Substitute.For<IEventAppointmentRepository>();
        _eventRepository = Substitute.For<IEventRepository>();
        _doctorRepository = Substitute.For<IDoctorRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new EventAppointmentService(_repository, _eventRepository, _doctorRepository, _mapper);
    }

    [Fact(DisplayName = "GetByEventIdAsync: not found throws KeyNotFoundException")]
    public async Task GetByEventIdAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventAppointment?)null);

        var act = async () => await _sut.GetByEventIdAsync(eventId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "GetByEventIdAsync: found returns mapped DTO")]
    public async Task GetByEventIdAsync_Found_ReturnsMappedDto()
    {
        var eventId = Guid.NewGuid();
        var appointment = new EventAppointment { EventId = eventId, Doctor = "Dr. House", Type = EventAppointmentType.Routine };
        var expected = new EventAppointmentDto(appointment.Id, eventId, "Dr. House", null, null, EventAppointmentType.Routine);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(appointment);
        _mapper.Map<EventAppointmentDto>(appointment).Returns(expected);

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
        var dto = new CreateEventAppointmentDto("Dr. House", null, EventAppointmentType.Routine);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns((Event?)null);

        var act = async () => await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "CreateAsync: appointment already exists throws InvalidOperationException")]
    public async Task CreateAsync_AlreadyExists_ThrowsInvalidOperationException()
    {
        var eventId = Guid.NewGuid();
        var dto = new CreateEventAppointmentDto("Dr. House", null, EventAppointmentType.Routine);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Appointment });
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(new EventAppointment { EventId = eventId, Type = EventAppointmentType.Routine });

        var act = async () => await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "CreateAsync: valid dto sets EventId and persists entity")]
    public async Task CreateAsync_ValidDto_SetsEventIdAndPersists()
    {
        var eventId = Guid.NewGuid();
        var dto = new CreateEventAppointmentDto("Dr. House", null, EventAppointmentType.Routine);
        var entity = new EventAppointment { EventId = Guid.Empty, Doctor = "Dr. House", Type = EventAppointmentType.Routine };
        var created = new EventAppointment { Id = entity.Id, EventId = eventId, Doctor = "Dr. House", Type = EventAppointmentType.Routine };
        var expected = new EventAppointmentDto(entity.Id, eventId, "Dr. House", null, null, EventAppointmentType.Routine);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Appointment });
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventAppointment?)null, created);
        _mapper.Map<EventAppointment>(dto).Returns(entity);
        _repository.AddAsync(Arg.Any<EventAppointment>(), Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<EventAppointmentDto>(created).Returns(expected);

        await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        entity.EventId.Should().Be(eventId);
    }

    [Fact(DisplayName = "CreateAsync: DoctorId not owned by person throws UnauthorizedAccessException")]
    public async Task CreateAsync_DoctorIdNotOwned_ThrowsUnauthorizedAccessException()
    {
        var eventId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var dto = new CreateEventAppointmentDto(null, doctorId, EventAppointmentType.Routine);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Appointment });
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventAppointment?)null);
        _doctorRepository.GetAsync(doctorId, Arg.Any<CancellationToken>()).Returns((Doctor?)null);

        var act = async () => await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        await _repository.DidNotReceive().AddAsync(Arg.Any<EventAppointment>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "CreateAsync: DoctorId owned by person persists entity")]
    public async Task CreateAsync_DoctorIdOwned_PersistsEntity()
    {
        var eventId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var dto = new CreateEventAppointmentDto(null, doctorId, EventAppointmentType.Routine);
        var entity = new EventAppointment { EventId = Guid.Empty, DoctorId = doctorId, Type = EventAppointmentType.Routine };
        var created = new EventAppointment { Id = entity.Id, EventId = eventId, DoctorId = doctorId, Type = EventAppointmentType.Routine };
        var expected = new EventAppointmentDto(entity.Id, eventId, null, doctorId, "Dr. House", EventAppointmentType.Routine);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Appointment });
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventAppointment?)null, created);
        _doctorRepository.GetAsync(doctorId, Arg.Any<CancellationToken>()).Returns(new Doctor { Id = doctorId, PersonId = Guid.NewGuid(), Name = "Dr. House" });
        _mapper.Map<EventAppointment>(dto).Returns(entity);
        _repository.AddAsync(Arg.Any<EventAppointment>(), Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<EventAppointmentDto>(created).Returns(expected);

        var result = await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(Arg.Any<EventAppointment>(), Arg.Any<CancellationToken>());
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
        var dto = new UpdateEventAppointmentDto("Dr. House", null, EventAppointmentType.Routine);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventAppointment?)null);

        var act = async () => await _sut.UpdateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var eventId = Guid.NewGuid();
        var existing = new EventAppointment { EventId = eventId, Doctor = "Old Doctor", Type = EventAppointmentType.Routine };
        var dto = new UpdateEventAppointmentDto("New Doctor", null, EventAppointmentType.Emergency);
        var updated = new EventAppointment { Id = existing.Id, EventId = eventId, Doctor = "New Doctor", Type = EventAppointmentType.Emergency };
        var expected = new EventAppointmentDto(existing.Id, eventId, "New Doctor", null, null, EventAppointmentType.Emergency);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(existing, updated);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<EventAppointmentDto>(updated).Returns(expected);

        var result = await _sut.UpdateAsync(eventId, dto, CancellationToken.None);

        existing.Doctor.Should().Be("New Doctor");
        existing.Type.Should().Be(EventAppointmentType.Emergency);
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "UpdateAsync: DoctorId not owned by person throws UnauthorizedAccessException")]
    public async Task UpdateAsync_DoctorIdNotOwned_ThrowsUnauthorizedAccessException()
    {
        var eventId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var existing = new EventAppointment { EventId = eventId, Doctor = "Old Doctor", Type = EventAppointmentType.Routine };
        var dto = new UpdateEventAppointmentDto("Old Doctor", doctorId, EventAppointmentType.Routine);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(existing);
        _doctorRepository.GetAsync(doctorId, Arg.Any<CancellationToken>()).Returns((Doctor?)null);

        var act = async () => await _sut.UpdateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        await _repository.DidNotReceive().UpdateAsync(Arg.Any<EventAppointment>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: DoctorId owned by person updates DoctorId field")]
    public async Task UpdateAsync_DoctorIdOwned_UpdatesDoctorIdField()
    {
        var eventId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var existing = new EventAppointment { EventId = eventId, Doctor = "Old Doctor", Type = EventAppointmentType.Routine };
        var dto = new UpdateEventAppointmentDto("Old Doctor", doctorId, EventAppointmentType.Routine);
        var updated = new EventAppointment { Id = existing.Id, EventId = eventId, Doctor = "Old Doctor", DoctorId = doctorId, Type = EventAppointmentType.Routine };
        var expected = new EventAppointmentDto(existing.Id, eventId, "Old Doctor", doctorId, "Dr. House", EventAppointmentType.Routine);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(existing, updated);
        _doctorRepository.GetAsync(doctorId, Arg.Any<CancellationToken>()).Returns(new Doctor { Id = doctorId, PersonId = Guid.NewGuid(), Name = "Dr. House" });
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<EventAppointmentDto>(updated).Returns(expected);

        var result = await _sut.UpdateAsync(eventId, dto, CancellationToken.None);

        existing.DoctorId.Should().Be(doctorId);
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "DeleteAsync: not found throws KeyNotFoundException")]
    public async Task DeleteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventAppointment?)null);

        var act = async () => await _sut.DeleteAsync(eventId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "DeleteAsync: found calls repository DeleteAsync with appointment id")]
    public async Task DeleteAsync_Found_CallsDeleteAsync()
    {
        var eventId = Guid.NewGuid();
        var appointment = new EventAppointment { EventId = eventId, Type = EventAppointmentType.Routine };

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(appointment);

        await _sut.DeleteAsync(eventId, CancellationToken.None);

        await _repository.Received(1).DeleteAsync(appointment.Id, Arg.Any<CancellationToken>());
    }
}
