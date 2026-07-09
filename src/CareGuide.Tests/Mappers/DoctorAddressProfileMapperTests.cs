using AutoMapper;
using CareGuide.Models.DTOs.DoctorAddress;
using CareGuide.Models.Entities;
using CareGuide.Models.Mappers.DoctorAddress;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class DoctorAddressProfileMapperTests
{
    private readonly IMapper _mapper;

    public DoctorAddressProfileMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<DoctorAddressProfileMapper>());
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "DoctorAddress→DoctorAddressDto: maps all fields from nested Address")]
    public void Map_DoctorAddressToDto_MapsAllFields()
    {
        var addressId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var entity = new DoctorAddress
        {
            DoctorId = doctorId,
            AddressId = addressId,
            Address = new Address
            {
                Id = addressId,
                PostalCode = "12345-000",
                Number = 100,
                Type = "Home",
                City = "Sao Paulo",
                Complement = "Apt 1",
                Country = "Brazil",
                District = "Center",
                ReferencePoint = "Near park",
                State = "SP"
            }
        };

        var dto = _mapper.Map<DoctorAddressDto>(entity);

        dto.DoctorId.Should().Be(doctorId);
        dto.AddressId.Should().Be(addressId);
        dto.PostalCode.Should().Be("12345-000");
        dto.Number.Should().Be(100);
        dto.Type.Should().Be("Home");
        dto.City.Should().Be("Sao Paulo");
        dto.Complement.Should().Be("Apt 1");
        dto.Country.Should().Be("Brazil");
        dto.District.Should().Be("Center");
        dto.ReferencePoint.Should().Be("Near park");
        dto.State.Should().Be("SP");
    }

    [Fact(DisplayName = "DoctorAddress→DoctorAddressDto: null Address navigation throws InvalidOperationException")]
    public void Map_DoctorAddressToDto_NullAddress_ThrowsInvalidOperationException()
    {
        var entity = new DoctorAddress
        {
            DoctorId = Guid.NewGuid(),
            AddressId = Guid.NewGuid(),
            Address = null!
        };

        var act = () => _mapper.Map<DoctorAddressDto>(entity);

        act.Should().Throw<AutoMapperMappingException>()
            .WithInnerException<InvalidOperationException>()
            .WithMessage("Address not found.");
    }
}
