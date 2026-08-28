using FundManager.DataAccess.ApplicationDbContext;
using FundManager.DataAccess.EntityModels;
using FundManager.Implement.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace FundManager.Implement.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly FundManagerDbContext _context;
        public EmployeeRepository(FundManagerDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Employee?> GetEmployeeByCodeOrUserNameAsync(string employeeCode)
        {
            return await _context.Employees
                .Where(e => (e.EmployeeCode == employeeCode || e.WindowAccount == employeeCode) && e.IsActive).Include(x => x.EmployeeRoles)
                .FirstOrDefaultAsync();
        }

        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _context.Employees
                .Where(e => e.Email == email && e.IsActive).Include(x => x.EmployeeRoles)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Employee>> GetActiveEmployeesAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.IsActive)
                .OrderBy(e => e.Id)
                .ToListAsync();
        }
    }
}