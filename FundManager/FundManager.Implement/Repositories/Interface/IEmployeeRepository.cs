using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Repositories.Interface
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetEmployeeByCodeOrUserNameAsync(string employeeCode);
        Task<List<Employee>> GetActiveEmployeesAsync();
        Task<Employee?> GetEmployeeByEmailAsync(string email);
    }
}