using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Person;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PersonService(IPersonRepository personRepository, IUserRepository userRepository, IMapper mapper)
        {
            _personRepository = personRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<PersonDto>> GetAllAsync(int page = PaginationConstants.DefaultPage, int pageSize = PaginationConstants.DefaultPageSize, CancellationToken cancellationToken = default)
        {
            var list = await _personRepository.GetAllAsync(page, pageSize, cancellationToken);
            return _mapper.Map<List<PersonDto>>(list);
        }

        public async Task<PersonDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var person = await _personRepository.GetAsync(id, cancellationToken);

            if (person == null)
                throw new KeyNotFoundException();

            return _mapper.Map<PersonDto>(person);
        }

        public async Task<PersonDto> CreateAsync(CreatePersonDto createPerson, CancellationToken cancellationToken = default)
        {
            var person = _mapper.Map<Person>(createPerson);
            await _personRepository.AddAsync(person, cancellationToken);
            return _mapper.Map<PersonDto>(person);
        }

        public async Task<PersonDto> UpdateAsync(Guid id, PersonDto updatePerson, CancellationToken cancellationToken = default)
        {
            var existingPerson = await _personRepository.GetAsync(id, cancellationToken);

            if (existingPerson == null)
                throw new KeyNotFoundException();

            existingPerson.Name = updatePerson.Name;
            existingPerson.Gender = updatePerson.Gender;
            existingPerson.Birthday = updatePerson.Birthday;
            existingPerson.Picture = updatePerson.Picture;

            var person = await _personRepository.UpdateAsync(existingPerson, cancellationToken);
            return _mapper.Map<PersonDto>(person);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existingPerson = await _personRepository.GetAsync(id, cancellationToken);

            if (existingPerson == null)
                throw new KeyNotFoundException();

            await _personRepository.DeleteAsync(existingPerson.Id, cancellationToken);
        }

        public async Task<MyProfileDto> GetMyProfileAsync(Guid personId, CancellationToken cancellationToken = default)
        {
            if (personId == Guid.Empty)
                throw new UnauthorizedAccessException("No authenticated person for this session.");

            var person = await _personRepository.GetAsync(personId, cancellationToken);

            if (person == null)
                throw new KeyNotFoundException();

            var user = await _userRepository.GetByPersonIdAsync(personId, cancellationToken)
                ?? throw new KeyNotFoundException();

            return MapToMyProfileDto(person, user.Email);
        }

        public async Task<MyProfileDto> UpdateMyProfileAsync(Guid personId, UpdatePersonProfileDto updateProfile, CancellationToken cancellationToken = default)
        {
            if (personId == Guid.Empty)
                throw new UnauthorizedAccessException("No authenticated person for this session.");

            var existingPerson = await _personRepository.GetAsync(personId, cancellationToken);

            if (existingPerson == null)
                throw new KeyNotFoundException();

            existingPerson.Name = updateProfile.Name;
            existingPerson.Gender = updateProfile.Gender;
            existingPerson.Birthday = updateProfile.Birthday;
            existingPerson.Picture = updateProfile.Picture;

            var updatedPerson = await _personRepository.UpdateAsync(existingPerson, cancellationToken);

            var user = await _userRepository.GetByPersonIdAsync(personId, cancellationToken)
                ?? throw new KeyNotFoundException();

            return MapToMyProfileDto(updatedPerson, user.Email);
        }

        private MyProfileDto MapToMyProfileDto(Person person, string email)
        {
            var personDto = _mapper.Map<PersonDto>(person);

            return new MyProfileDto(
                personDto.Id,
                personDto.Name,
                personDto.Gender,
                personDto.Birthday,
                personDto.Picture,
                email
            );
        }
    }
}
