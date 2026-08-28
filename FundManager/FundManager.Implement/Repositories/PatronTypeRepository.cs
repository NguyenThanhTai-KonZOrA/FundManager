using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
{
    public class PatronTypeRepository : GenericRepository<PatronType>, IPatronTypeRepository
    {
        public PatronTypeRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            var query = _context.PatronTypes.Where(p => p.Name == name);
            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);
            return await query.AnyAsync();
        }
    }
}
