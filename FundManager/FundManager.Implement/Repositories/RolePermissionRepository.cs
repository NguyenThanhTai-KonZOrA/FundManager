using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class RolePermissionRepository : GenericRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(FundManagerDbContext context) : base(context)
        {
        }

        public async Task<List<RolePermission>> GetByRoleIdAsync(int roleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
        }

        public async Task RemoveByRoleIdAsync(int roleId)
        {
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();
            _context.RolePermissions.RemoveRange(rolePermissions);
        }
    }
}