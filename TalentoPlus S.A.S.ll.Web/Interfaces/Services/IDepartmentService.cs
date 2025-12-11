using TalentoPlus_S.A.S.ll.Web.Data.Entities;
namespace TalentoPlus_S.A.S.ll.Web.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<Department> CreateDepartmentAsync(Department department);
        Task UpdateDepartmentAsync(Department department);
        Task DeleteDepartmentAsync(int id);
        Task<bool> NameExistsAsync(string name);
        Task<int> GetEmployeeCountAsync(int departmentId);
    }
}
