using AutoMapper;
using CareGuide.Models.DTOs.Account;
using CareGuide.Models.DTOs.Person;
using CareGuide.Models.DTOs.User;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.Person;
using CareGuide.Models.Mappers.User;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class AccountMapperTests
{
    private readonly IMapper _mapper;

    public AccountMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<AccountToPersonProfileMapper>();
            cfg.AddProfile<AccountToUserProfileMapper>();
            cfg.AddProfile<UserProfileMapper>();
        });
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    // ── CreateAccountDto → CreatePersonDto ───────────────────────────────────

    [Fact(DisplayName = "CreateAccountDto→CreatePersonDto: Name, Gender, Birthday map correctly")]
    public void Map_CreateAccountToCreatePerson_MapsCorrectFields()
    {
        var dto = new CreateAccountDto(
            "alice@test.com",
            "Str0ng@Pass!",
            "Alice Smith",
            Gender.F,
            new DateOnly(1990, 5, 20)
        );

        var result = _mapper.Map<CreatePersonDto>(dto);

        result.Name.Should().Be("Alice Smith");
        result.Gender.Should().Be(Gender.F);
        result.Birthday.Should().Be(new DateOnly(1990, 5, 20));
        result.Picture.Should().BeNull();
    }

    [Fact(DisplayName = "CreatePersonDto: does not expose an Id property")]
    public void CreatePersonDto_DoesNotExposeId()
    {
        typeof(CreatePersonDto).GetProperty("Id").Should().BeNull(because: "the Id must be generated server-side, never client-supplied");
    }

    [Fact(DisplayName = "CreateAccountDto→CreatePersonDto: Email and Password are not mapped")]
    public void Map_CreateAccountToCreatePerson_EmailAndPasswordNotMapped()
    {
        typeof(CreatePersonDto).GetProperty("Email").Should().BeNull(because: "CreatePersonDto must not expose email");
        typeof(CreatePersonDto).GetProperty("Password").Should().BeNull(because: "CreatePersonDto must not expose password");
    }

    // ── CreateAccountDto → CreateUserDto ─────────────────────────────────────

    [Fact(DisplayName = "CreateAccountDto→CreateUserDto: Email and Password map correctly")]
    public void Map_CreateAccountToCreateUser_MapsEmailAndPassword()
    {
        var dto = new CreateAccountDto(
            "alice@test.com",
            "Str0ng@Pass!",
            "Alice Smith",
            Gender.F,
            new DateOnly(1990, 5, 20)
        );

        var result = _mapper.Map<CreateUserDto>(dto);

        result.Email.Should().Be("alice@test.com");
        result.Password.Should().Be("Str0ng@Pass!");
    }

    [Fact(DisplayName = "CreateUserDto: does not expose Id or PersonId properties")]
    public void CreateUserDto_DoesNotExposeIdOrPersonId()
    {
        typeof(CreateUserDto).GetProperty("Id").Should().BeNull(because: "the Id must be generated server-side, never client-supplied");
        typeof(CreateUserDto).GetProperty("PersonId").Should().BeNull(because: "PersonId must be wired server-side by AccountService, never client-supplied");
    }

    // ── User → UserDto ────────────────────────────────────────────────────────

    [Fact(DisplayName = "User→UserDto: all fields map correctly")]
    public void Map_UserToDto_MapsAllFields()
    {
        var personId = Guid.NewGuid();
        var entity = new User
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Email = "alice@test.com",
            Password = "hashed-password",
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 6, 1),
        };

        var dto = _mapper.Map<UserDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.PersonId.Should().Be(personId);
        dto.Email.Should().Be("alice@test.com");
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    [Fact(DisplayName = "User→UserDto: Password is not exposed in DTO")]
    public void Map_UserToDto_PasswordNotInDto()
    {
        var entity = new User
        {
            PersonId = Guid.NewGuid(),
            Email = "alice@test.com",
            Password = "super-secret-hash",
        };

        var dto = _mapper.Map<UserDto>(entity);

        dto.Should().NotBeNull();
        typeof(UserDto).GetProperty("Password").Should().BeNull(because: "UserDto must not expose password");
    }

    // ── CreateUserDto → User ──────────────────────────────────────────────────

    [Fact(DisplayName = "CreateUserDto→User: Email and Password map correctly")]
    public void Map_CreateUserDtoToEntity_MapsEmailAndPassword()
    {
        var dto = new CreateUserDto("alice@test.com", "Str0ng@Pass!");

        var entity = _mapper.Map<User>(dto);

        entity.Email.Should().Be("alice@test.com");
        entity.Password.Should().Be("Str0ng@Pass!");
        entity.Id.Should().NotBeEmpty(because: "the Id must be server-generated, never client-supplied");
    }
}
