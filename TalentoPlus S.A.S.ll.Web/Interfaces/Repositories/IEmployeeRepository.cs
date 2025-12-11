using TalentoPlus_S.A.S.ll.Web.Data.Entities;
namespace TalentoPlus_S.A.S.ll.Web.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetAllWithDepartmentsAsync();
        Task<Employee?> GetByIdWithDepartmentAsync(int id);
        Task<IEnumerable<Employee>> SearchAsync(string? searchString, int? departmentId, EmployeeStatus? status);
        Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
        Task<IEnumerable<Employee>> GetByStatusAsync(EmployeeStatus status);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }
}
