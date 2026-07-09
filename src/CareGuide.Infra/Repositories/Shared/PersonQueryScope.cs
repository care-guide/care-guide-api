using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Models.Entities.Shared;
using CareGuide.Security.Interfaces;

namespace CareGuide.Infra.Repositories.Shared
{
    public class PersonQueryScope : IQueryScope
    {
        private readonly IUserSessionContext _userSessionContext;

        public PersonQueryScope(IUserSessionContext userSessionContext)
        {
            _userSessionContext = userSessionContext;
        }

        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query) where TEntity : IPersonOwnedEntity
        {
            if (_userSessionContext.IsAdmin)
                return query;

            var personId = _userSessionContext.PersonId;
            return query.Where(e => e.PersonId == personId);
        }
    }
}
