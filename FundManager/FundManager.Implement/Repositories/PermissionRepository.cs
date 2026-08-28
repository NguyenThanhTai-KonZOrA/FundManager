using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(FundManagerDbContext context) : base(context)
        {
        }

        public async Task<bool> PermissionCodeExistsAsync(string permissionCode, int? excludeId = null)
        {
            var query = _context.Permissions.Where(p => p.PermissionCode == permissionCode);
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<List<Permission>> GetByIdsAsync(List<int> permissionIds)
        {
            return await _context.Permissions
                .Where(p => permissionIds.Contains(p.Id))
                .ToListAsync();
        }
    }
}