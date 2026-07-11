using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Entities;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories
{
    public class EventFileRepository : IEventFileRepository
    {
        private readonly DatabaseContext _context;
        private readonly IUserSessionContext _userSessionContext;

        public EventFileRepository(DatabaseContext context, IUserSessionContext userSessionContext)
        {
            _context = context;
            _userSessionContext = userSessionContext;
        }

        public async Task<EventFile> AddAsync(EventFile entity, CancellationToken cancellationToken = default)
        {
            await _context.EventFiles.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<List<EventFile>> GetAllByEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.EventFiles
                .Include(ef => ef.File)
                .Where(ef => ef.EventId == eventId && ef.Event.PersonId == personId)
                .ToListAsync(cancellationToken);
        }

        public async Task<EventFile?> GetByEventAndFileAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.EventFiles
                .Include(ef => ef.File)
                .FirstOrDefaultAsync(ef =>
                    ef.EventId == eventId &&
                    ef.FileId == fileId &&
                    ef.Event.PersonId == personId &&
                    ef.File.PersonId == personId,
                    cancellationToken);
        }

        public async Task DeleteAsync(EventFile entity, CancellationToken cancellationToken = default)
        {
            _context.EventFiles.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
