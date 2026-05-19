using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Phone;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class PhoneServiceTests
{
    private readonly IPhoneRepository _repository;
    private readonly IMapper _mapper;
    private readonly PhoneService _sut;

    public PhoneServiceTests()
    {
        _repository = Substitute.For<IPhoneRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new PhoneService(_repository, _mapper);
    }

    [Fact(DisplayName = "GetAsync: empty id throws ArgumentException")]
    public async Task GetAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.GetAsync(Guid.Empty, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*id*");
    }

    [Fact(DisplayName = "GetAsync: phone not found throws KeyNotFoundException")]
    public async Task GetAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Phone?)null);

        var act = async () => await _sut.GetAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var entity = new Phone { Id = id, Number = "912345678", AreaCode = "11", Type = PhoneType.CEL };
        var expected = new PhoneDto(id, "912345678", "11", PhoneType.CEL, DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<PhoneDto>(entity).Returns(expected);

        var result = await _sut.GetAsync(id, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CreateAsync: null dto throws ArgumentNullException")]
    public async Task CreateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.CreateAsync(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync: valid dto persists and returns DTO")]
    public async Task CreateAsync_ValidDto_PersistsAndReturnsDto()
    {
        var createDto = new CreatePhoneDto("912345678", "11", PhoneType.CEL);
        var entity = new Phone { Number = "912345678", AreaCode = "11", Type = PhoneType.CEL };
        var expected = new PhoneDto(entity.Id, "912345678", "11", PhoneType.CEL, DateTime.UtcNow, DateTime.UtcNow);

        _mapper.Map<Phone>(createDto).Returns(entity);
        _mapper.Map<PhoneDto>(entity).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: empty id throws ArgumentException")]
    public async Task UpdateAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.Empty, new UpdatePhoneDto(Guid.Empty, "912345678", "11", PhoneType.CEL), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*id*");
    }

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: phone not found throws KeyNotFoundException")]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Phone?)null);

        var act = async () => await _sut.UpdateAsync(id, new UpdatePhoneDto(id, "912345678", "11", PhoneType.CEL), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var id = Guid.NewGuid();
        var existing = new Phone { Id = id, Number = "11111111", AreaCode = "11", Type = PhoneType.R };
        var updateDto = new UpdatePhoneDto(id, "912345678", "21", PhoneType.CEL);
        var expected = new PhoneDto(id, "912345678", "21", PhoneType.CEL, DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<PhoneDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        existing.Number.Should().Be("912345678");
        existing.AreaCode.Should().Be("21");
        existing.Type.Should().Be(PhoneType.CEL);
        result.Should().Be(expected);
    }
}
