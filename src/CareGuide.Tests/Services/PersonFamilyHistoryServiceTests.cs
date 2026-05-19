using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.PersonFamilyHistory;
using CareGuide.Models.Entities;

namespace CareGuide.Tests.Services;

public class PersonFamilyHistoryServiceTests
{
    private readonly IPersonFamilyHistoryRepository _repository;
    private readonly IMapper _mapper;
    private readonly PersonFamilyHistoryService _sut;

    public PersonFamilyHistoryServiceTests()
    {
        _repository = Substitute.For<IPersonFamilyHistoryRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new PersonFamilyHistoryService(_repository, _mapper);
    }

    [Fact(DisplayName = "GetAsync: empty id throws ArgumentException")]
    public async Task GetAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.GetAsync(Guid.Empty, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*id*");
    }

    [Fact(DisplayName = "GetAsync: not found throws KeyNotFoundException")]
    public async Task GetAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((PersonFamilyHistory?)null);

        var act = async () => await _sut.GetAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var entity = new PersonFamilyHistory { Id = id, PersonId = personId, Relationship = "Father", Diagnosis = "Heart disease", AgeAtDiagnosis = 55 };
        var expected = new PersonFamilyHistoryDto(id, personId, "Father", "Heart disease", 55);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<PersonFamilyHistoryDto>(entity).Returns(expected);

        var result = await _sut.GetAsync(id, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "GetAllByPersonAsync: returns mapped paged list")]
    public async Task GetAllByPersonAsync_ReturnsMappedList()
    {
        var personId = Guid.NewGuid();
        var entities = new List<PersonFamilyHistory>
        {
            new() { PersonId = personId, Relationship = "Father", Diagnosis = "Heart disease" },
            new() { PersonId = personId, Relationship = "Mother", Diagnosis = "Cancer" },
        };
        var dtos = entities.Select(e => new PersonFamilyHistoryDto(e.Id, personId, e.Relationship, e.Diagnosis, null)).ToList();

        _repository.GetAllByPersonAsync(1, 10, Arg.Any<CancellationToken>()).Returns(entities);
        _mapper.Map<List<PersonFamilyHistoryDto>>(entities).Returns(dtos);

        var result = await _sut.GetAllByPersonAsync(1, 10, CancellationToken.None);

        result.Should().HaveCount(2);
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
        var createDto = new CreatePersonFamilyHistoryDto("Father", "Heart disease", 55);
        var entity = new PersonFamilyHistory { PersonId = personId, Relationship = "Father", Diagnosis = "Heart disease", AgeAtDiagnosis = 55 };
        var expected = new PersonFamilyHistoryDto(entity.Id, personId, "Father", "Heart disease", 55);

        _mapper.Map<PersonFamilyHistory>(createDto).Returns(entity);
        _mapper.Map<PersonFamilyHistoryDto>(entity).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: empty id throws ArgumentException")]
    public async Task UpdateAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.Empty, new UpdatePersonFamilyHistoryDto(Guid.Empty, "Father", "Diabetes", null), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*id*");
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
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((PersonFamilyHistory?)null);

        var act = async () => await _sut.UpdateAsync(id, new UpdatePersonFamilyHistoryDto(id, "Father", "Diabetes", null), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var existing = new PersonFamilyHistory { Id = id, PersonId = personId, Relationship = "Father", Diagnosis = "Old", AgeAtDiagnosis = 40 };
        var updateDto = new UpdatePersonFamilyHistoryDto(id, "Brother", "Diabetes", 30);
        var expected = new PersonFamilyHistoryDto(id, personId, "Brother", "Diabetes", 30);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<PersonFamilyHistoryDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        existing.Relationship.Should().Be("Brother");
        existing.Diagnosis.Should().Be("Diabetes");
        existing.AgeAtDiagnosis.Should().Be(30);
        result.Should().Be(expected);
    }
}
