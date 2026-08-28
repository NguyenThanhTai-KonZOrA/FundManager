using DigitalDocumentPlatform.DataAccess.EntityModels;

namespace DigitalDocumentPlatform.Implement.Services.Interface
{
    public interface IEmployeeService
    {
        Task<Employee> GetOrCreateEmployeeFromWindowsAccountAsync(string username);
        Task<Employee?> GetEmployeeByCodeAsync(string employeeCode);
    }
}