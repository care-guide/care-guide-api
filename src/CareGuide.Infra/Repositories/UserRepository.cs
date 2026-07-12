using CareGuide.Models.Entities;

namespace CareGuide.Infra.Repositories
{
    using CareGuide.Infra.Contexts;
    using CareGuide.Infra.Interfaces;
    using CareGuide.Infra.Interfaces.Shared;
    using CareGuide.Infra.Repositories.Shared;
    using Microsoft.EntityFrameworkCore;

    public class UserRepository : BasePersonOwnedRepository<User>, IUserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context, IQueryScope queryScope) : base(context, queryScope)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User?> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.PersonId == personId, cancellationToken);
        }
    }
}
