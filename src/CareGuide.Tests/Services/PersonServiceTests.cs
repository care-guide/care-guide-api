using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Person;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class PersonServiceTests
{
    private readonly IPersonRepository _repository;
    private readonly IMapper _mapper;
    private readonly PersonService _sut;

    public PersonServiceTests()
    {
        _repository = Substitute.For<IPersonRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new PersonService(_repository, _mapper);
    }

    [Fact(DisplayName = "GetAsync: person not found throws KeyNotFoundException")]
    public async Task GetAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Person?)null);

        var act = async () => await _sut.GetAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var entity = new Person { Id = id, Name = "Alice", Gender = Gender.F, Birthday = new DateOnly(1990, 1, 1) };
        var expected = new PersonDto(id, "Alice", null, Gender.F, new DateOnly(1990, 1, 1), DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<PersonDto>(entity).Returns(expected);

        var result = await _sut.GetAsync(id, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "GetAllAsync: returns mapped paged list")]
    public async Task GetAllAsync_ReturnsMappedList()
    {
        var persons = new List<Person>
        {
            new() { Name = "Alice", Gender = Gender.F, Birthday = new DateOnly(1990, 1, 1) },
            new() { Name = "Bob", Gender = Gender.M, Birthday = new DateOnly(1985, 3, 10) },
        };
        var dtos = persons.Select(p => new PersonDto(p.Id, p.Name, null, p.Gender, p.Birthday, DateTime.UtcNow, DateTime.UtcNow)).ToList();

        _repository.GetAllAsync(1, 10, Arg.Any<CancellationToken>()).Returns(persons);
        _mapper.Map<List<PersonDto>>(persons).Returns(dtos);

        var result = await _sut.GetAllAsync(1, 10, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact(DisplayName = "CreateAsync: persists mapped entity and returns DTO")]
    public async Task CreateAsync_ValidDto_PersistsAndReturnsDto()
    {
        var createDto = new CreatePersonDto(Guid.NewGuid(), "Alice", Gender.F, new DateOnly(1990, 1, 1), null);
        var entity = new Person { Name = "Alice", Gender = Gender.F, Birthday = new DateOnly(1990, 1, 1) };
        var expected = new PersonDto(entity.Id, "Alice", null, Gender.F, new DateOnly(1990, 1, 1), DateTime.UtcNow, DateTime.UtcNow);

        _mapper.Map<Person>(createDto).Returns(entity);
        _mapper.Map<PersonDto>(entity).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: person not found throws KeyNotFoundException")]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Person?)null);
        var updateDto = new PersonDto(id, "Alice", null, Gender.F, new DateOnly(1990, 1, 1), DateTime.UtcNow, DateTime.UtcNow);

        var act = async () => await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var id = Guid.NewGuid();
        var existing = new Person { Id = id, Name = "Old", Gender = Gender.M, Birthday = new DateOnly(1980, 1, 1) };
        var updateDto = new PersonDto(id, "New Name", "http://pic.url", Gender.F, new DateOnly(1990, 5, 10), DateTime.UtcNow, DateTime.UtcNow);
        var expected = updateDto;

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<PersonDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        existing.Name.Should().Be("New Name");
        existing.Gender.Should().Be(Gender.F);
        existing.Birthday.Should().Be(new DateOnly(1990, 5, 10));
        existing.Picture.Should().Be("http://pic.url");
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "DeleteAsync: person not found throws KeyNotFoundException")]
    public async Task DeleteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Person?)null);

        var act = async () => await _sut.DeleteAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "DeleteAsync: found calls repository DeleteAsync")]
    public async Task DeleteAsync_Found_CallsDeleteAsync()
    {
        var id = Guid.NewGuid();
        var entity = new Person { Id = id, Name = "Alice", Gender = Gender.F, Birthday = new DateOnly(1990, 1, 1) };
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);

        await _sut.DeleteAsync(id, CancellationToken.None);

        await _repository.Received(1).DeleteAsync(id, Arg.Any<CancellationToken>());
    }
}
