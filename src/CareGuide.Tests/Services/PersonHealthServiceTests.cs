using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.PersonHealth;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class PersonHealthServiceTests
{
    private readonly IPersonHealthRepository _repository;
    private readonly IMapper _mapper;
    private readonly PersonHealthService _sut;

    public PersonHealthServiceTests()
    {
        _repository = Substitute.For<IPersonHealthRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new PersonHealthService(_repository, _mapper);
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
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((PersonHealth?)null);

        var act = async () => await _sut.GetAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var entity = new PersonHealth { Id = id, PersonId = personId, BloodType = BloodType.A_Positive, Height = 1.75m, Weight = 70m };
        var expected = new PersonHealthDto(id, personId, BloodType.A_Positive, 1.75m, 70m, null, DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<PersonHealthDto>(entity).Returns(expected);

        var result = await _sut.GetAsync(id, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "GetAllByPersonAsync: returns mapped paged list")]
    public async Task GetAllByPersonAsync_ReturnsMappedList()
    {
        var personId = Guid.NewGuid();
        var entities = new List<PersonHealth>
        {
            new() { PersonId = personId, BloodType = BloodType.A_Positive, Height = 1.75m, Weight = 70m },
        };
        var dtos = entities.Select(e => new PersonHealthDto(e.Id, personId, e.BloodType, e.Height, e.Weight, null, DateTime.UtcNow, DateTime.UtcNow)).ToList();

        _repository.GetAllByPersonAsync(1, 10, Arg.Any<CancellationToken>()).Returns(entities);
        _mapper.Map<List<PersonHealthDto>>(entities).Returns(dtos);

        var result = await _sut.GetAllByPersonAsync(1, 10, CancellationToken.None);

        result.Should().HaveCount(1);
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
        var createDto = new CreatePersonHealthDto(BloodType.A_Positive, 1.75m, 70m, null);
        var entity = new PersonHealth { PersonId = personId, BloodType = BloodType.A_Positive, Height = 1.75m, Weight = 70m };
        var expected = new PersonHealthDto(entity.Id, personId, BloodType.A_Positive, 1.75m, 70m, null, DateTime.UtcNow, DateTime.UtcNow);

        _mapper.Map<PersonHealth>(createDto).Returns(entity);
        _mapper.Map<PersonHealthDto>(entity).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: empty id throws ArgumentException")]
    public async Task UpdateAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.Empty, new UpdatePersonHealthDto(Guid.Empty, BloodType.A_Positive, 1.75m, 70m, null), CancellationToken.None);

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
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((PersonHealth?)null);

        var act = async () => await _sut.UpdateAsync(id, new UpdatePersonHealthDto(id, BloodType.A_Positive, 1.75m, 70m, null), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var existing = new PersonHealth { Id = id, PersonId = personId, BloodType = BloodType.O_Positive, Height = 1.60m, Weight = 60m };
        var updateDto = new UpdatePersonHealthDto(id, BloodType.A_Positive, 1.75m, 70m, "Healthy");
        var expected = new PersonHealthDto(id, personId, BloodType.A_Positive, 1.75m, 70m, "Healthy", DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<PersonHealthDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        existing.BloodType.Should().Be(BloodType.A_Positive);
        existing.Height.Should().Be(1.75m);
        existing.Weight.Should().Be(70m);
        existing.Description.Should().Be("Healthy");
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "DeleteAsync: empty id throws ArgumentException")]
    public async Task DeleteAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.DeleteAsync(Guid.Empty, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*id*");
    }

    [Fact(DisplayName = "DeleteAsync: not found throws KeyNotFoundException")]
    public async Task DeleteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((PersonHealth?)null);

        var act = async () => await _sut.DeleteAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "DeleteAsync: found calls repository DeleteAsync")]
    public async Task DeleteAsync_Found_CallsDeleteAsync()
    {
        var id = Guid.NewGuid();
        var entity = new PersonHealth { Id = id, PersonId = Guid.NewGuid(), BloodType = BloodType.A_Positive, Height = 1.75m, Weight = 70m };
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);

        await _sut.DeleteAsync(id, CancellationToken.None);

        await _repository.Received(1).DeleteAsync(id, Arg.Any<CancellationToken>());
    }
}
