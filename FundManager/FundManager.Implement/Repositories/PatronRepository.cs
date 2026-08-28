using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class PatronRepository : GenericRepository<Patron>, IPatronRepository
    {
        public PatronRepository(DigitalDocumentPlatformDbContext context) : base(context) { }

        public async Task<Patron?> GetByEmailAsync(string email)
        {
            return await _context.Patron.FirstOrDefaultAsync(p => p.Address == email);
        }

        public async Task<Patron> GetPatronByIdAsync(int patronId)
        {
            return await _context.Patron
                .FirstOrDefaultAsync(x => x.Id == patronId);
        }

        public async Task<Patron?> GetRandomPatronAsync()
        {
            int count = await _context.Patron.CountAsync();
            if (count == 0) return null;

            var random = new Random();
            int skip = random.Next(0, count);

            return await _context.Patron
                .Skip(skip)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsExistPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Patron.AnyAsync(x => x.PhoneNumber == phoneNumber && x.IsActive);
        }

        public async Task<IEnumerable<Patron>> GetAllPatronsByIdsAsync(List<int> patronIds)
        {
            return await _context.Patron.AsNoTracking()
                .Where(x => x.IsActive && !x.IsDelete && patronIds.Contains(x.Id))
                .ToListAsync();
        }
    }
}