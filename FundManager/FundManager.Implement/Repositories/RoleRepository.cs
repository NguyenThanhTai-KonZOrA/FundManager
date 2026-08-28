using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(FundManagerDbContext context) : base(context)
        {
        }

        public async Task<Role?> GetRoleWithPermissionsAsync(int roleId)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions!)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<List<Role>> GetAllRolesWithPermissionsAsync()
        {
            return await _context.Roles
                .AsNoTracking()
                .Include(r => r.RolePermissions!)
                    .ThenInclude(rp => rp.Permission)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<bool> RoleNameExistsAsync(string roleName, int? excludeId = null)
        {
            var query = _context.Roles.Where(r => r.RoleName == roleName);
            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}