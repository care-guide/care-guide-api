using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.TransactionManagement;
using CareGuide.Models.DTOs.DoctorPhone;
using CareGuide.Models.DTOs.Phone;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class DoctorPhoneServiceTests
{
    private readonly IDoctorPhoneRepository _repository;
    private readonly IPhoneService _phoneService;
    private readonly IEfTransactionUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly DoctorPhoneService _sut;

    public DoctorPhoneServiceTests()
    {
        _repository = Substitute.For<IDoctorPhoneRepository>();
        _phoneService = Substitute.For<IPhoneService>();
        _unitOfWork = Substitute.For<IEfTransactionUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _sut = new DoctorPhoneService(_repository, _phoneService, _unitOfWork, _mapper);
    }

    // ── GetAllByDoctorAsync ───────────────────────────────────────────────────

    [Fact(DisplayName = "GetAllByDoctorAsync: empty result throws KeyNotFoundException")]
    public async Task GetAllByDoctorAsync_EmptyList_ThrowsKeyNotFoundException()
    {
        var doctorId = Guid.NewGuid();
        _repository.GetAllByDoctorWithPhonesAsync(1, 10, doctorId, Arg.Any<CancellationToken>()).Returns(new List<DoctorPhone>());

        var act = async () => await _sut.GetAllByDoctorAsync(1, 10, doctorId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{doctorId}*");
    }

    [Fact(DisplayName = "GetAllByDoctorAsync: found returns mapped list")]
    public async Task GetAllByDoctorAsync_Found_ReturnsMappedList()
    {
        var doctorId = Guid.NewGuid();
        var phoneId = Guid.NewGuid();
        var doctorPhones = new List<DoctorPhone>
        {
            new() { DoctorId = doctorId, PhoneId = phoneId, Phone = new Phone { Id = phoneId, Number = "912345678", AreaCode = "11", Type = PhoneType.CEL } }
        };
        var dtos = new List<DoctorPhoneDto> { new(doctorId, phoneId, "912345678", "11", PhoneType.CEL) };

        _repository.GetAllByDoctorWithPhonesAsync(1, 10, doctorId, Arg.Any<CancellationToken>()).Returns(doctorPhones);
        _mapper.Map<List<DoctorPhoneDto>>(doctorPhones).Returns(dtos);

        var result = await _sut.GetAllByDoctorAsync(1, 10, doctorId, CancellationToken.None);

        result.Should().HaveCount(1);
    }

    // ── GetAsync ──────────────────────────────────────────────────────────────

    [Fact(DisplayName = "GetAsync: not found throws UnauthorizedAccessException")]
    public async Task GetAsync_NotFound_ThrowsUnauthorized()
    {
        var phoneId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        _repository.GetByDoctorWithPhoneAsync(phoneId, doctorId, Arg.Any<CancellationToken>()).Returns((DoctorPhone?)null);

        var act = async () => await _sut.GetAsync(phoneId, doctorId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var doctorId = Guid.NewGuid();
        var phoneId = Guid.NewGuid();
        var doctorPhone = new DoctorPhone { DoctorId = doctorId, PhoneId = phoneId, Phone = new Phone { Id = phoneId, Number = "912345678", AreaCode = "11", Type = PhoneType.CEL } };
        var expected = new DoctorPhoneDto(doctorId, phoneId, "912345678", "11", PhoneType.CEL);

        _repository.GetByDoctorWithPhoneAsync(phoneId, doctorId, Arg.Any<CancellationToken>()).Returns(doctorPhone);
        _mapper.Map<DoctorPhoneDto>(doctorPhone).Returns(expected);

        var result = await _sut.GetAsync(phoneId, doctorId, CancellationToken.None);

        result.Should().Be(expected);
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact(DisplayName = "CreateAsync: null dto throws ArgumentNullException")]
    public async Task CreateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.CreateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync: success commits transaction and returns DTO")]
    public async Task CreateAsync_Success_CommitsAndReturnsDto()
    {
        var doctorId = Guid.NewGuid();
        var phoneId = Guid.NewGuid();
        var createDto = new CreatePhoneDto("912345678", "11", PhoneType.CEL);
        var createdPhoneDto = new PhoneDto(phoneId, "912345678", "11", PhoneType.CEL, DateTime.UtcNow, DateTime.UtcNow);
        var createdDoctorPhone = new DoctorPhone { DoctorId = doctorId, PhoneId = phoneId, Phone = new Phone { Id = phoneId, Number = "912345678", AreaCode = "11", Type = PhoneType.CEL } };
        var expected = new DoctorPhoneDto(doctorId, phoneId, "912345678", "11", PhoneType.CEL);

        _phoneService.CreateAsync(createDto, Arg.Any<CancellationToken>()).Returns(createdPhoneDto);
        _repository.GetByDoctorWithPhoneAsync(phoneId, doctorId, Arg.Any<CancellationToken>()).Returns(createdDoctorPhone);
        _mapper.Map<DoctorPhoneDto>(createdDoctorPhone).Returns(expected);

        var result = await _sut.CreateAsync(doctorId, createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "CreateAsync: phone service throws rolls back transaction")]
    public async Task CreateAsync_PhoneServiceThrows_RollsBack()
    {
        var doctorId = Guid.NewGuid();
        var createDto = new CreatePhoneDto("912345678", "11", PhoneType.CEL);
        _phoneService.CreateAsync(createDto, Arg.Any<CancellationToken>()).Returns(Task.FromException<PhoneDto>(new Exception("phone error")));

        var act = async () => await _sut.CreateAsync(doctorId, createDto, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();
        await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
    }

    // ── UpdateAsync ───────────────────────────────────────────────────────────

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: not found throws UnauthorizedAccessException")]
    public async Task UpdateAsync_NotFound_ThrowsUnauthorized()
    {
        var id = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        _repository.GetByDoctorWithPhoneAsync(id, doctorId, Arg.Any<CancellationToken>()).Returns((DoctorPhone?)null);

        var act = async () => await _sut.UpdateAsync(id, doctorId, new UpdatePhoneDto(id, "912345678", "11", PhoneType.CEL), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "UpdateAsync: valid data delegates to phone service and returns updated DTO")]
    public async Task UpdateAsync_Valid_ReturnsUpdatedDto()
    {
        var id = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var existing = new DoctorPhone { DoctorId = doctorId, PhoneId = id, Phone = new Phone { Id = id, Number = "11111111", AreaCode = "11", Type = PhoneType.CEL } };
        var updateDto = new UpdatePhoneDto(id, "987654321", "21", PhoneType.R);
        var updated = new DoctorPhone { DoctorId = doctorId, PhoneId = id, Phone = new Phone { Id = id, Number = "987654321", AreaCode = "21", Type = PhoneType.R } };
        var expected = new DoctorPhoneDto(doctorId, id, "987654321", "21", PhoneType.R);

        _repository.GetByDoctorWithPhoneAsync(id, doctorId, Arg.Any<CancellationToken>()).Returns(existing, updated);
        _mapper.Map<DoctorPhoneDto>(updated).Returns(expected);

        var result = await _sut.UpdateAsync(id, doctorId, updateDto, CancellationToken.None);

        await _phoneService.Received(1).UpdateAsync(id, updateDto, Arg.Any<CancellationToken>());
        result.Should().Be(expected);
    }

    // ── DeleteByIdsAsync ──────────────────────────────────────────────────────

    [Fact(DisplayName = "DeleteByIdsAsync: empty list throws ArgumentException")]
    public async Task DeleteByIdsAsync_EmptyList_ThrowsArgumentException()
    {
        var act = async () => await _sut.DeleteByIdsAsync(new List<Guid>(), Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*empty*");
    }

    [Fact(DisplayName = "DeleteByIdsAsync: no valid records throws UnauthorizedAccessException")]
    public async Task DeleteByIdsAsync_NoValidRecords_ThrowsUnauthorized()
    {
        var doctorId = Guid.NewGuid();
        var ids = new List<Guid> { Guid.NewGuid() };
        _repository.GetManyByDoctorAndPhoneIdsAsync(ids, doctorId, Arg.Any<CancellationToken>()).Returns(new List<DoctorPhone>());

        var act = async () => await _sut.DeleteByIdsAsync(ids, doctorId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "DeleteByIdsAsync: valid ids commits transaction and deletes")]
    public async Task DeleteByIdsAsync_Valid_CommitsAndDeletes()
    {
        var doctorId = Guid.NewGuid();
        var phoneId = Guid.NewGuid();
        var ids = new List<Guid> { phoneId };
        var doctorPhones = new List<DoctorPhone> { new() { DoctorId = doctorId, PhoneId = phoneId } };

        _repository.GetManyByDoctorAndPhoneIdsAsync(ids, doctorId, Arg.Any<CancellationToken>()).Returns(doctorPhones);

        await _sut.DeleteByIdsAsync(ids, doctorId, CancellationToken.None);

        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
        await _phoneService.Received(1).DeleteByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    // ── DeleteAllByDoctorAsync ────────────────────────────────────────────────

    [Fact(DisplayName = "DeleteAllByDoctorAsync: empty phone list skips delete")]
    public async Task DeleteAllByDoctorAsync_EmptyList_SkipsOperation()
    {
        var doctorId = Guid.NewGuid();
        _repository.GetAllByDoctorWithPhonesAsync(1, int.MaxValue, doctorId, Arg.Any<CancellationToken>()).Returns(new List<DoctorPhone>());

        await _sut.DeleteAllByDoctorAsync(doctorId, CancellationToken.None);

        await _unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "DeleteAllByDoctorAsync: with phones commits and deletes all")]
    public async Task DeleteAllByDoctorAsync_WithPhones_CommitsAndDeletes()
    {
        var doctorId = Guid.NewGuid();
        var phoneId = Guid.NewGuid();
        var doctorPhones = new List<DoctorPhone> { new() { DoctorId = doctorId, PhoneId = phoneId } };
        _repository.GetAllByDoctorWithPhonesAsync(1, int.MaxValue, doctorId, Arg.Any<CancellationToken>()).Returns(doctorPhones);

        await _sut.DeleteAllByDoctorAsync(doctorId, CancellationToken.None);

        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
        await _phoneService.Received(1).DeleteByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }
}
