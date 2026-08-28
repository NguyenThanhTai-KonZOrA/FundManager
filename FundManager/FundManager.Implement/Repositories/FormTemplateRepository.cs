using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
{
    public class FormTemplateRepository : GenericRepository<FormTemplate>, IFormTemplateRepository
    {
        public FormTemplateRepository(DigitalDocumentPlatformDbContext context) : base(context) { }

        public async Task<List<FormTemplate>> GetAllActiveAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(t => t.Translations)
                .Include(t => t.VersionHistories)
                .AsSplitQuery()
                .Where(t => t.IsActive && !t.IsDelete)
                .OrderBy(t => t.Title)
                .ToListAsync();
        }

        public async Task<FormTemplate?> GetByIdWithQuestionsAsync(int id)
        {
            return await _dbSet
                .Include(t => t.Questions.Where(q => !q.IsDelete).OrderBy(q => q.SortOrder))
                    .ThenInclude(q => q.Options.Where(o => !o.IsDelete).OrderBy(o => o.SortOrder))
                .Include(t => t.Translations)
                .Include(t => t.VersionHistories)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDelete);
        }
    }
}
