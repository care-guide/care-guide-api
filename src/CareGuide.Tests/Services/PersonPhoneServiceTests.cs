using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.TransactionManagement;
using CareGuide.Models.DTOs.Phone;
using CareGuide.Models.DTOs.PersonPhone;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class PersonPhoneServiceTests
{
    private readonly IPersonPhoneRepository _repository;
    private readonly IPhoneService _phoneService;
    private readonly IEfTransactionUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly PersonPhoneService _sut;

    public PersonPhoneServiceTests()
    {
        _repository = Substitute.For<IPersonPhoneRepository>();
        _phoneService = Substitute.For<IPhoneService>();
        _unitOfWork = Substitute.For<IEfTransactionUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _sut = new PersonPhoneService(_repository, _phoneService, _unitOfWork, _mapper);
    }

    // ── GetAllByPersonAsync ───────────────────────────────────────────────────

    [Fact(DisplayName = "GetAllByPersonAsync: empty result throws KeyNotFoundException")]
    public async Task GetAllByPersonAsync_EmptyList_ThrowsKeyNotFoundException()
    {
        _repository.GetAllByPersonWithPhonesAsync(1, 10, Arg.Any<CancellationToken>()).Returns(new List<PersonPhone>());

        var act = async () => await _sut.GetAllByPersonAsync(1, 10, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "GetAllByPersonAsync: found returns mapped list")]
    public async Task GetAllByPersonAsync_Found_ReturnsMappedList()
    {
        var phoneId = Guid.NewGuid();
        var personPhones = new List<PersonPhone>
        {
            new() { PhoneId = phoneId, Phone = new Phone { Id = phoneId, Number = "912345678", AreaCode = "11", Type = PhoneType.CEL } }
        };
        var dtos = new List<PersonPhoneDto> { new(Guid.NewGuid(), phoneId, "912345678", "11", PhoneType.CEL) };

        _repository.GetAllByPersonWithPhonesAsync(1, 10, Arg.Any<CancellationToken>()).Returns(personPhones);
        _mapper.Map<List<PersonPhoneDto>>(personPhones).Returns(dtos);

        var result = await _sut.GetAllByPersonAsync(1, 10, CancellationToken.None);

        result.Should().HaveCount(1);
    }

    // ── GetAsync ──────────────────────────────────────────────────────────────

    [Fact(DisplayName = "GetAsync: not found throws UnauthorizedAccessException")]
    public async Task GetAsync_NotFound_ThrowsUnauthorized()
    {
        var phoneId = Guid.NewGuid();
        _repository.GetByPersonWithPhoneAsync(phoneId, Arg.Any<CancellationToken>()).Returns((PersonPhone?)null);

        var act = async () => await _sut.GetAsync(phoneId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var phoneId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var personPhone = new PersonPhone { PhoneId = phoneId, Phone = new Phone { Id = phoneId, Number = "912345678", AreaCode = "11", Type = PhoneType.CEL } };
        var expected = new PersonPhoneDto(personId, phoneId, "912345678", "11", PhoneType.CEL);

        _repository.GetByPersonWithPhoneAsync(phoneId, Arg.Any<CancellationToken>()).Returns(personPhone);
        _mapper.Map<PersonPhoneDto>(personPhone).Returns(expected);

        var result = await _sut.GetAsync(phoneId, CancellationToken.None);

        result.Should().Be(expected);
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact(DisplayName = "CreateAsync: success commits transaction and returns DTO")]
    public async Task CreateAsync_Success_CommitsAndReturnsDto()
    {
        var phoneId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var createDto = new CreatePhoneDto("912345678", "11", PhoneType.CEL);
        var createdPhoneDto = new PhoneDto(phoneId, "912345678", "11", PhoneType.CEL, DateTime.UtcNow, DateTime.UtcNow);
        var createdPersonPhone = new PersonPhone { PhoneId = phoneId, Phone = new Phone { Id = phoneId, Number = "912345678", AreaCode = "11", Type = PhoneType.CEL } };
        var expected = new PersonPhoneDto(personId, phoneId, "912345678", "11", PhoneType.CEL);

        _phoneService.CreateAsync(createDto, Arg.Any<CancellationToken>()).Returns(createdPhoneDto);
        _repository.GetByPersonWithPhoneAsync(phoneId, Arg.Any<CancellationToken>()).Returns(createdPersonPhone);
        _mapper.Map<PersonPhoneDto>(createdPersonPhone).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "CreateAsync: phone service throws rolls back transaction")]
    public async Task CreateAsync_PhoneServiceThrows_RollsBack()
    {
        var createDto = new CreatePhoneDto("912345678", "11", PhoneType.CEL);
        _phoneService.CreateAsync(createDto, Arg.Any<CancellationToken>()).Returns(Task.FromException<PhoneDto>(new Exception("phone error")));

        var act = async () => await _sut.CreateAsync(createDto, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();
        await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
    }

    // ── UpdateAsync ───────────────────────────────────────────────────────────

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: not found throws UnauthorizedAccessException")]
    public async Task UpdateAsync_NotFound_ThrowsUnauthorized()
    {
        var id = Guid.NewGuid();
        _repository.GetByPersonWithPhoneAsync(id, Arg.Any<CancellationToken>()).Returns((PersonPhone?)null);

        var act = async () => await _sut.UpdateAsync(id, new UpdatePhoneDto(id, "912345678", "11", PhoneType.CEL), CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "UpdateAsync: valid data delegates to phone service and returns updated DTO")]
    public async Task UpdateAsync_Valid_ReturnsUpdatedDto()
    {
        var id = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var existing = new PersonPhone { PhoneId = id, Phone = new Phone { Id = id, Number = "11111111", AreaCode = "11", Type = PhoneType.CEL } };
        var updateDto = new UpdatePhoneDto(id, "987654321", "21", PhoneType.R);
        var updated = new PersonPhone { PhoneId = id, Phone = new Phone { Id = id, Number = "987654321", AreaCode = "21", Type = PhoneType.R } };
        var expected = new PersonPhoneDto(personId, id, "987654321", "21", PhoneType.R);

        _repository.GetByPersonWithPhoneAsync(id, Arg.Any<CancellationToken>()).Returns(existing, updated);
        _mapper.Map<PersonPhoneDto>(updated).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        await _phoneService.Received(1).UpdateAsync(id, updateDto, Arg.Any<CancellationToken>());
        result.Should().Be(expected);
    }

    // ── DeleteByIdsAsync ──────────────────────────────────────────────────────

    [Fact(DisplayName = "DeleteByIdsAsync: empty list throws ArgumentException")]
    public async Task DeleteByIdsAsync_EmptyList_ThrowsArgumentException()
    {
        var act = async () => await _sut.DeleteByIdsAsync(new List<Guid>(), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*empty*");
    }

    [Fact(DisplayName = "DeleteByIdsAsync: no valid records throws UnauthorizedAccessException")]
    public async Task DeleteByIdsAsync_NoValidRecords_ThrowsUnauthorized()
    {
        var ids = new List<Guid> { Guid.NewGuid() };
        _repository.GetManyByPersonAndPhoneIdsAsync(ids, Arg.Any<CancellationToken>()).Returns(new List<PersonPhone>());

        var act = async () => await _sut.DeleteByIdsAsync(ids, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "DeleteByIdsAsync: valid ids commits transaction and deletes")]
    public async Task DeleteByIdsAsync_Valid_CommitsAndDeletes()
    {
        var phoneId = Guid.NewGuid();
        var ids = new List<Guid> { phoneId };
        var personPhones = new List<PersonPhone> { new() { PhoneId = phoneId } };

        _repository.GetManyByPersonAndPhoneIdsAsync(ids, Arg.Any<CancellationToken>()).Returns(personPhones);

        await _sut.DeleteByIdsAsync(ids, CancellationToken.None);

        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
        await _phoneService.Received(1).DeleteByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }

    // ── DeleteAllByPersonAsync ────────────────────────────────────────────────

    [Fact(DisplayName = "DeleteAllByPersonAsync: empty phone list skips delete")]
    public async Task DeleteAllByPersonAsync_EmptyList_SkipsOperation()
    {
        _repository.GetAllByPersonWithPhonesAsync(1, int.MaxValue, Arg.Any<CancellationToken>()).Returns(new List<PersonPhone>());

        await _sut.DeleteAllByPersonAsync(CancellationToken.None);

        await _unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "DeleteAllByPersonAsync: with phones commits and deletes all")]
    public async Task DeleteAllByPersonAsync_WithPhones_CommitsAndDeletes()
    {
        var phoneId = Guid.NewGuid();
        var personPhones = new List<PersonPhone> { new() { PhoneId = phoneId } };
        _repository.GetAllByPersonWithPhonesAsync(1, int.MaxValue, Arg.Any<CancellationToken>()).Returns(personPhones);

        await _sut.DeleteAllByPersonAsync(CancellationToken.None);

        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
        await _phoneService.Received(1).DeleteByIdsAsync(Arg.Any<List<Guid>>(), Arg.Any<CancellationToken>());
    }
}
