using TalentoPlus_S.A.S.ll.Web.Data.Entities;
namespace TalentoPlus_S.A.S.ll.Web.Repositories
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<Department?> GetByNameAsync(string name);
        Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync();
        Task<int> GetEmployeeCountByDepartmentAsync(int departmentId);
    }
}
