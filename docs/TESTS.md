# Testes

## Visão Geral

O projeto `CareGuide.Tests` contém testes unitários para as camadas de **Serviços**, **Validadores** e **Mappers** da solução. Os testes não dependem de banco de dados, infraestrutura externa ou configuração de ambiente — são completamente isolados e executados em memória.

## Tecnologias

| Biblioteca | Versão | Finalidade |
|---|---|---|
| [xUnit](https://xunit.net/) | 2.9.3 | Framework de testes |
| [NSubstitute](https://nsubstitute.github.io/) | 5.3.0 | Mocking de dependências |
| [FluentAssertions](https://fluentassertions.com/) | 6.12.2 | Assertions legíveis |
| [AutoMapper](https://automapper.org/) | 16.1.1 | Teste dos profiles de mapeamento |

## Estrutura dos Testes

```
CareGuide.Tests/
├── Helpers/
│   └── PasswordManagerHelperTests.cs       # Hash, validação e força de senha
├── Mappers/
│   ├── AccountMapperTests.cs               # CreateAccountDto → Person/User/UserDto
│   ├── DoctorMapperTests.cs                # Doctor ↔ DTO, DoctorSpecialty ↔ DTO
│   ├── DoctorPhoneProfileMapperTests.cs    # DoctorPhone → DoctorPhoneDto
│   ├── PersonHealthMapperTests.cs          # PersonHealth, PersonDisease, PersonFamilyHistory ↔ DTOs
│   ├── PersonMapperTests.cs                # Person ↔ DTO, PersonAnnotation ↔ DTO
│   ├── PersonPhoneProfileMapperTests.cs    # PersonPhone → PersonPhoneDto
│   └── PhoneMapperTests.cs                 # Phone ↔ DTO
├── Services/
│   ├── AccountServiceTests.cs              # Criação de conta, login, refresh token, logout, exclusão
│   ├── DoctorPhoneServiceTests.cs          # CRUD de telefones de médico com UoW
│   ├── DoctorServiceTests.cs               # CRUD de médicos
│   ├── DoctorSpecialtyServiceTests.cs      # CRUD de especialidades
│   ├── PersonAnnotationServiceTests.cs     # CRUD de anotações
│   ├── PersonDiseaseServiceTests.cs        # CRUD de doenças
│   ├── PersonFamilyHistoryServiceTests.cs  # CRUD de histórico familiar
│   ├── PersonHealthServiceTests.cs         # CRUD de saúde + exclusão individual
│   ├── PersonPhoneServiceTests.cs          # CRUD de telefones de pessoa com UoW
│   ├── PersonServiceTests.cs               # CRUD de pessoas
│   ├── PhoneServiceTests.cs                # CRUD de telefones
│   ├── RefreshTokenServiceTests.cs         # Criação, rotação e invalidação de tokens
│   └── UserServiceTests.cs                 # CRUD de usuários com hash de senha
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

## Padrão Utilizado

Todos os testes seguem o padrão **AAA (Arrange, Act, Assert)**:

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

Dependências externas (repositórios, serviços, mapper) são substituídas por **substitutos** via NSubstitute. Nenhum teste acessa banco de dados ou rede.

## Como Executar

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) instalado.

### Todos os testes

```bash
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj
```

### Com output detalhado

```bash
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj --verbosity normal
```

### Filtrando por categoria

```bash
# Apenas testes de serviços
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj --filter "FullyQualifiedName~Services"

# Apenas testes de validadores
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj --filter "FullyQualifiedName~Validators"

# Apenas testes de mappers
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj --filter "FullyQualifiedName~Mappers"
```

### Pelo nome do DisplayName

```bash
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj --filter "DisplayName~rollback"
```

## Resultado Esperado

```
Aprovado!  – Com falha: 0, Aprovado: 307, Ignorado: 0, Total: 307
```
