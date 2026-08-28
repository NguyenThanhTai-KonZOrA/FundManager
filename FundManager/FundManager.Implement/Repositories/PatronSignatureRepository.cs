using FundManager.Common.Enum;
using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class PatronSignatureRepository : GenericRepository<PatronSignature>, IPatronSignatureRepository
    {
        public PatronSignatureRepository(DigitalDocumentPlatformDbContext context) : base(context) { }

        public async Task<PatronSignature?> GetLatestSignatureAsync(int patronId, DocumentTypeEnum documentType)
        {
            return await _context.PatronSignature
                .Where(x => x.PatronId == patronId
                    && x.DocumentType == documentType
                    && x.IsActive
                    && !x.IsDelete)
                .OrderByDescending(x => x.SignedDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PatronSignature>> GetSignaturesByDocumentTypeAsync(int patronId, DocumentTypeEnum documentType)
        {
            return await _context.PatronSignature
                .Where(x => x.PatronId == patronId
                    && x.DocumentType == documentType
                    && x.IsActive
                    && !x.IsDelete)
                .OrderByDescending(x => x.SignedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatronSignature>> GetAllSignaturesByPatronIdAsync(int patronId)
        {
            return await _context.PatronSignature
                .Where(x => x.PatronId == patronId
                    && x.IsActive
                    && !x.IsDelete)
                .OrderByDescending(x => x.SignedDate)
                .ToListAsync();
        }

        public async Task<bool> HasSignedDocumentAsync(int patronId, DocumentTypeEnum documentType)
        {
            return await _context.PatronSignature
                .AnyAsync(x => x.PatronId == patronId
                    && x.DocumentType == documentType
                    && x.IsActive
                    && !x.IsDelete);
        }

        public async Task<IEnumerable<PatronSignature>> GetAllPatronSignature()
        {
            return await _context.PatronSignature.AsNoTracking()
                        .Where(x => x.IsActive && !x.IsDelete)
                        .OrderByDescending(x => x.SignedDate)
                        .ToListAsync();
        }
    }
}