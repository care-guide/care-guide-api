using AutoMapper;
using CareGuide.Models.DTOs.Phone;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.Phone;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class PhoneMapperTests
{
    private readonly IMapper _mapper;

    public PhoneMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg => cfg.AddProfile<PhoneProfileMapper>());
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    // ── Phone → PhoneDto ──────────────────────────────────────────────────────

    [Fact(DisplayName = "Phone→PhoneDto: all fields map correctly")]
    public void Map_PhoneToDto_MapsAllFields()
    {
        var entity = new Phone
        {
            Id = Guid.NewGuid(),
            Number = "912345678",
            AreaCode = "11",
            Type = PhoneType.CEL,
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 6, 1),
        };

        var dto = _mapper.Map<PhoneDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.Number.Should().Be("912345678");
        dto.AreaCode.Should().Be("11");
        dto.Type.Should().Be(PhoneType.CEL);
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    // ── CreatePhoneDto → Phone ────────────────────────────────────────────────

    [Fact(DisplayName = "CreatePhoneDto→Phone: all fields map correctly")]
    public void Map_CreatePhoneDtoToEntity_MapsFields()
    {
        var dto = new CreatePhoneDto("987654321", "31", PhoneType.R);

        var entity = _mapper.Map<Phone>(dto);

        entity.Number.Should().Be("987654321");
        entity.AreaCode.Should().Be("31");
        entity.Type.Should().Be(PhoneType.R);
    }

    // ── UpdatePhoneDto → Phone ────────────────────────────────────────────────

    [Fact(DisplayName = "UpdatePhoneDto→Phone: all fields map correctly")]
    public void Map_UpdatePhoneDtoToEntity_MapsFields()
    {
        var dto = new UpdatePhoneDto(Guid.NewGuid(), "111222333", "47", PhoneType.O);

        var entity = _mapper.Map<Phone>(dto);

        entity.Number.Should().Be("111222333");
        entity.AreaCode.Should().Be("47");
        entity.Type.Should().Be(PhoneType.O);
    }
}
