using AutoMapper;
using CareGuide.Models.DTOs.DoctorPhone;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.DoctorPhone;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class DoctorPhoneProfileMapperTests
{
    private readonly IMapper _mapper;

    public DoctorPhoneProfileMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<DoctorPhoneProfileMapper>());
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "DoctorPhone→DoctorPhoneDto: maps all fields from nested Phone")]
    public void Map_DoctorPhoneToDto_MapsAllFields()
    {
        var phoneId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var entity = new DoctorPhone
        {
            DoctorId = doctorId,
            PhoneId = phoneId,
            Phone = new Phone
            {
                Id = phoneId,
                Number = "912345678",
                AreaCode = "11",
                Type = PhoneType.CEL,
            }
        };

        var dto = _mapper.Map<DoctorPhoneDto>(entity);

        dto.DoctorId.Should().Be(doctorId);
        dto.PhoneId.Should().Be(phoneId);
        dto.Number.Should().Be("912345678");
        dto.AreaCode.Should().Be("11");
        dto.Type.Should().Be(PhoneType.CEL);
    }

    [Fact(DisplayName = "DoctorPhone→DoctorPhoneDto: null Phone navigation throws InvalidOperationException")]
    public void Map_DoctorPhoneToDto_NullPhone_ThrowsInvalidOperationException()
    {
        var entity = new DoctorPhone
        {
            DoctorId = Guid.NewGuid(),
            PhoneId = Guid.NewGuid(),
            Phone = null!
        };

        var act = () => _mapper.Map<DoctorPhoneDto>(entity);

        act.Should().Throw<AutoMapperMappingException>()
            .WithInnerException<InvalidOperationException>()
            .WithMessage("Phone not found.");
    }
}
