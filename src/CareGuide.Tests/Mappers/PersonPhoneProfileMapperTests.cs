using AutoMapper;
using CareGuide.Models.DTOs.PersonPhone;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.PersonPhone;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class PersonPhoneProfileMapperTests
{
    private readonly IMapper _mapper;

    public PersonPhoneProfileMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<PersonPhoneProfileMapper>());
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "PersonPhone→PersonPhoneDto: maps all fields from nested Phone")]
    public void Map_PersonPhoneToDto_MapsAllFields()
    {
        var phoneId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var entity = new PersonPhone
        {
            PersonId = personId,
            PhoneId = phoneId,
            Phone = new Phone
            {
                Id = phoneId,
                Number = "998765432",
                AreaCode = "21",
                Type = PhoneType.R,
            }
        };

        var dto = _mapper.Map<PersonPhoneDto>(entity);

        dto.PersonId.Should().Be(personId);
        dto.PhoneId.Should().Be(phoneId);
        dto.Number.Should().Be("998765432");
        dto.AreaCode.Should().Be("21");
        dto.Type.Should().Be(PhoneType.R);
    }

    [Fact(DisplayName = "PersonPhone→PersonPhoneDto: null Phone navigation throws InvalidOperationException")]
    public void Map_PersonPhoneToDto_NullPhone_ThrowsInvalidOperationException()
    {
        var entity = new PersonPhone
        {
            PersonId = Guid.NewGuid(),
            PhoneId = Guid.NewGuid(),
            Phone = null!
        };

        var act = () => _mapper.Map<PersonPhoneDto>(entity);

        act.Should().Throw<AutoMapperMappingException>()
            .WithInnerException<InvalidOperationException>()
            .WithMessage("Phone not found.");
    }
}
