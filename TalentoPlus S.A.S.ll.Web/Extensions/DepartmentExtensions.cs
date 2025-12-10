using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Models.ViewModels;

namespace TalentoPlus_S.A.S.ll.Web.Extensions
{
    public static class DepartmentExtensions
    {
        public static DepartmentViewModel ToViewModel(this Department department, int employeeCount = 0)
        {
            if (department == null) return null!;

            return new DepartmentViewModel
            {
                Id = department.Id,
                Name = department.Name,
                EmployeeCount = employeeCount
            };
        }

        public static List<DepartmentViewModel> ToViewModelList(this IEnumerable<Department> departments)
        {
            return departments?.Select(d => d.ToViewModel()).ToList() ?? new List<DepartmentViewModel>();
        }
    }
}

