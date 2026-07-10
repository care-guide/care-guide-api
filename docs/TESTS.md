# Tests

> Test conventions (AAA, NSubstitute, FluentAssertions) and where to create each file: see
> [AGENTS.md](../AGENTS.md) → Tests.

## Overview

The `CareGuide.Tests` project contains unit tests for the **Services**, **Validators** and **Mappers**
layers of the solution. The tests do not depend on a database, external infrastructure or environment
configuration — they are fully isolated and run in memory.

## Technologies

| Library | Version | Purpose |
|---|---|---|
| [xUnit](https://xunit.net/) | 2.9.3 | Test framework |
| [NSubstitute](https://nsubstitute.github.io/) | 5.3.0 | Dependency mocking |
| [FluentAssertions](https://fluentassertions.com/) | 6.12.2 | Readable assertions |
| [AutoMapper](https://automapper.org/) | 16.1.1 | Testing the mapping profiles |

## Test Structure

```
CareGuide.Tests/
├── Helpers/
│   └── PasswordManagerHelperTests.cs       # Password hashing, validation and strength
├── Mappers/
│   ├── AccountMapperTests.cs               # CreateAccountDto → Person/User/UserDto
│   ├── DoctorMapperTests.cs                # Doctor ↔ DTO, DoctorSpecialty ↔ DTO
│   ├── DoctorPhoneProfileMapperTests.cs    # DoctorPhone → DoctorPhoneDto
│   ├── PersonHealthMapperTests.cs          # PersonHealth, PersonDisease, PersonFamilyHistory ↔ DTOs
│   ├── PersonMapperTests.cs                # Person ↔ DTO, PersonAnnotation ↔ DTO
│   ├── PersonPhoneProfileMapperTests.cs    # PersonPhone → PersonPhoneDto
│   └── PhoneMapperTests.cs                 # Phone ↔ DTO
├── Services/
│   ├── AccountServiceTests.cs              # Account creation, login, refresh token, logout, deletion
│   ├── DoctorPhoneServiceTests.cs          # Doctor phone CRUD with UoW
│   ├── DoctorServiceTests.cs               # Doctor CRUD
│   ├── DoctorSpecialtyServiceTests.cs      # Specialty CRUD
│   ├── PersonAnnotationServiceTests.cs     # Annotation CRUD
│   ├── PersonDiseaseServiceTests.cs        # Disease CRUD
│   ├── PersonFamilyHistoryServiceTests.cs  # Family history CRUD
│   ├── PersonHealthServiceTests.cs         # Health CRUD + individual deletion
│   ├── PersonPhoneServiceTests.cs          # Person phone CRUD with UoW
│   ├── PersonServiceTests.cs               # Person CRUD
│   ├── PhoneServiceTests.cs                # Phone CRUD
│   ├── RefreshTokenServiceTests.cs         # Token creation, rotation and invalidation
│   └── UserServiceTests.cs                 # User CRUD with password hashing
└── Validators/
    ├── CreateAccountDtoValidatorTests.cs
    ├── CreateDoctorDtoValidatorTests.cs
    ├── DoctorSpecialtyDtoValidatorTests.cs
    ├── PersonAnnotationDtoValidatorTests.cs
    ├── PersonDiseaseDtoValidatorTests.cs
    ├── PersonFamilyHistoryDtoValidatorTests.cs
    ├── PersonHealthDtoValidatorTests.cs
    ├── PhoneDtoValidatorTests.cs
    └── UpdateDoctorDtoValidatorTests.cs
```

## Pattern Used

All tests follow the **AAA (Arrange, Act, Assert)** pattern:

```csharp
[Fact(DisplayName = "GetAsync: not found throws KeyNotFoundException")]
public async Task GetAsync_NotFound_ThrowsKeyNotFoundException()
{
    // Arrange
    var id = Guid.NewGuid();
    _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Person?)null);

    // Act
    var act = async () => await _sut.GetAsync(id, CancellationToken.None);

    // Assert
    await act.Should().ThrowAsync<KeyNotFoundException>();
}
```

External dependencies (repositories, services, mapper) are replaced by **substitutes** via NSubstitute.
No test accesses a database or the network.

## How to Run

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) installed.

### All tests

```bash
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj
```

### With detailed output

```bash
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj --verbosity normal
```

### Filtering by category

```bash
# Service tests only
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj --filter "FullyQualifiedName~Services"

# Validator tests only
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj --filter "FullyQualifiedName~Validators"

# Mapper tests only
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj --filter "FullyQualifiedName~Mappers"
```

### By DisplayName

```bash
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj --filter "DisplayName~rollback"
```

## Expected Result

All tests must pass — the number that matters is **`Failed: 0`**:

```
Passed!  – Failed: 0, Passed: <N>, Skipped: 0, Total: <N>
```

> The total (`<N>`) grows as new tests are added; do not hard-code it in the documentation.

---

[Documentation index](README.md) · [Project README](../README.md)
