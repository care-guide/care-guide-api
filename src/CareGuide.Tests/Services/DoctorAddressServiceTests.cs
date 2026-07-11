using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.DoctorAddress;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class DoctorAddressServiceTests
{
    private readonly IDoctorAddressRepository _repository;
    private readonly IAddressRepository _addressRepository;
    private readonly IMapper _mapper;
    private readonly DoctorAddressService _sut;

    public DoctorAddressServiceTests()
    {
        _repository = Substitute.For<IDoctorAddressRepository>();
        _addressRepository = Substitute.For<IAddressRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new DoctorAddressService(_repository, _addressRepository, _mapper);
    }

    private static Address MakeAddress(Guid id) => new()
    {
        Id = id,
        PostalCode = "12345-000",
        Type = AddressType.Clinic,
        City = "Sao Paulo",
        Country = "Brazil",
        State = "SP"
    };

    [Fact(DisplayName = "GetAllByDoctorAsync: empty results throws KeyNotFoundException")]
    public async Task GetAllByDoctorAsync_EmptyResults_ThrowsKeyNotFoundException()
    {
        var doctorId = Guid.NewGuid();
        _repository.GetAllByDoctorWithAddressesAsync(Arg.Any<int>(), Arg.Any<int>(), doctorId, Arg.Any<CancellationToken>())
            .Returns(new List<DoctorAddress>());

        var act = async () => await _sut.GetAllByDoctorAsync(1, 10, doctorId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{doctorId}*");
    }

    [Fact(DisplayName = "GetAllByDoctorAsync: results found returns mapped collection")]
    public async Task GetAllByDoctorAsync_HasResults_ReturnsMappedCollection()
    {
        var doctorId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var doctorAddresses = new List<DoctorAddress>
        {
            new() { DoctorId = doctorId, AddressId = addressId, Address = MakeAddress(addressId) }
        };
        var dtos = new List<DoctorAddressDto> { new(doctorId, addressId, "12345-000", null, AddressType.Clinic, "Sao Paulo", null, "Brazil", null, null, "SP") };

        _repository.GetAllByDoctorWithAddressesAsync(1, 10, doctorId, Arg.Any<CancellationToken>()).Returns(doctorAddresses);
        _mapper.Map<List<DoctorAddressDto>>(doctorAddresses).Returns(dtos);

        var result = await _sut.GetAllByDoctorAsync(1, 10, doctorId, CancellationToken.None);

        result.Should().HaveCount(1);
    }

    [Fact(DisplayName = "GetAsync: not found throws UnauthorizedAccessException")]
    public async Task GetAsync_NotFound_ThrowsUnauthorizedAccessException()
    {
        var addressId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        _repository.GetByDoctorWithAddressAsync(addressId, doctorId, Arg.Any<CancellationToken>()).Returns((DoctorAddress?)null);

        var act = async () => await _sut.GetAsync(addressId, doctorId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var doctorId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var doctorAddress = new DoctorAddress { DoctorId = doctorId, AddressId = addressId, Address = MakeAddress(addressId) };
        var expected = new DoctorAddressDto(doctorId, addressId, "12345-000", null, AddressType.Clinic, "Sao Paulo", null, "Brazil", null, null, "SP");

        _repository.GetByDoctorWithAddressAsync(addressId, doctorId, Arg.Any<CancellationToken>()).Returns(doctorAddress);
        _mapper.Map<DoctorAddressDto>(doctorAddress).Returns(expected);

        var result = await _sut.GetAsync(addressId, doctorId, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CreateAsync: null dto throws ArgumentNullException")]
    public async Task CreateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.CreateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync: address does not exist throws KeyNotFoundException")]
    public async Task CreateAsync_AddressNotFound_ThrowsKeyNotFoundException()
    {
        var doctorId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var dto = new CreateDoctorAddressDto(addressId);

        _addressRepository.ExistsAsync(addressId, Arg.Any<CancellationToken>()).Returns(false);

        var act = async () => await _sut.CreateAsync(doctorId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{addressId}*");
    }

    [Fact(DisplayName = "CreateAsync: created record cannot be retrieved throws InvalidOperationException")]
    public async Task CreateAsync_CannotRetrieveCreated_ThrowsInvalidOperationException()
    {
        var doctorId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var dto = new CreateDoctorAddressDto(addressId);

        _addressRepository.ExistsAsync(addressId, Arg.Any<CancellationToken>()).Returns(true);
        _repository.GetByDoctorWithAddressAsync(addressId, doctorId, Arg.Any<CancellationToken>()).Returns((DoctorAddress?)null);

        var act = async () => await _sut.CreateAsync(doctorId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact(DisplayName = "CreateAsync: valid attaches address and returns mapped DTO")]
    public async Task CreateAsync_Valid_ReturnsMappedDto()
    {
        var doctorId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var dto = new CreateDoctorAddressDto(addressId);
        var createdDoctorAddress = new DoctorAddress { DoctorId = doctorId, AddressId = addressId, Address = MakeAddress(addressId) };
        var expected = new DoctorAddressDto(doctorId, addressId, "12345-000", null, AddressType.Clinic, "Sao Paulo", null, "Brazil", null, null, "SP");

        _addressRepository.ExistsAsync(addressId, Arg.Any<CancellationToken>()).Returns(true);
        _repository.GetByDoctorWithAddressAsync(addressId, doctorId, Arg.Any<CancellationToken>()).Returns(createdDoctorAddress);
        _mapper.Map<DoctorAddressDto>(createdDoctorAddress).Returns(expected);

        var result = await _sut.CreateAsync(doctorId, dto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(Arg.Is<DoctorAddress>(da => da.DoctorId == doctorId && da.AddressId == addressId), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "DeleteByIdsAsync: empty list throws ArgumentException")]
    public async Task DeleteByIdsAsync_EmptyList_ThrowsArgumentException()
    {
        var act = async () => await _sut.DeleteByIdsAsync([], Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*empty*");
    }

    [Fact(DisplayName = "DeleteByIdsAsync: no valid records throws UnauthorizedAccessException")]
    public async Task DeleteByIdsAsync_NoValidRecords_ThrowsUnauthorizedAccessException()
    {
        var doctorId = Guid.NewGuid();
        var ids = new List<Guid> { Guid.NewGuid() };
        _repository.GetManyByDoctorAndAddressIdsAsync(ids, doctorId, Arg.Any<CancellationToken>()).Returns(new List<DoctorAddress>());

        var act = async () => await _sut.DeleteByIdsAsync(ids, doctorId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "DeleteByIdsAsync: valid ids detaches records")]
    public async Task DeleteByIdsAsync_Valid_DetachesRecords()
    {
        var doctorId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var ids = new List<Guid> { addressId };
        var doctorAddresses = new List<DoctorAddress> { new() { DoctorId = doctorId, AddressId = addressId } };

        _repository.GetManyByDoctorAndAddressIdsAsync(ids, doctorId, Arg.Any<CancellationToken>()).Returns(doctorAddresses);

        await _sut.DeleteByIdsAsync(ids, doctorId, CancellationToken.None);

        await _repository.Received(1).DeleteManyAsync(Arg.Is<List<Guid>>(list => list.Contains(addressId)), doctorId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "DeleteAllByDoctorAsync: delegates to repository")]
    public async Task DeleteAllByDoctorAsync_DelegatesToRepository()
    {
        var doctorId = Guid.NewGuid();

        await _sut.DeleteAllByDoctorAsync(doctorId, CancellationToken.None);

        await _repository.Received(1).DeleteAllByDoctorAsync(doctorId, Arg.Any<CancellationToken>());
    }
}
