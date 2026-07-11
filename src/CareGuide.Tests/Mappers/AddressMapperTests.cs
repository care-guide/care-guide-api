using AutoMapper;
using CareGuide.Models.DTOs.Address;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.Address;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class AddressMapperTests
{
    private readonly IMapper _mapper;

    public AddressMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<AddressProfileMapper>());
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "Address→AddressDto: all fields map correctly")]
    public void Map_AddressToDto_MapsAllFields()
    {
        var entity = new Address
        {
            Id = Guid.NewGuid(),
            PostalCode = "12345-000",
            Number = 100,
            Type = AddressType.Clinic,
            City = "Sao Paulo",
            Complement = "Apt 1",
            Country = "Brazil",
            District = "Center",
            ReferencePoint = "Near park",
            State = "SP",
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 6, 1),
        };

        var dto = _mapper.Map<AddressDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.PostalCode.Should().Be("12345-000");
        dto.Number.Should().Be(100);
        dto.Type.Should().Be(AddressType.Clinic);
        dto.City.Should().Be("Sao Paulo");
        dto.Complement.Should().Be("Apt 1");
        dto.Country.Should().Be("Brazil");
        dto.District.Should().Be("Center");
        dto.ReferencePoint.Should().Be("Near park");
        dto.State.Should().Be("SP");
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    [Fact(DisplayName = "CreateAddressDto→Address: all fields map correctly")]
    public void Map_CreateAddressDtoToEntity_MapsFields()
    {
        var dto = new CreateAddressDto("98765-000", 200, AddressType.UrgencyEmergency, "Rio de Janeiro", "Suite 2", "Brazil", "Downtown", "Near mall", "RJ");

        var entity = _mapper.Map<Address>(dto);

        entity.PostalCode.Should().Be("98765-000");
        entity.Number.Should().Be(200);
        entity.Type.Should().Be(AddressType.UrgencyEmergency);
        entity.City.Should().Be("Rio de Janeiro");
        entity.Complement.Should().Be("Suite 2");
        entity.Country.Should().Be("Brazil");
        entity.District.Should().Be("Downtown");
        entity.ReferencePoint.Should().Be("Near mall");
        entity.State.Should().Be("RJ");
    }

    [Fact(DisplayName = "UpdateAddressDto→Address: all fields map correctly")]
    public void Map_UpdateAddressDtoToEntity_MapsFields()
    {
        var dto = new UpdateAddressDto(Guid.NewGuid(), "11111-000", 300, AddressType.Other, "Curitiba", "Block C", "Brazil", "North", "Near school", "PR");

        var entity = _mapper.Map<Address>(dto);

        entity.PostalCode.Should().Be("11111-000");
        entity.Number.Should().Be(300);
        entity.Type.Should().Be(AddressType.Other);
        entity.City.Should().Be("Curitiba");
        entity.Complement.Should().Be("Block C");
        entity.Country.Should().Be("Brazil");
        entity.District.Should().Be("North");
        entity.ReferencePoint.Should().Be("Near school");
        entity.State.Should().Be("PR");
    }
}
