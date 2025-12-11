using TalentoPlus_S.A.S.ll.Web.Data.Entities;
namespace TalentoPlus_S.A.S.ll.Web.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string? searchString, int? departmentId, EmployeeStatus? status);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }
}
