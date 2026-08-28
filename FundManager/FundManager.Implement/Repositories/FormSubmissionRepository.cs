using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
{
    public class FormSubmissionRepository : GenericRepository<FormSubmission>, IFormSubmissionRepository
    {
        public FormSubmissionRepository(DigitalDocumentPlatformDbContext context) : base(context) { }

        public async Task<FormSubmission?> GetByIdWithAnswersAsync(int id)
        {
            return await _dbSet
                .Include(s => s.FormTemplate)
                .Include(s => s.Answers)
                    .ThenInclude(a => a.FormQuestion)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDelete);
        }

        public async Task<List<FormSubmission>> GetByPatronDeviceIdAsync(int patronDeviceId)
        {
            return await _dbSet
                .Include(s => s.FormTemplate)
                .Where(s => s.PatronDeviceId == patronDeviceId && !s.IsDelete)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }

        public async Task<List<FormSubmission>> GetByTemplateIdAsync(int templateId)
        {
            return await _dbSet
                .Where(s => s.FormTemplateId == templateId && !s.IsDelete)
                .OrderByDescending(s => s.SubmittedAt)
                .ToListAsync();
        }
    }
}
