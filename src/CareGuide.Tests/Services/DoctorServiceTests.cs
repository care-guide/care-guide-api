using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Doctor;
using CareGuide.Models.Entities;

namespace CareGuide.Tests.Services;

public class DoctorServiceTests
{
    private readonly IDoctorRepository _repository;
    private readonly IMapper _mapper;
    private readonly DoctorService _sut;

    public DoctorServiceTests()
    {
        _repository = Substitute.For<IDoctorRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new DoctorService(_repository, _mapper);
    }

    [Fact(DisplayName = "GetAsync: empty id throws ArgumentException")]
    public async Task GetAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.GetAsync(Guid.Empty, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*id*");
    }

    [Fact(DisplayName = "GetAsync: entity not found throws KeyNotFoundException")]
    public async Task GetAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Doctor?)null);

        var act = async () => await _sut.GetAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "GetAsync: found entity returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var doctor = new Doctor { Id = id, PersonId = Guid.NewGuid(), Name = "Dr. House" };
        var expected = new DoctorDto(id, doctor.PersonId!.Value, "Dr. House", null, DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(doctor);
        _mapper.Map<DoctorDto>(doctor).Returns(expected);

        var result = await _sut.GetAsync(id, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CreateAsync: null dto throws ArgumentNullException")]
    public async Task CreateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.CreateAsync(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync: valid dto maps, persists, and returns DTO")]
    public async Task CreateAsync_ValidDto_ReturnsMappedDto()
    {
        var createDto = new CreateDoctorDto("Dr. House", "Diagnostics");
        var entity = new Doctor { PersonId = Guid.NewGuid(), Name = "Dr. House", Details = "Diagnostics" };
        var expected = new DoctorDto(entity.Id, entity.PersonId!.Value, entity.Name, entity.Details, DateTime.UtcNow, DateTime.UtcNow);

        _mapper.Map<Doctor>(createDto).Returns(entity);
        _repository.AddAsync(entity, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<DoctorDto>(entity).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: empty id throws ArgumentException")]
    public async Task UpdateAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.Empty, new UpdateDoctorDto(Guid.Empty, "Name", null), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*id*");
    }

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: entity not found throws KeyNotFoundException")]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Doctor?)null);

        var act = async () => await _sut.UpdateAsync(id, new UpdateDoctorDto(id, "New Name", null), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates entity fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesAndReturnsDto()
    {
        var id = Guid.NewGuid();
        var existing = new Doctor { Id = id, PersonId = Guid.NewGuid(), Name = "Old Name" };
        var updateDto = new UpdateDoctorDto(id, "New Name", "New Details");
        var updated = existing;
        var expected = new DoctorDto(id, existing.PersonId!.Value, "New Name", "New Details", DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(updated);
        _mapper.Map<DoctorDto>(updated).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        result.Name.Should().Be("New Name");
        result.Details.Should().Be("New Details");
    }

    [Fact(DisplayName = "GetAllByPersonAsync: returns mapped list")]
    public async Task GetAllByPersonAsync_ReturnsPagedMappedList()
    {
        var doctors = new List<Doctor>
        {
            new() { PersonId = Guid.NewGuid(), Name = "Dr. A" },
            new() { PersonId = Guid.NewGuid(), Name = "Dr. B" },
        };
        var dtos = doctors.Select(d => new DoctorDto(d.Id, d.PersonId!.Value, d.Name, null, DateTime.UtcNow, DateTime.UtcNow)).ToList();

        _repository.GetAllByPersonAsync(1, 10, Arg.Any<CancellationToken>()).Returns(doctors);
        _mapper.Map<List<DoctorDto>>(doctors).Returns(dtos);

        var result = await _sut.GetAllByPersonAsync(1, 10, CancellationToken.None);

        result.Should().HaveCount(2);
    }
}
