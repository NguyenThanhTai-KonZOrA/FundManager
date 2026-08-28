using DigitalDocumentPlatform.Common.Constants;
using DigitalDocumentPlatform.DataAccess.ApplicationDbContext;
using DigitalDocumentPlatform.DataAccess.EntityModels;
using DigitalDocumentPlatform.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DigitalDocumentPlatform.Implement.Repositories
{
    public class SignatureSessionRepository : GenericRepository<SignatureSession>, ISignatureSessionRepository
    {
        public SignatureSessionRepository(DigitalDocumentPlatformDbContext context) : base(context)
        {
        }

        public async Task<SignatureSession> GetSignatureSessionAsync(int patronId, int staffDeviceId, int patronDeviceId)
        {
            return await _context.SignatureSessions
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(x =>
                    x.PatronId == patronId &&
                    x.StaffDeviceId == staffDeviceId &&
                    x.PatronDeviceId == patronDeviceId);
        }

        public async Task<SignatureSession?> GetPendingSessionByPatronIdAsync(int patronId)
        {
            return await _context.SignatureSessions
                .Include(s => s.StaffDevice)
                .Include(s => s.PatronDevice)
                .Where(s =>
                    s.PatronId == patronId &&
                    s.Status == SignatureSessionStatus.Pending &&
                    !s.CompletedAt.HasValue)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<SignatureSession?> GetCompletedSessionByPatronIdAsync(int patronId)
        {
            return await _context.SignatureSessions
                .Where(s =>
                    s.PatronId == patronId &&
                    s.Status == SignatureSessionStatus.Signed &&
                    s.CompletedAt.HasValue)
                .OrderByDescending(s => s.CompletedAt)
                .FirstOrDefaultAsync();
        }
    }
}