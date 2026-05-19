using AutoMapper;
using CareGuide.Models.DTOs.Doctor;
using CareGuide.Models.DTOs.DoctorSpecialty;
using CareGuide.Models.Entities;
using CareGuide.Models.Mappers.Doctor;
using CareGuide.Models.Mappers.DoctorSpecialty;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class DoctorMapperTests
{
    private readonly IMapper _mapper;

    public DoctorMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<DoctorProfileMapper>();
            cfg.AddProfile<DoctorSpecialtyProfileMapper>();
        });
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    // ── Doctor → DoctorDto ────────────────────────────────────────────────────

    [Fact(DisplayName = "Doctor→DoctorDto: all fields map correctly")]
    public void Map_DoctorToDto_MapsAllFields()
    {
        var entity = new Doctor
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Name = "Dr. House",
            Details = "Diagnostics",
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 6, 1),
        };

        var dto = _mapper.Map<DoctorDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.PersonId.Should().Be(entity.PersonId!.Value);
        dto.Name.Should().Be(entity.Name);
        dto.Details.Should().Be(entity.Details);
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    [Fact(DisplayName = "Doctor→DoctorDto: null Details maps to null")]
    public void Map_DoctorToDto_NullDetails_MapsToNull()
    {
        var entity = new Doctor { PersonId = Guid.NewGuid(), Name = "Dr. Wilson", Details = null };

        var dto = _mapper.Map<DoctorDto>(entity);

        dto.Details.Should().BeNull();
    }

    // ── CreateDoctorDto → Doctor ───────────────────────────────────────────────

    [Fact(DisplayName = "CreateDoctorDto→Doctor: Name and Details map correctly")]
    public void Map_CreateDtoToDoctor_MapsNameAndDetails()
    {
        var dto = new CreateDoctorDto("Dr. House", "Diagnostics");

        var entity = _mapper.Map<Doctor>(dto);

        entity.Name.Should().Be("Dr. House");
        entity.Details.Should().Be("Diagnostics");
    }

    [Fact(DisplayName = "CreateDoctorDto→Doctor: null Details maps to null")]
    public void Map_CreateDtoToDoctor_NullDetails_MapsToNull()
    {
        var dto = new CreateDoctorDto("Dr. House", null);

        var entity = _mapper.Map<Doctor>(dto);

        entity.Details.Should().BeNull();
    }

    // ── UpdateDoctorDto → Doctor ───────────────────────────────────────────────

    [Fact(DisplayName = "UpdateDoctorDto→Doctor: Name and Details map correctly")]
    public void Map_UpdateDtoToDoctor_MapsNameAndDetails()
    {
        var dto = new UpdateDoctorDto(Guid.NewGuid(), "New Name", "New Details");

        var entity = _mapper.Map<Doctor>(dto);

        entity.Name.Should().Be("New Name");
        entity.Details.Should().Be("New Details");
    }

    // ── DoctorSpecialty → DoctorSpecialtyDto ──────────────────────────────────

    [Fact(DisplayName = "DoctorSpecialty→DoctorSpecialtyDto: all fields map correctly")]
    public void Map_DoctorSpecialtyToDto_MapsAllFields()
    {
        var doctorId = Guid.NewGuid();
        var entity = new DoctorSpecialty
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            Name = "Cardiology",
        };

        var dto = _mapper.Map<DoctorSpecialtyDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.DoctorId.Should().Be(doctorId);
        dto.Name.Should().Be("Cardiology");
    }

    // ── CreateDoctorSpecialtyDto → DoctorSpecialty ────────────────────────────

    [Fact(DisplayName = "CreateDoctorSpecialtyDto→DoctorSpecialty: Name maps correctly")]
    public void Map_CreateSpecialtyDtoToEntity_MapsName()
    {
        var dto = new CreateDoctorSpecialtyDto("Neurology");

        var entity = _mapper.Map<DoctorSpecialty>(dto);

        entity.Name.Should().Be("Neurology");
    }

    // ── UpdateDoctorSpecialtyDto → DoctorSpecialty ────────────────────────────

    [Fact(DisplayName = "UpdateDoctorSpecialtyDto→DoctorSpecialty: Name maps correctly")]
    public void Map_UpdateSpecialtyDtoToEntity_MapsName()
    {
        var dto = new UpdateDoctorSpecialtyDto(Guid.NewGuid(), "Oncology");

        var entity = _mapper.Map<DoctorSpecialty>(dto);

        entity.Name.Should().Be("Oncology");
    }
}
