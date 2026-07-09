using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Infra.Repositories
{
    public class FileAssetRepository : BaseRepository<FileAsset>, IFileAssetRepository
    {
        private readonly DatabaseContext _context;

        public FileAssetRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }
    }
}
