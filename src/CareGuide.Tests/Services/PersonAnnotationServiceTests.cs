using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.PersonAnnotation;
using CareGuide.Models.Entities;

namespace CareGuide.Tests.Services;

public class PersonAnnotationServiceTests
{
    private readonly IPersonAnnotationRepository _repository;
    private readonly IMapper _mapper;
    private readonly PersonAnnotationService _sut;

    public PersonAnnotationServiceTests()
    {
        _repository = Substitute.For<IPersonAnnotationRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new PersonAnnotationService(_repository, _mapper);
    }

    [Fact(DisplayName = "GetAsync: empty id throws ArgumentException")]
    public async Task GetAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.GetAsync(Guid.Empty, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*id*");
    }

    [Fact(DisplayName = "GetAsync: annotation not found throws KeyNotFoundException")]
    public async Task GetAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((PersonAnnotation?)null);

        var act = async () => await _sut.GetAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var entity = new PersonAnnotation { Id = id, PersonId = personId, Details = "Annual checkup" };
        var expected = new PersonAnnotationDto(id, personId, "Annual checkup", null, DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<PersonAnnotationDto>(entity).Returns(expected);

        var result = await _sut.GetAsync(id, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CreateAsync: null dto throws ArgumentNullException")]
    public async Task CreateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.CreateAsync(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync: valid dto persists entity and returns DTO")]
    public async Task CreateAsync_ValidDto_PersistsAndReturnsDto()
    {
        var personId = Guid.NewGuid();
        var createDto = new CreatePersonAnnotationDto("Checkup notes", null);
        var entity = new PersonAnnotation { PersonId = personId, Details = "Checkup notes" };
        var expected = new PersonAnnotationDto(entity.Id, personId, "Checkup notes", null, DateTime.UtcNow, DateTime.UtcNow);

        _mapper.Map<PersonAnnotation>(createDto).Returns(entity);
        _repository.AddAsync(entity, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<PersonAnnotationDto>(entity).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: empty id throws ArgumentException")]
    public async Task UpdateAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.Empty, new UpdatePersonAnnotationDto(Guid.Empty, "Details", null), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*id*");
    }

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: annotation not found throws KeyNotFoundException")]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((PersonAnnotation?)null);

        var act = async () => await _sut.UpdateAsync(id, new UpdatePersonAnnotationDto(id, "Updated", null), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var existing = new PersonAnnotation { Id = id, PersonId = personId, Details = "Old details" };
        var updateDto = new UpdatePersonAnnotationDto(id, "New details", "http://file.url");
        var expected = new PersonAnnotationDto(id, personId, "New details", "http://file.url", DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<PersonAnnotationDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        existing.Details.Should().Be("New details");
        existing.FileUrl.Should().Be("http://file.url");
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "GetAllByPersonAsync: returns mapped paged list")]
    public async Task GetAllByPersonAsync_ReturnsMappedList()
    {
        var personId = Guid.NewGuid();
        var annotations = new List<PersonAnnotation>
        {
            new() { PersonId = personId, Details = "Note 1" },
            new() { PersonId = personId, Details = "Note 2" },
        };
        var dtos = annotations.Select(a => new PersonAnnotationDto(a.Id, personId, a.Details, null, DateTime.UtcNow, DateTime.UtcNow)).ToList();

        _repository.GetAllByPersonAsync(1, 10, Arg.Any<CancellationToken>()).Returns(annotations);
        _mapper.Map<List<PersonAnnotationDto>>(annotations).Returns(dtos);

        var result = await _sut.GetAllByPersonAsync(1, 10, CancellationToken.None);

        result.Should().HaveCount(2);
    }
}
