using FundManager.DataAccess.EntityModels;

namespace FundManager.Implement.Services.Interface
{
    public interface IEmployeeService
    {
        Task<Employee> GetOrCreateEmployeeFromWindowsAccountAsync(string username);
        Task<Employee?> GetEmployeeByCodeAsync(string employeeCode);
    }
}