using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.TransactionManagement;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.PersonPhone;
using CareGuide.Models.DTOs.Phone;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class PersonPhoneService : IPersonPhoneService
    {
        private readonly IPersonPhoneRepository _personPhoneRepository;
        private readonly IPhoneService _phoneService;
        private readonly IEfTransactionUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PersonPhoneService(IPersonPhoneRepository personPhoneRepository, IPhoneService phoneService, IEfTransactionUnitOfWork unitOfWork, IMapper mapper)
        {
            _personPhoneRepository = personPhoneRepository;
            _phoneService = phoneService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<PersonPhoneDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (normalizedPage, normalizedPageSize) = PaginationNormalizer.Normalize(page, pageSize);

            var personPhones = await _personPhoneRepository.GetAllByPersonWithPhonesAsync(normalizedPage, normalizedPageSize, cancellationToken);
            var total = await _personPhoneRepository.CountByPersonAsync(cancellationToken);

            if (total == 0)
                throw new KeyNotFoundException("No phone records found for the logged-in person.");

            return new PagedResult<PersonPhoneDto>(_mapper.Map<List<PersonPhoneDto>>(personPhones), normalizedPage, normalizedPageSize, total);
        }

        public async Task<PersonPhoneDto> GetAsync(Guid phoneId, CancellationToken cancellationToken)
        {
            var personPhone = await _personPhoneRepository.GetByPersonWithPhoneAsync(phoneId, cancellationToken);

            if (personPhone == null)
                throw new UnauthorizedAccessException("You are not authorized to access this phone record or it does not exist.");

            return _mapper.Map<PersonPhoneDto>(personPhone);
        }

        public async Task<PersonPhoneDto> CreateAsync(CreatePhoneDto phoneDto, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var createdPhone = await _phoneService.CreateAsync(phoneDto, cancellationToken);

                var personPhone = new PersonPhone
                {
                    PhoneId = createdPhone.Id
                };

                await _personPhoneRepository.AddAsync(personPhone, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                var createdPersonPhone = await _personPhoneRepository.GetByPersonWithPhoneAsync(createdPhone.Id, cancellationToken);

                if (createdPersonPhone == null)
                    throw new InvalidOperationException("The created phone record could not be retrieved.");

                return _mapper.Map<PersonPhoneDto>(createdPersonPhone);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task<PersonPhoneDto> UpdateAsync(Guid id, UpdatePhoneDto phoneDto, CancellationToken cancellationToken)
        {
            if (phoneDto == null)
                throw new ArgumentNullException(nameof(phoneDto));

            var personPhone = await _personPhoneRepository.GetByPersonWithPhoneAsync(id, cancellationToken);

            if (personPhone == null)
                throw new UnauthorizedAccessException("You are not authorized to update this phone record or it does not exist.");

            await _phoneService.UpdateAsync(id, phoneDto, cancellationToken);

            var updatedPersonPhone = await _personPhoneRepository.GetByPersonWithPhoneAsync(id, cancellationToken);

            if (updatedPersonPhone == null)
                throw new InvalidOperationException("The updated phone record could not be retrieved.");

            return _mapper.Map<PersonPhoneDto>(updatedPersonPhone);
        }

        public async Task DeleteAllByPersonAsync(CancellationToken cancellationToken)
        {
            var personPhones = await _personPhoneRepository.GetAllByPersonUnpagedAsync(cancellationToken);

            if (personPhones.Count == 0)
                return;

            var phoneIds = personPhones.Select(pp => pp.PhoneId).ToList();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                await _personPhoneRepository.DeleteAllByPersonAsync(cancellationToken);
                await _phoneService.DeleteByIdsAsync(phoneIds, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            if (ids == null || ids.Count == 0)
                throw new ArgumentException("The list of IDs cannot be empty.", nameof(ids));

            var personPhones = await _personPhoneRepository.GetManyByPersonAndPhoneIdsAsync(ids, cancellationToken);

            if (personPhones.Count == 0)
                throw new UnauthorizedAccessException("No valid phone records found for this person.");

            var phoneIds = personPhones.Select(pp => pp.PhoneId).ToList();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                await _personPhoneRepository.DeleteManyAsync(phoneIds, cancellationToken);
                await _phoneService.DeleteByIdsAsync(phoneIds, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}