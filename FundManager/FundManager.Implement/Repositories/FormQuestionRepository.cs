using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
{
    public class FormQuestionRepository : GenericRepository<FormQuestion>, IFormQuestionRepository
    {
        public FormQuestionRepository(DigitalDocumentPlatformDbContext context) : base(context) { }

        public async Task<List<FormQuestion>> GetByTemplateIdAsync(int templateId)
        {
            return await _dbSet
                .Include(q => q.Options.Where(o => !o.IsDelete).OrderBy(o => o.SortOrder))
                .Where(q => q.FormTemplateId == templateId && !q.IsDelete)
                .OrderBy(q => q.SortOrder)
                .ToListAsync();
        }

        public async Task<FormQuestion?> GetByIdWithOptionsAsync(int id)
        {
            return await _dbSet
                .Include(q => q.Options.Where(o => !o.IsDelete).OrderBy(o => o.SortOrder))
                .FirstOrDefaultAsync(q => q.Id == id && !q.IsDelete);
        }

        public async Task ReorderAsync(int templateId, List<(int questionId, int newOrder)> orderMap)
        {
            var questions = await _dbSet
                .Where(q => q.FormTemplateId == templateId && !q.IsDelete)
                .ToListAsync();

            foreach (var (questionId, newOrder) in orderMap)
            {
                var q = questions.FirstOrDefault(x => x.Id == questionId);
                if (q != null) q.SortOrder = newOrder;
            }
            await _context.SaveChangesAsync();
        }
    }
}
