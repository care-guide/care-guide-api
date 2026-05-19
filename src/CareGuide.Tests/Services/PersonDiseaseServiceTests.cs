using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.PersonDisease;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class PersonDiseaseServiceTests
{
    private readonly IPersonDiseaseRepository _repository;
    private readonly IMapper _mapper;
    private readonly PersonDiseaseService _sut;

    public PersonDiseaseServiceTests()
    {
        _repository = Substitute.For<IPersonDiseaseRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new PersonDiseaseService(_repository, _mapper);
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
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((PersonDisease?)null);

        var act = async () => await _sut.GetAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var entity = new PersonDisease { Id = id, PersonId = personId, Name = "Hypertension", DiseaseType = DiseaseType.Chronic };
        var expected = new PersonDiseaseDto(id, personId, "Hypertension", null, DiseaseType.Chronic, DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<PersonDiseaseDto>(entity).Returns(expected);

        var result = await _sut.GetAsync(id, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "GetAllByPersonAsync: returns mapped paged list")]
    public async Task GetAllByPersonAsync_ReturnsMappedList()
    {
        var personId = Guid.NewGuid();
        var entities = new List<PersonDisease>
        {
            new() { PersonId = personId, Name = "Hypertension", DiseaseType = DiseaseType.Chronic },
            new() { PersonId = personId, Name = "Asthma", DiseaseType = DiseaseType.Chronic },
        };
        var dtos = entities.Select(e => new PersonDiseaseDto(e.Id, personId, e.Name, null, e.DiseaseType, DateTime.UtcNow, DateTime.UtcNow)).ToList();

        _repository.GetAllByPersonAsync(1, 10, Arg.Any<CancellationToken>()).Returns(entities);
        _mapper.Map<List<PersonDiseaseDto>>(entities).Returns(dtos);

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
        var createDto = new CreatePersonDiseaseDto("Hypertension", new DateOnly(2020, 1, 1), DiseaseType.Chronic);
        var entity = new PersonDisease { PersonId = personId, Name = "Hypertension", DiseaseType = DiseaseType.Chronic };
        var expected = new PersonDiseaseDto(entity.Id, personId, "Hypertension", new DateOnly(2020, 1, 1), DiseaseType.Chronic, DateTime.UtcNow, DateTime.UtcNow);

        _mapper.Map<PersonDisease>(createDto).Returns(entity);
        _mapper.Map<PersonDiseaseDto>(entity).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: empty id throws ArgumentException")]
    public async Task UpdateAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.Empty, new UpdatePersonDiseaseDto(Guid.Empty, "Name", null, DiseaseType.Chronic), CancellationToken.None);

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
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((PersonDisease?)null);

        var act = async () => await _sut.UpdateAsync(id, new UpdatePersonDiseaseDto(id, "Name", null, DiseaseType.Chronic), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var existing = new PersonDisease { Id = id, PersonId = personId, Name = "Old", DiseaseType = DiseaseType.Chronic };
        var updateDto = new UpdatePersonDiseaseDto(id, "Hypertension", new DateOnly(2021, 3, 10), DiseaseType.Chronic);
        var expected = new PersonDiseaseDto(id, personId, "Hypertension", new DateOnly(2021, 3, 10), DiseaseType.Chronic, DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<PersonDiseaseDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        existing.Name.Should().Be("Hypertension");
        existing.DiagnosisDate.Should().Be(new DateOnly(2021, 3, 10));
        existing.DiseaseType.Should().Be(DiseaseType.Chronic);
        result.Should().Be(expected);
    }
}
