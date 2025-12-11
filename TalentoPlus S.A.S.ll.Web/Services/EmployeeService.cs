using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Repositories;
namespace TalentoPlus_S.A.S.ll.Web.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllWithDepartmentsAsync();
        }
        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetByIdWithDepartmentAsync(id);
        }
        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(
            string? searchString, 
            int? departmentId, 
            EmployeeStatus? status)
        {
            return await _employeeRepository.SearchAsync(searchString, departmentId, status);
        }
        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            if (await _employeeRepository.EmailExistsAsync(employee.Email))
            {
                throw new InvalidOperationException("Ya existe un empleado con este email.");
            }
            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();
            return employee;
        }
        public async Task UpdateEmployeeAsync(Employee employee)
        {
            if (await _employeeRepository.EmailExistsAsync(employee.Email, employee.Id))
            {
                throw new InvalidOperationException("Ya existe otro empleado con este email.");
            }
            await _employeeRepository.UpdateAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }
        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
            {
                throw new KeyNotFoundException($"No se encontró el empleado con ID {id}");
            }
            await _employeeRepository.DeleteAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }
        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            return await _employeeRepository.EmailExistsAsync(email, excludeId);
        }
    }
}
