using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.DoctorSpecialty;
using CareGuide.Models.Entities;

namespace CareGuide.Tests.Services;

public class DoctorSpecialtyServiceTests
{
    private readonly IDoctorSpecialtyRepository _repository;
    private readonly IMapper _mapper;
    private readonly DoctorSpecialtyService _sut;

    public DoctorSpecialtyServiceTests()
    {
        _repository = Substitute.For<IDoctorSpecialtyRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new DoctorSpecialtyService(_repository, _mapper);
    }

    [Fact(DisplayName = "GetAllByDoctorAsync: empty results throws KeyNotFoundException")]
    public async Task GetAllByDoctorAsync_EmptyResults_ThrowsKeyNotFoundException()
    {
        var doctorId = Guid.NewGuid();
        _repository.GetAllByDoctorAsync(Arg.Any<int>(), Arg.Any<int>(), doctorId, Arg.Any<CancellationToken>())
            .Returns(new List<DoctorSpecialty>());

        var act = async () => await _sut.GetAllByDoctorAsync(1, 10, doctorId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{doctorId}*");
    }

    [Fact(DisplayName = "GetAllByDoctorAsync: results found returns mapped collection")]
    public async Task GetAllByDoctorAsync_HasResults_ReturnsMappedCollection()
    {
        var doctorId = Guid.NewGuid();
        var specialties = new List<DoctorSpecialty>
        {
            new() { DoctorId = doctorId, Name = "Cardiology" },
        };
        var dtos = specialties.Select(s => new DoctorSpecialtyDto(s.Id, s.DoctorId, s.Name)).ToList();

        _repository.GetAllByDoctorAsync(1, 10, doctorId, Arg.Any<CancellationToken>()).Returns(specialties);
        _mapper.Map<List<DoctorSpecialtyDto>>(specialties).Returns(dtos);

        var result = await _sut.GetAllByDoctorAsync(1, 10, doctorId, CancellationToken.None);

        result.Should().HaveCount(1);
    }

    [Fact(DisplayName = "GetAsync: specialty not found throws UnauthorizedAccessException")]
    public async Task GetAsync_NotFound_ThrowsUnauthorizedAccessException()
    {
        var id = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        _repository.GetByIdAsync(id, doctorId, Arg.Any<CancellationToken>()).Returns((DoctorSpecialty?)null);

        var act = async () => await _sut.GetAsync(id, doctorId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var specialty = new DoctorSpecialty { Id = id, DoctorId = doctorId, Name = "Cardiology" };
        var expected = new DoctorSpecialtyDto(id, doctorId, "Cardiology");

        _repository.GetByIdAsync(id, doctorId, Arg.Any<CancellationToken>()).Returns(specialty);
        _mapper.Map<DoctorSpecialtyDto>(specialty).Returns(expected);

        var result = await _sut.GetAsync(id, doctorId, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CreateAsync: null dto throws ArgumentNullException")]
    public async Task CreateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.CreateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync: sets DoctorId from parameter before persisting")]
    public async Task CreateAsync_ValidDto_SetsDoctorIdOnEntity()
    {
        var doctorId = Guid.NewGuid();
        var createDto = new CreateDoctorSpecialtyDto("Neurology");
        var entity = new DoctorSpecialty { DoctorId = Guid.Empty, Name = "Neurology" };
        var savedEntity = entity;
        var expected = new DoctorSpecialtyDto(entity.Id, doctorId, "Neurology");

        _mapper.Map<DoctorSpecialty>(createDto).Returns(entity);
        _repository.AddAsync(Arg.Any<DoctorSpecialty>(), Arg.Any<CancellationToken>()).Returns(savedEntity);
        _mapper.Map<DoctorSpecialtyDto>(savedEntity).Returns(expected);

        await _sut.CreateAsync(doctorId, createDto, CancellationToken.None);

        entity.DoctorId.Should().Be(doctorId);
    }

    [Fact(DisplayName = "UpdateAsync: id mismatch throws ArgumentException")]
    public async Task UpdateAsync_IdMismatch_ThrowsArgumentException()
    {
        var routeId = Guid.NewGuid();
        var dto = new UpdateDoctorSpecialtyDto(Guid.NewGuid(), "Name");

        var act = async () => await _sut.UpdateAsync(routeId, Guid.NewGuid(), dto, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*route ID*");
    }

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: specialty not owned by doctor throws UnauthorizedAccessException")]
    public async Task UpdateAsync_SpecialtyNotFound_ThrowsUnauthorizedAccessException()
    {
        var id = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var dto = new UpdateDoctorSpecialtyDto(id, "New Name");

        _repository.GetByIdAsync(id, doctorId, Arg.Any<CancellationToken>()).Returns((DoctorSpecialty?)null);

        var act = async () => await _sut.UpdateAsync(id, doctorId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "DeleteByIdsAsync: empty list throws ArgumentException")]
    public async Task DeleteByIdsAsync_EmptyList_ThrowsArgumentException()
    {
        var act = async () => await _sut.DeleteByIdsAsync([], Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*IDs*");
    }

    [Fact(DisplayName = "DeleteByIdsAsync: no valid specialties for doctor throws UnauthorizedAccessException")]
    public async Task DeleteByIdsAsync_NoValidSpecialties_ThrowsUnauthorizedAccessException()
    {
        var ids = new List<Guid> { Guid.NewGuid() };
        var doctorId = Guid.NewGuid();

        _repository.GetManyByIdsAsync(ids, doctorId, Arg.Any<CancellationToken>())
            .Returns(new List<DoctorSpecialty>());

        var act = async () => await _sut.DeleteByIdsAsync(ids, doctorId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "DeleteAllByDoctorAsync: no specialties skips delete call")]
    public async Task DeleteAllByDoctorAsync_NoSpecialties_SkipsDelete()
    {
        var doctorId = Guid.NewGuid();
        _repository.GetAllByDoctorAsync(Arg.Any<int>(), Arg.Any<int>(), doctorId, Arg.Any<CancellationToken>())
            .Returns(new List<DoctorSpecialty>());

        await _sut.DeleteAllByDoctorAsync(doctorId, CancellationToken.None);

        await _repository.DidNotReceive().DeleteAllByDoctorAsync(doctorId, Arg.Any<CancellationToken>());
    }
}
