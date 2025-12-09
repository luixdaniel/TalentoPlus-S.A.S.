using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Services;

namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Pages.Departments
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IDepartmentService _departmentService;

        public IndexModel(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public IEnumerable<Department> Departments { get; set; } = new List<Department>();

        public async Task OnGetAsync()
        {
            Departments = await _departmentService.GetAllDepartmentsAsync();
        }
    }
}

