using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TalentoPlus_S.A.S.ll.Web.Data.Entities;
using TalentoPlus_S.A.S.ll.Web.Services;

namespace TalentoPlus_S.A.S.ll.Web.Areas.Admin.Pages.Employees
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public CreateModel(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        [BindProperty]
        public Employee Employee { get; set; } = new Employee();
        
        public SelectList Departments { get; set; } = null!;

        public async Task OnGetAsync()
        {
            await LoadDepartmentsAsync();
            // Set default values
            Employee.HireDate = DateTime.Today;
            Employee.Status = EmployeeStatus.Active;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDepartmentsAsync();
                return Page();
            }

            await _employeeService.CreateEmployeeAsync(Employee);
            TempData["Success"] = "Empleado creado exitosamente";
            return RedirectToPage("./Index");
        }

        private async Task LoadDepartmentsAsync()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            Departments = new SelectList(departments, "Id", "Name");
        }
    }
}

