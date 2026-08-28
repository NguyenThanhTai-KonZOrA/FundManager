using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
{
    public class DocumentTemplateRepository : GenericRepository<DocumentTemplate>, IDocumentTemplateRepository
    {
        public DocumentTemplateRepository(DigitalDocumentPlatformDbContext context) : base(context) { }

        public async Task<List<DocumentTemplate>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(d => d.IsActive && !d.IsDelete)
                .Include(d => d.Outlet)
                .Include(d => d.Translations)
                .Include(d => d.VersionHistories)
                .AsSplitQuery()
                .OrderBy(d => d.DocumentType)
                .ThenBy(d => d.Title)
                .ToListAsync();
        }

        public async Task<DocumentTemplate?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(d => d.Outlet)
                .Include(d => d.Translations)
                .Include(d => d.VersionHistories)
                .AsSplitQuery()
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDelete);
        }

        public async Task<List<DocumentTemplate>> GetByTypeAsync(DocumentType documentType)
        {
            return await _dbSet
                .Where(d => d.DocumentType == documentType && d.IsActive && !d.IsDelete)
                .OrderByDescending(d => d.Version)
                .ToListAsync();
        }

        public async Task<List<DocumentTemplate>> GetByOutletAsync(int outletId)
        {
            return await _dbSet
                .Where(d => (d.OutletId == null || d.OutletId == outletId) && d.IsActive && !d.IsDelete)
                .Include(d => d.Outlet)
                .OrderBy(d => d.DocumentType)
                .ThenBy(d => d.Title)
                .ToListAsync();
        }
    }
}
