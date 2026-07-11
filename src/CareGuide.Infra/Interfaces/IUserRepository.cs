using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Models.Entities;


namespace CareGuide.Infra.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken);
    }
}
