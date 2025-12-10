using ApiTalento.Web.Data.Entities;

namespace ApiTalento.Web.Repositories
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<Department?> GetByNameAsync(string name);
        Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync();
        Task<int> GetEmployeeCountByDepartmentAsync(int departmentId);
    }
}

