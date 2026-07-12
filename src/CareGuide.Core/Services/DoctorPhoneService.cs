using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.TransactionManagement;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.DoctorPhone;
using CareGuide.Models.DTOs.Phone;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class DoctorPhoneService : IDoctorPhoneService
    {
        private readonly IDoctorPhoneRepository _doctorPhoneRepository;
        private readonly IPhoneService _phoneService;
        private readonly IEfTransactionUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DoctorPhoneService(IDoctorPhoneRepository doctorPhoneRepository, IPhoneService phoneService, IEfTransactionUnitOfWork unitOfWork, IMapper mapper)
        {
            _doctorPhoneRepository = doctorPhoneRepository;
            _phoneService = phoneService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<DoctorPhoneDto>> GetAllByDoctorAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken)
        {
            var (normalizedPage, normalizedPageSize) = PaginationNormalizer.Normalize(page, pageSize);

            var doctorPhones = await _doctorPhoneRepository.GetAllByDoctorWithPhonesAsync(normalizedPage, normalizedPageSize, doctorId, cancellationToken);
            var total = await _doctorPhoneRepository.CountByDoctorAsync(doctorId, cancellationToken);

            if (total == 0)
                throw new KeyNotFoundException($"No phone records found for the doctor with ID {doctorId}.");

            return new PagedResult<DoctorPhoneDto>(_mapper.Map<List<DoctorPhoneDto>>(doctorPhones), normalizedPage, normalizedPageSize, total);
        }

        public async Task<DoctorPhoneDto> GetAsync(Guid phoneId, Guid doctorId, CancellationToken cancellationToken)
        {
            var doctorPhone = await _doctorPhoneRepository.GetByDoctorWithPhoneAsync(phoneId, doctorId, cancellationToken);

            if (doctorPhone == null)
                throw new UnauthorizedAccessException("You are not authorized to access this phone record or it does not exist.");

            return _mapper.Map<DoctorPhoneDto>(doctorPhone);
        }

        public async Task<DoctorPhoneDto> CreateAsync(Guid doctorId, CreatePhoneDto phoneDto, CancellationToken cancellationToken)
        {
            if (phoneDto == null)
                throw new ArgumentNullException(nameof(phoneDto));

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var createdPhone = await _phoneService.CreateAsync(phoneDto, cancellationToken);

                var doctorPhone = new DoctorPhone
                {
                    DoctorId = doctorId,
                    PhoneId = createdPhone.Id
                };

                await _doctorPhoneRepository.AddAsync(doctorPhone, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                var createdDoctorPhone = await _doctorPhoneRepository.GetByDoctorWithPhoneAsync(createdPhone.Id, doctorId, cancellationToken);

                if (createdDoctorPhone == null)
                    throw new InvalidOperationException("The created phone record could not be retrieved.");

                return _mapper.Map<DoctorPhoneDto>(createdDoctorPhone);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task<DoctorPhoneDto> UpdateAsync(Guid id, Guid doctorId, UpdatePhoneDto phoneDto, CancellationToken cancellationToken)
        {
            if (phoneDto == null)
                throw new ArgumentNullException(nameof(phoneDto));

            var doctorPhone = await _doctorPhoneRepository.GetByDoctorWithPhoneAsync(id, doctorId, cancellationToken);

            if (doctorPhone == null)
                throw new UnauthorizedAccessException("You are not authorized to update this phone record or it does not exist.");

            await _phoneService.UpdateAsync(id, phoneDto, cancellationToken);

            var updatedDoctorPhone = await _doctorPhoneRepository.GetByDoctorWithPhoneAsync(id, doctorId, cancellationToken);

            if (updatedDoctorPhone == null)
                throw new InvalidOperationException("The updated phone record could not be retrieved.");

            return _mapper.Map<DoctorPhoneDto>(updatedDoctorPhone);
        }

        public async Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken)
        {
            var doctorPhones = await _doctorPhoneRepository.GetAllByDoctorUnpagedAsync(doctorId, cancellationToken);

            if (doctorPhones.Count == 0)
                return;

            var phoneIds = doctorPhones.Select(dp => dp.PhoneId).ToList();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                await _doctorPhoneRepository.DeleteAllByDoctorAsync(doctorId, cancellationToken);
                await _phoneService.DeleteByIdsAsync(phoneIds, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task DeleteByIdsAsync(List<Guid> phoneIds, Guid doctorId, CancellationToken cancellationToken)
        {
            if (phoneIds == null || phoneIds.Count == 0)
                throw new ArgumentException("The list of IDs cannot be empty.", nameof(phoneIds));

            var doctorPhones = await _doctorPhoneRepository.GetManyByDoctorAndPhoneIdsAsync(phoneIds, doctorId, cancellationToken);

            if (doctorPhones.Count == 0)
                throw new UnauthorizedAccessException("No valid phone records found for this doctor.");

            var validPhoneIds = doctorPhones.Select(dp => dp.PhoneId).ToList();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                await _doctorPhoneRepository.DeleteManyAsync(validPhoneIds, doctorId, cancellationToken);
                await _phoneService.DeleteByIdsAsync(validPhoneIds, cancellationToken);
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