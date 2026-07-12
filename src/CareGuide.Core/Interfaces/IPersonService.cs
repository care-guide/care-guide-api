using CareGuide.Models.DTOs.Person;

namespace CareGuide.Core.Interfaces
{
    public interface IPersonService
    {
        Task<List<PersonDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<PersonDto> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<PersonDto> CreateAsync(CreatePersonDto createPerson, CancellationToken cancellationToken);
        Task<PersonDto> UpdateAsync(Guid id, PersonDto updatePerson, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<MyProfileDto> GetMyProfileAsync(Guid personId, CancellationToken cancellationToken);
        Task<MyProfileDto> UpdateMyProfileAsync(Guid personId, UpdatePersonProfileDto updateProfile, CancellationToken cancellationToken);
    }
}
