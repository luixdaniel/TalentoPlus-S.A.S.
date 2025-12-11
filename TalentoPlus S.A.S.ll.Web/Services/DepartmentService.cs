using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Repositories;
namespace TalentoPlus_S.A.S.ll.Web.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetDepartmentsWithEmployeesAsync();
        }
        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _departmentRepository.GetByIdAsync(id);
        }
        public async Task<Department> CreateDepartmentAsync(Department department)
        {
            if (await NameExistsAsync(department.Name))
            {
                throw new InvalidOperationException("Ya existe un departamento con este nombre.");
            }
            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveChangesAsync();
            return department;
        }
        public async Task UpdateDepartmentAsync(Department department)
        {
            await _departmentRepository.UpdateAsync(department);
            await _departmentRepository.SaveChangesAsync();
        }
        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null)
            {
                throw new KeyNotFoundException($"No se encontró el departamento con ID {id}");
            }
            var employeeCount = await GetEmployeeCountAsync(id);
            if (employeeCount > 0)
            {
                throw new InvalidOperationException($"No se puede eliminar el departamento porque tiene {employeeCount} empleado(s) asignado(s).");
            }
            await _departmentRepository.DeleteAsync(department);
            await _departmentRepository.SaveChangesAsync();
        }
        public async Task<bool> NameExistsAsync(string name)
        {
            var department = await _departmentRepository.GetByNameAsync(name);
            return department != null;
        }
        public async Task<int> GetEmployeeCountAsync(int departmentId)
        {
            return await _departmentRepository.GetEmployeeCountByDepartmentAsync(departmentId);
        }
    }
}
