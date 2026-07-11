using AutoMapper;
using CareGuide.Models.DTOs.Person;
using CareGuide.Models.DTOs.PersonAnnotation;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.Person;
using CareGuide.Models.Mappers.PersonAnnotation;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class PersonMapperTests
{
    private readonly IMapper _mapper;

    public PersonMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<PersonProfileMapper>();
            cfg.AddProfile<PersonAnnotationProfileMapper>();
        });
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    // ── Person → PersonDto ────────────────────────────────────────────────────

    [Fact(DisplayName = "Person→PersonDto: all fields map correctly")]
    public void Map_PersonToDto_MapsAllFields()
    {
        var entity = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Alice",
            Gender = Gender.F,
            Birthday = new DateOnly(1990, 5, 20),
            Picture = "https://cdn.example.com/pic.jpg",
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 6, 1),
        };

        var dto = _mapper.Map<PersonDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.Name.Should().Be("Alice");
        dto.Gender.Should().Be(Gender.F);
        dto.Birthday.Should().Be(entity.Birthday);
        dto.Picture.Should().Be(entity.Picture);
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    [Fact(DisplayName = "Person→PersonDto: null Picture maps to null")]
    public void Map_PersonToDto_NullPicture_MapsToNull()
    {
        var entity = new Person
        {
            Name = "Bob",
            Gender = Gender.M,
            Birthday = new DateOnly(1985, 3, 10),
            Picture = null,
        };

        var dto = _mapper.Map<PersonDto>(entity);

        dto.Picture.Should().BeNull();
    }

    // ── CreatePersonDto → Person ───────────────────────────────────────────────

    [Fact(DisplayName = "CreatePersonDto→Person: Name, Gender, Birthday map correctly")]
    public void Map_CreatePersonDtoToPerson_MapsFields()
    {
        var dto = new CreatePersonDto("Alice", Gender.F, new DateOnly(1990, 5, 20), null);

        var entity = _mapper.Map<Person>(dto);

        entity.Name.Should().Be("Alice");
        entity.Gender.Should().Be(Gender.F);
        entity.Birthday.Should().Be(new DateOnly(1990, 5, 20));
        entity.Id.Should().NotBeEmpty(because: "the Id must be server-generated, never client-supplied");
    }

    // ── PersonAnnotation → PersonAnnotationDto ────────────────────────────────

    [Fact(DisplayName = "PersonAnnotation→PersonAnnotationDto: all fields map correctly")]
    public void Map_PersonAnnotationToDto_MapsAllFields()
    {
        var personId = Guid.NewGuid();
        var entity = new PersonAnnotation
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Details = "Annual checkup",
            FileUrl = "https://cdn.example.com/file.pdf",
            CreatedAt = new DateTime(2024, 3, 1),
            UpdatedAt = new DateTime(2024, 3, 15),
        };

        var dto = _mapper.Map<PersonAnnotationDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.PersonId.Should().Be(personId);
        dto.Details.Should().Be("Annual checkup");
        dto.FileUrl.Should().Be(entity.FileUrl);
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    [Fact(DisplayName = "PersonAnnotation→PersonAnnotationDto: null FileUrl maps to null")]
    public void Map_PersonAnnotationToDto_NullFileUrl_MapsToNull()
    {
        var entity = new PersonAnnotation
        {
            PersonId = Guid.NewGuid(),
            Details = "Note",
            FileUrl = null,
        };

        var dto = _mapper.Map<PersonAnnotationDto>(entity);

        dto.FileUrl.Should().BeNull();
    }

    // ── CreatePersonAnnotationDto → PersonAnnotation ──────────────────────────

    [Fact(DisplayName = "CreatePersonAnnotationDto→PersonAnnotation: Details and FileUrl map correctly")]
    public void Map_CreatePersonAnnotationDtoToEntity_MapsFields()
    {
        var dto = new CreatePersonAnnotationDto("Checkup notes", "https://cdn.example.com/file.pdf");

        var entity = _mapper.Map<PersonAnnotation>(dto);

        entity.Details.Should().Be("Checkup notes");
        entity.FileUrl.Should().Be("https://cdn.example.com/file.pdf");
    }

    // ── UpdatePersonAnnotationDto → PersonAnnotation ──────────────────────────

    [Fact(DisplayName = "UpdatePersonAnnotationDto→PersonAnnotation: Details and FileUrl map correctly")]
    public void Map_UpdatePersonAnnotationDtoToEntity_MapsFields()
    {
        var dto = new UpdatePersonAnnotationDto(Guid.NewGuid(), "Updated notes", null);

        var entity = _mapper.Map<PersonAnnotation>(dto);

        entity.Details.Should().Be("Updated notes");
        entity.FileUrl.Should().BeNull();
    }
}
