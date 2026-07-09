using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Address;
using CareGuide.Models.Entities;

namespace CareGuide.Tests.Services;

public class AddressServiceTests
{
    private readonly IAddressRepository _repository;
    private readonly IMapper _mapper;
    private readonly AddressService _sut;

    public AddressServiceTests()
    {
        _repository = Substitute.For<IAddressRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new AddressService(_repository, _mapper);
    }

    [Fact(DisplayName = "GetAsync: empty id throws ArgumentException")]
    public async Task GetAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.GetAsync(Guid.Empty, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*id*");
    }

    [Fact(DisplayName = "GetAsync: address not found throws KeyNotFoundException")]
    public async Task GetAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Address?)null);

        var act = async () => await _sut.GetAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var entity = new Address { Id = id, PostalCode = "12345-000", City = "Sao Paulo", Country = "Brazil", State = "SP" };
        var expected = new AddressDto(id, "12345-000", null, null, "Sao Paulo", null, "Brazil", null, null, "SP", DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<AddressDto>(entity).Returns(expected);

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
        var createDto = new CreateAddressDto("12345-000", 100, "Home", "Sao Paulo", "Apt 1", "Brazil", "Center", "Near park", "SP");
        var entity = new Address { PostalCode = "12345-000", Number = 100, Type = "Home", City = "Sao Paulo", Complement = "Apt 1", Country = "Brazil", District = "Center", ReferencePoint = "Near park", State = "SP" };
        var expected = new AddressDto(entity.Id, "12345-000", 100, "Home", "Sao Paulo", "Apt 1", "Brazil", "Center", "Near park", "SP", DateTime.UtcNow, DateTime.UtcNow);

        _mapper.Map<Address>(createDto).Returns(entity);
        _mapper.Map<AddressDto>(entity).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: empty id throws ArgumentException")]
    public async Task UpdateAsync_EmptyId_ThrowsArgumentException()
    {
        var dto = new UpdateAddressDto(Guid.Empty, "12345-000", null, null, "Sao Paulo", null, "Brazil", null, null, "SP");
        var act = async () => await _sut.UpdateAsync(Guid.Empty, dto, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*id*");
    }

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: address not found throws KeyNotFoundException")]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Address?)null);

        var dto = new UpdateAddressDto(id, "12345-000", null, null, "Sao Paulo", null, "Brazil", null, null, "SP");
        var act = async () => await _sut.UpdateAsync(id, dto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var id = Guid.NewGuid();
        var existing = new Address { Id = id, PostalCode = "00000-000", City = "Old City", Country = "Old Country", State = "OC" };
        var updateDto = new UpdateAddressDto(id, "12345-000", 100, "Home", "Sao Paulo", "Apt 1", "Brazil", "Center", "Near park", "SP");
        var expected = new AddressDto(id, "12345-000", 100, "Home", "Sao Paulo", "Apt 1", "Brazil", "Center", "Near park", "SP", DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<AddressDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        existing.PostalCode.Should().Be("12345-000");
        existing.Number.Should().Be(100);
        existing.Type.Should().Be("Home");
        existing.City.Should().Be("Sao Paulo");
        existing.Complement.Should().Be("Apt 1");
        existing.Country.Should().Be("Brazil");
        existing.District.Should().Be("Center");
        existing.ReferencePoint.Should().Be("Near park");
        existing.State.Should().Be("SP");
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "DeleteByIdsAsync: delegates to repository DeleteManyAsync")]
    public async Task DeleteByIdsAsync_Delegates_ToRepositoryDeleteManyAsync()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        await _sut.DeleteByIdsAsync(ids, CancellationToken.None);

        await _repository.Received(1).DeleteManyAsync(ids, Arg.Any<CancellationToken>());
    }
}
