using Microsoft.EntityFrameworkCore;
using TalentoPlus_S.A.S.ll.Web.Data;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
namespace TalentoPlus_S.A.S.ll.Web.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<Department?> GetByNameAsync(string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(d => d.Name == name);
        }
        public async Task<IEnumerable<Department>> GetDepartmentsWithEmployeesAsync()
        {
            return await _dbSet
                .Include(d => d.Employees)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }
        public async Task<int> GetEmployeeCountByDepartmentAsync(int departmentId)
        {
            var department = await _dbSet
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == departmentId);
            return department?.Employees.Count ?? 0;
        }
    }
}
