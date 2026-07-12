using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories
{
    public class DoctorSpecialtyRepository : IDoctorSpecialtyRepository
    {
        private readonly DatabaseContext _context;
        private readonly IUserSessionContext _userSessionContext;

        public DoctorSpecialtyRepository(DatabaseContext context, IUserSessionContext userSessionContext)
        {
            _context = context;
            _userSessionContext = userSessionContext;
        }

        public async Task<DoctorSpecialty?> GetByIdAsync(Guid id, Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorSpecialties
                .FirstOrDefaultAsync(x => x.Id == id && x.DoctorId == doctorId && x.Doctor.PersonId == personId, cancellationToken);
        }

        public async Task<List<DoctorSpecialty>> GetAllByDoctorAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken = default)
        {
            (page, pageSize) = PaginationNormalizer.Normalize(page, pageSize);

            var personId = _userSessionContext.PersonId;

            return await _context.DoctorSpecialties
                .Where(x => x.DoctorId == doctorId && x.Doctor.PersonId == personId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<DoctorSpecialty>> GetAllByDoctorUnpagedAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorSpecialties
                .Where(x => x.DoctorId == doctorId && x.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorSpecialties
                .Where(x => x.DoctorId == doctorId && x.Doctor.PersonId == personId)
                .CountAsync(cancellationToken);
        }

        public async Task<List<DoctorSpecialty>> GetManyByIdsAsync(IEnumerable<Guid> ids, Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorSpecialties
                .Where(x => ids.Contains(x.Id) && x.DoctorId == doctorId && x.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);
        }

        public async Task<DoctorSpecialty> AddAsync(DoctorSpecialty entity, CancellationToken cancellationToken = default)
        {
            await _context.DoctorSpecialties.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task UpdateAsync(DoctorSpecialty entity, CancellationToken cancellationToken = default)
        {
            _context.DoctorSpecialties.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            var entities = await _context.DoctorSpecialties
                .Where(x => x.DoctorId == doctorId && x.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);

            _context.DoctorSpecialties.RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteManyAsync(IEnumerable<Guid> ids, Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            var entities = await _context.DoctorSpecialties
                .Where(x => ids.Contains(x.Id) && x.DoctorId == doctorId && x.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);

            _context.DoctorSpecialties.RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}