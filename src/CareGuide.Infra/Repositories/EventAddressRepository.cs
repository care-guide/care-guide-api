using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Entities;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories
{
    public class EventAddressRepository : IEventAddressRepository
    {
        private readonly DatabaseContext _context;
        private readonly IUserSessionContext _userSessionContext;

        public EventAddressRepository(DatabaseContext context, IUserSessionContext userSessionContext)
        {
            _context = context;
            _userSessionContext = userSessionContext;
        }

        public async Task<EventAddress> AddAsync(EventAddress entity, CancellationToken cancellationToken = default)
        {
            await _context.EventAddresses.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<List<EventAddress>> GetAllByEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.EventAddresses
                .Include(ea => ea.Address)
                .Where(ea => ea.EventId == eventId && ea.Event.PersonId == personId)
                .ToListAsync(cancellationToken);
        }

        public async Task<EventAddress?> GetByEventAndAddressAsync(Guid eventId, Guid addressId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.EventAddresses
                .Include(ea => ea.Address)
                .FirstOrDefaultAsync(ea => ea.EventId == eventId && ea.AddressId == addressId && ea.Event.PersonId == personId, cancellationToken);
        }

        public async Task DeleteAsync(EventAddress entity, CancellationToken cancellationToken = default)
        {
            _context.EventAddresses.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
