using Microsoft.EntityFrameworkCore;
using TalentoPlus_S.A.S.ll.Web.Data;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
namespace TalentoPlus_S.A.S.ll.Web.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Employee>> GetAllWithDepartmentsAsync()
        {
            return await _dbSet
                .Include(e => e.Department)
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }
        public async Task<Employee?> GetByIdWithDepartmentAsync(int id)
        {
            return await _dbSet
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<IEnumerable<Employee>> SearchAsync(
            string? searchString, 
            int? departmentId, 
            EmployeeStatus? status)
        {
            var query = _dbSet.Include(e => e.Department).AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(e => 
                    e.FirstName.Contains(searchString) ||
                    e.LastName.Contains(searchString) ||
                    e.Email.Contains(searchString) ||
                    e.Position.Contains(searchString));
            }
            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }
            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }
            return await query
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToListAsync();
        }
        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
        {
            return await _dbSet
                .Include(e => e.Department)
                .Where(e => e.DepartmentId == departmentId)
                .OrderBy(e => e.LastName)
                .ToListAsync();
        }
        public async Task<IEnumerable<Employee>> GetByStatusAsync(EmployeeStatus status)
        {
            return await _dbSet
                .Include(e => e.Department)
                .Where(e => e.Status == status)
                .OrderBy(e => e.LastName)
                .ToListAsync();
        }
        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _dbSet.AsQueryable();
            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }
            return await query.AnyAsync(e => e.Email == email);
        }
    }
}
