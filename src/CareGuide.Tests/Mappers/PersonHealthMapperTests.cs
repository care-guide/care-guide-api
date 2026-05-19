using AutoMapper;
using CareGuide.Models.DTOs.PersonDisease;
using CareGuide.Models.DTOs.PersonFamilyHistory;
using CareGuide.Models.DTOs.PersonHealth;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.PersonDisease;
using CareGuide.Models.Mappers.PersonFamilyHistory;
using CareGuide.Models.Mappers.PersonHealth;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class PersonHealthMapperTests
{
    private readonly IMapper _mapper;

    public PersonHealthMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<PersonHealthProfileMapper>();
            cfg.AddProfile<PersonDiseaseProfileMapper>();
            cfg.AddProfile<PersonFamilyHistoryProfileMapper>();
        });
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    // ── PersonHealth → PersonHealthDto ───────────────────────────────────────

    [Fact(DisplayName = "PersonHealth→PersonHealthDto: all fields map correctly")]
    public void Map_PersonHealthToDto_MapsAllFields()
    {
        var personId = Guid.NewGuid();
        var entity = new PersonHealth
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            BloodType = BloodType.A_Positive,
            Height = 1.75m,
            Weight = 70.5m,
            Description = "Healthy",
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 6, 1),
        };

        var dto = _mapper.Map<PersonHealthDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.PersonId.Should().Be(personId);
        dto.BloodType.Should().Be(BloodType.A_Positive);
        dto.Height.Should().Be(1.75m);
        dto.Weight.Should().Be(70.5m);
        dto.Description.Should().Be("Healthy");
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    [Fact(DisplayName = "PersonHealth→PersonHealthDto: null Description maps to null")]
    public void Map_PersonHealthToDto_NullDescription_MapsToNull()
    {
        var entity = new PersonHealth
        {
            PersonId = Guid.NewGuid(),
            BloodType = BloodType.O_Positive,
            Height = 1.80m,
            Weight = 80m,
            Description = null,
        };

        var dto = _mapper.Map<PersonHealthDto>(entity);

        dto.Description.Should().BeNull();
    }

    // ── CreatePersonHealthDto → PersonHealth ─────────────────────────────────

    [Fact(DisplayName = "CreatePersonHealthDto→PersonHealth: all fields map correctly")]
    public void Map_CreatePersonHealthDtoToEntity_MapsFields()
    {
        var dto = new CreatePersonHealthDto(BloodType.B_Negative, 1.65m, 60m, "Athlete");

        var entity = _mapper.Map<PersonHealth>(dto);

        entity.BloodType.Should().Be(BloodType.B_Negative);
        entity.Height.Should().Be(1.65m);
        entity.Weight.Should().Be(60m);
        entity.Description.Should().Be("Athlete");
    }

    // ── UpdatePersonHealthDto → PersonHealth ─────────────────────────────────

    [Fact(DisplayName = "UpdatePersonHealthDto→PersonHealth: all fields map correctly")]
    public void Map_UpdatePersonHealthDtoToEntity_MapsFields()
    {
        var dto = new UpdatePersonHealthDto(Guid.NewGuid(), BloodType.AB_Positive, 1.70m, 75m, null);

        var entity = _mapper.Map<PersonHealth>(dto);

        entity.BloodType.Should().Be(BloodType.AB_Positive);
        entity.Height.Should().Be(1.70m);
        entity.Weight.Should().Be(75m);
        entity.Description.Should().BeNull();
    }

    // ── PersonDisease → PersonDiseaseDto ─────────────────────────────────────

    [Fact(DisplayName = "PersonDisease→PersonDiseaseDto: all fields map correctly")]
    public void Map_PersonDiseaseToDto_MapsAllFields()
    {
        var personId = Guid.NewGuid();
        var entity = new PersonDisease
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Name = "Hypertension",
            DiagnosisDate = new DateOnly(2020, 3, 15),
            DiseaseType = DiseaseType.Chronic,
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 6, 1),
        };

        var dto = _mapper.Map<PersonDiseaseDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.PersonId.Should().Be(personId);
        dto.Name.Should().Be("Hypertension");
        dto.DiagnosisDate.Should().Be(new DateOnly(2020, 3, 15));
        dto.DiseaseType.Should().Be(DiseaseType.Chronic);
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    [Fact(DisplayName = "PersonDisease→PersonDiseaseDto: null DiagnosisDate maps to null")]
    public void Map_PersonDiseaseToDto_NullDiagnosisDate_MapsToNull()
    {
        var entity = new PersonDisease
        {
            PersonId = Guid.NewGuid(),
            Name = "Diabetes",
            DiagnosisDate = null,
            DiseaseType = DiseaseType.Chronic,
        };

        var dto = _mapper.Map<PersonDiseaseDto>(entity);

        dto.DiagnosisDate.Should().BeNull();
    }

    // ── CreatePersonDiseaseDto → PersonDisease ───────────────────────────────

    [Fact(DisplayName = "CreatePersonDiseaseDto→PersonDisease: all fields map correctly")]
    public void Map_CreatePersonDiseaseDtoToEntity_MapsFields()
    {
        var dto = new CreatePersonDiseaseDto("Asthma", new DateOnly(2018, 7, 1), DiseaseType.Chronic);

        var entity = _mapper.Map<PersonDisease>(dto);

        entity.Name.Should().Be("Asthma");
        entity.DiagnosisDate.Should().Be(new DateOnly(2018, 7, 1));
        entity.DiseaseType.Should().Be(DiseaseType.Chronic);
    }

    // ── PersonFamilyHistory → PersonFamilyHistoryDto ─────────────────────────

    [Fact(DisplayName = "PersonFamilyHistory→PersonFamilyHistoryDto: all fields map correctly")]
    public void Map_PersonFamilyHistoryToDto_MapsAllFields()
    {
        var personId = Guid.NewGuid();
        var entity = new PersonFamilyHistory
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Relationship = "Father",
            Diagnosis = "Heart disease",
            AgeAtDiagnosis = 55,
        };

        var dto = _mapper.Map<PersonFamilyHistoryDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.PersonId.Should().Be(personId);
        dto.Relationship.Should().Be("Father");
        dto.Diagnosis.Should().Be("Heart disease");
        dto.AgeAtDiagnosis.Should().Be(55);
    }

    [Fact(DisplayName = "PersonFamilyHistory→PersonFamilyHistoryDto: null AgeAtDiagnosis maps to null")]
    public void Map_PersonFamilyHistoryToDto_NullAge_MapsToNull()
    {
        var entity = new PersonFamilyHistory
        {
            PersonId = Guid.NewGuid(),
            Relationship = "Mother",
            Diagnosis = "Cancer",
            AgeAtDiagnosis = null,
        };

        var dto = _mapper.Map<PersonFamilyHistoryDto>(entity);

        dto.AgeAtDiagnosis.Should().BeNull();
    }

    // ── CreatePersonFamilyHistoryDto → PersonFamilyHistory ───────────────────

    [Fact(DisplayName = "CreatePersonFamilyHistoryDto→PersonFamilyHistory: all fields map correctly")]
    public void Map_CreatePersonFamilyHistoryDtoToEntity_MapsFields()
    {
        var dto = new CreatePersonFamilyHistoryDto("Brother", "Diabetes", 40);

        var entity = _mapper.Map<PersonFamilyHistory>(dto);

        entity.Relationship.Should().Be("Brother");
        entity.Diagnosis.Should().Be("Diabetes");
        entity.AgeAtDiagnosis.Should().Be(40);
    }
}
