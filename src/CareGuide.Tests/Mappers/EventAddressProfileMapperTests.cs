using AutoMapper;
using CareGuide.Models.DTOs.EventAddress;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.EventAddress;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class EventAddressProfileMapperTests
{
    private readonly IMapper _mapper;

    public EventAddressProfileMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<EventAddressProfileMapper>());
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "EventAddress→EventAddressDto: maps all fields from nested Address")]
    public void Map_EventAddressToDto_MapsAllFields()
    {
        var eventId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var entity = new EventAddress
        {
            EventId = eventId,
            AddressId = addressId,
            Address = new Address
            {
                Id = addressId,
                PostalCode = "12345-000",
                Number = 100,
                Type = AddressType.Clinic,
                City = "Sao Paulo",
                Complement = "Apt 1",
                Country = "Brazil",
                District = "Centro",
                ReferencePoint = "Near the square",
                State = "SP",
            }
        };

        var dto = _mapper.Map<EventAddressDto>(entity);

        dto.EventId.Should().Be(eventId);
        dto.AddressId.Should().Be(addressId);
        dto.PostalCode.Should().Be("12345-000");
        dto.Number.Should().Be(100);
        dto.Type.Should().Be(AddressType.Clinic);
        dto.City.Should().Be("Sao Paulo");
        dto.Complement.Should().Be("Apt 1");
        dto.Country.Should().Be("Brazil");
        dto.District.Should().Be("Centro");
        dto.ReferencePoint.Should().Be("Near the square");
        dto.State.Should().Be("SP");
    }

    [Fact(DisplayName = "EventAddress→EventAddressDto: null Address navigation throws InvalidOperationException")]
    public void Map_EventAddressToDto_NullAddress_ThrowsInvalidOperationException()
    {
        var entity = new EventAddress
        {
            EventId = Guid.NewGuid(),
            AddressId = Guid.NewGuid(),
            Address = null!
        };

        var act = () => _mapper.Map<EventAddressDto>(entity);

        act.Should().Throw<AutoMapperMappingException>()
            .WithInnerException<InvalidOperationException>()
            .WithMessage("Address not found.");
    }
}
