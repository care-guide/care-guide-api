using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Account;
using CareGuide.Models.DTOs.Person;
using CareGuide.Models.DTOs.User;
using CareGuide.Models.Entities;

namespace CareGuide.Tests.Services;

public class UserServiceTests
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _repository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new UserService(_repository, _mapper);
    }

    [Fact(DisplayName = "GetByIdAsync: user not found throws KeyNotFoundException")]
    public async Task GetByIdAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = async () => await _sut.GetByIdAsync(id);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "GetByIdAsync: user found returns entity")]
    public async Task GetByIdAsync_Found_ReturnsUser()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, PersonId = Guid.NewGuid(), Email = "test@test.com", Password = "hash" };

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.GetByIdAsync(id);

        result.Id.Should().Be(id);
    }

    [Fact(DisplayName = "GetByIdDtoAsync: maps found user to DTO")]
    public async Task GetByIdDtoAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var user = new User { Id = id, PersonId = personId, Email = "test@test.com", Password = "hash" };
        var expected = new UserDto(id, personId, "test@test.com", DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(user);
        _mapper.Map<UserDto>(user).Returns(expected);

        var result = await _sut.GetByIdDtoAsync(id);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CreateAsync: duplicate email throws InvalidOperationException")]
    public async Task CreateAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var personDto = new PersonDto(Guid.NewGuid(), "Test User", null, Models.Enums.Gender.F, DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), DateTime.UtcNow, DateTime.UtcNow);
        var createUserDto = new CreateUserDto(Guid.NewGuid(), personDto.Id, "existing@test.com", "Str0ng@Pass!");
        var existingUser = new User { PersonId = personDto.Id, Email = "existing@test.com", Password = "hash" };

        _repository.GetByEmailAsync(createUserDto.Email, Arg.Any<CancellationToken>()).Returns(existingUser);

        var act = async () => await _sut.CreateAsync(personDto, createUserDto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already registered");
    }

    [Fact(DisplayName = "CreateAsync: weak password throws InvalidOperationException")]
    public async Task CreateAsync_WeakPassword_ThrowsInvalidOperationException()
    {
        var personDto = new PersonDto(Guid.NewGuid(), "Test User", null, Models.Enums.Gender.F, DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), DateTime.UtcNow, DateTime.UtcNow);
        var createUserDto = new CreateUserDto(Guid.NewGuid(), personDto.Id, "new@test.com", "weakpassword");

        _repository.GetByEmailAsync(createUserDto.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = async () => await _sut.CreateAsync(personDto, createUserDto);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "CreateAsync: valid data hashes password and persists user")]
    public async Task CreateAsync_ValidData_HashesPasswordAndPersists()
    {
        var personDto = new PersonDto(Guid.NewGuid(), "Test User", null, Models.Enums.Gender.F, DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), DateTime.UtcNow, DateTime.UtcNow);
        var createUserDto = new CreateUserDto(Guid.NewGuid(), personDto.Id, "new@test.com", "Str0ng@Pass!");
        var userEntity = new User { PersonId = personDto.Id, Email = "new@test.com", Password = "Str0ng@Pass!" };
        var expected = new UserDto(userEntity.Id, personDto.Id, "new@test.com", DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetByEmailAsync(createUserDto.Email, Arg.Any<CancellationToken>()).Returns((User?)null);
        _mapper.Map<User>(createUserDto).Returns(userEntity);
        _repository.AddAsync(userEntity, Arg.Any<CancellationToken>()).Returns(userEntity);
        _mapper.Map<UserDto>(userEntity).Returns(expected);

        var result = await _sut.CreateAsync(personDto, createUserDto);

        userEntity.Password.Should().NotBe("Str0ng@Pass!", because: "password must be hashed before persisting");
        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(userEntity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdatePasswordAsync: user not found throws KeyNotFoundException")]
    public async Task UpdatePasswordAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = async () => await _sut.UpdatePasswordAsync(id, new UpdatePasswordAccountDto("Str0ng@Pass!"));

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "UpdatePasswordAsync: same password throws InvalidOperationException")]
    public async Task UpdatePasswordAsync_SamePassword_ThrowsInvalidOperationException()
    {
        var id = Guid.NewGuid();
        var plainPassword = "Str0ng@Pass!";
        var hashedPassword = Security.Helpers.PasswordManagerHelper.HashPassword(plainPassword);
        var user = new User { Id = id, PersonId = Guid.NewGuid(), Email = "test@test.com", Password = hashedPassword };

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(user);

        var act = async () => await _sut.UpdatePasswordAsync(id, new UpdatePasswordAccountDto(plainPassword));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*same as the current password*");
    }

    [Fact(DisplayName = "DeleteAsync: user not found throws KeyNotFoundException")]
    public async Task DeleteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = async () => await _sut.DeleteAsync(id);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "DeleteAsync: existing user calls repository delete")]
    public async Task DeleteAsync_ExistingUser_CallsRepositoryDelete()
    {
        var id = Guid.NewGuid();
        var user = new User { Id = id, PersonId = Guid.NewGuid(), Email = "test@test.com", Password = "hash" };

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(user);
        _repository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(user);

        await _sut.DeleteAsync(id);

        await _repository.Received(1).DeleteAsync(id, Arg.Any<CancellationToken>());
    }
}
