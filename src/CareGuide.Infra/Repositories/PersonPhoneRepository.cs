using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories
{
    public class PersonPhoneRepository : IPersonPhoneRepository
    {
        private readonly DatabaseContext _context;
        private readonly IUserSessionContext _userSessionContext;

        public PersonPhoneRepository(DatabaseContext context, IUserSessionContext userSessionContext)
        {
            _context = context;
            _userSessionContext = userSessionContext;
        }

        public async Task AddAsync(PersonPhone entity, CancellationToken cancellationToken = default)
        {
            await _context.PersonPhones.AddAsync(entity, cancellationToken);
        }

        public async Task<List<PersonPhone>> GetAllByPersonWithPhonesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            (page, pageSize) = PaginationNormalizer.Normalize(page, pageSize);

            var personId = _userSessionContext.PersonId;

            return await _context.PersonPhones
                .Include(pp => pp.Phone)
                .Where(pp => pp.PersonId == personId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByPersonAsync(CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.PersonPhones
                .Where(pp => pp.PersonId == personId)
                .CountAsync(cancellationToken);
        }

        public async Task<List<PersonPhone>> GetAllByPersonUnpagedAsync(CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.PersonPhones
                .Where(pp => pp.PersonId == personId)
                .ToListAsync(cancellationToken);
        }

        public async Task<PersonPhone?> GetByPersonWithPhoneAsync(Guid phoneId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.PersonPhones
                .Include(pp => pp.Phone)
                .FirstOrDefaultAsync(pp => pp.PersonId == personId && pp.PhoneId == phoneId, cancellationToken);
        }

        public async Task<List<PersonPhone>> GetManyByPersonAndPhoneIdsAsync(IEnumerable<Guid> phoneIds, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.PersonPhones
                .Include(pp => pp.Phone)
                .Where(pp => pp.PersonId == personId && phoneIds.Contains(pp.PhoneId))
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteAllByPersonAsync(CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            var entities = await _context.PersonPhones
                .Where(pp => pp.PersonId == personId)
                .ToListAsync(cancellationToken);

            _context.PersonPhones.RemoveRange(entities);
        }

        public async Task DeleteManyAsync(IEnumerable<Guid> phoneIds, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            var entities = await _context.PersonPhones
                .Where(pp => pp.PersonId == personId && phoneIds.Contains(pp.PhoneId))
                .ToListAsync(cancellationToken);

            _context.PersonPhones.RemoveRange(entities);
        }
    }
}